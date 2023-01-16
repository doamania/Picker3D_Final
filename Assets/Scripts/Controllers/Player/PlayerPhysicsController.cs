using System;
using Collidables;
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
        [SerializeField] private PlayerMovementController movementController;
        [SerializeField] private new Collider collider;
        [SerializeField] private new Rigidbody rigidbody;

        #endregion

        #region Private Variables
        
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
                CoreGameSignals.Instance.onLevelEnd?.Invoke();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out DiamondBox diamondBox))
            {
                if(movementController.VelocityVector.z > 0) return;
                if(manager.LevelEndDiamond == diamondBox.value) return;
                
                manager.LevelEndDiamond = diamondBox.value;
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