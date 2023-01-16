using System;
using Controllers.Pool;
using DG.Tweening;
using Managers;
using Signals;
using UnityEngine;

namespace Controllers.Player
{
    public class PlayerPhysicsController : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private PlayerManager manager;
        [SerializeField] private new Collider collider;
        [SerializeField] private new Rigidbody rigidbody;

        #endregion

        #region Private Variables

        private byte _lastCollectedAmount;

        #endregion

        #endregion


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("StageArea"))
            {
                manager.ForceCommand.Execute();
                CoreGameSignals.Instance.onStageAreaEntered?.Invoke();
                InputSignals.Instance.onDisableInput?.Invoke();

                DOVirtual.DelayedCall(3, () =>
                {
                    // if (other.TryGetComponent(out PoolController pool))
                    // {
                    //     bool resul = pool.TakeStageResult(manager.StageID);
                    //     _lastCollectedAmount = pool.TakeCollectedAmount();
                    //
                    //     if (resul)
                    //     {
                    //         CoreGameSignals.Instance.onStageAreaSuccessful?.Invoke(manager.StageID);
                    //         UISignals.Instance.onSetStageColor?.Invoke(manager.StageID);
                    //         InputSignals.Instance.onEnableInput?.Invoke();
                    //         manager.StageID++;
                    //     }
                    //     else
                    //     {
                    //         CoreGameSignals.Instance.onLevelFailed?.Invoke();
                    //     }
                    // }
                    var result = other.transform.parent.GetComponentInChildren<PoolController>()
                        .TakeStageResult(manager.StageID);
                    if (result)
                    {
                        CoreGameSignals.Instance.onStageAreaSuccessful?.Invoke(manager.StageID);
                        UISignals.Instance.onSetStageColor?.Invoke(manager.StageID);
                        InputSignals.Instance.onEnableInput?.Invoke();
                        manager.StageID++;
                    }
                    else CoreGameSignals.Instance.onLevelFailed?.Invoke();
                });
                return;
            }

            if (other.CompareTag("Finish"))
            {
                print(_lastCollectedAmount);
                CoreGameSignals.Instance.onLevelEnd?.Invoke(_lastCollectedAmount);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            var transform1 = manager.transform;
            var position = transform1.position;
            Gizmos.DrawSphere(new Vector3(position.x, position.y - 1.2f, position.z + 1f), 1.65f);
        }

        public void OnReset()
        {
        }
    }
}