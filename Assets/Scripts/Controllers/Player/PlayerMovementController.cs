using System;
using Data.ValueObjects;
using DG.Tweening;
using Keys;
using Managers;
using Signals;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Controllers.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private PlayerManager manager;

        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private new Collider collider;

        #endregion

        #region Private Variables

        [ShowInInspector] private MovementData _data;

        [ShowInInspector] private bool _isReadyToMove, _isReadyToPlay;

        private float _xValue;
        private float2 _clampValues;
        private float _tempSpeed;
        
        #endregion

        #endregion

        #region Properties

        #region Public Properties

        public Vector3 VelocityVector => rigidbody.velocity;

        #endregion

        #endregion

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.onLevelEnd += OnLevelEnd;
        }

        private void UnSubscribeEvents()
        {
            CoreGameSignals.Instance.onLevelEnd -= OnLevelEnd;
        }

        private void OnLevelEnd()
        {
            _tempSpeed = _data.ForwardSpeed * _data.MiniGameMultiplier;
            DOTween.To(() => _tempSpeed, x => _tempSpeed = x, 5, UnityEngine.Random.Range(3, 7))
                .OnUpdate(() =>
                {
                    _data.ForwardSpeed = _tempSpeed;
                })
                .OnComplete(() =>
                {
                    _data.ForwardSpeed = 0;
                    DOVirtual.DelayedCall(3, () =>
                    {
                        CoreGameSignals.Instance.onLevelSuccessful?.Invoke();
                    });
                });
        }

        internal void SetMovementData(MovementData movementData)
        {
            _data = movementData;
        }

        private void FixedUpdate()
        {
            if (!_isReadyToPlay)
            {
                StopPlayer();
                return;
            }

            if (_isReadyToMove)
            {
                MovePlayer();
            }
            else
            {
                StopPlayerHorizontally();
            }
        }

        private void MovePlayer()
        {
            var velocity = rigidbody.velocity;
            velocity = new float3(_xValue * _data.SidewaysSpeed, velocity.y,
                _data.ForwardSpeed);
            rigidbody.velocity = velocity;

            float3 position;
            position = new float3(
                Mathf.Clamp(rigidbody.position.x, _clampValues.x,
                    _clampValues.y),
                (position = rigidbody.position).y,
                position.z);
            rigidbody.position = position;
        }

        private void StopPlayerHorizontally()
        {
            rigidbody.velocity = new float3(0, rigidbody.velocity.y, _data.ForwardSpeed);
            rigidbody.angularVelocity = float3.zero;
        }

        private void StopPlayer()
        {
            rigidbody.velocity = float3.zero;
            rigidbody.angularVelocity = float3.zero;
        }

        internal void IsReadyToPlay(bool condition)
        {
            _isReadyToPlay = condition;
        }

        internal void IsReadyToMove(bool condition)
        {
            _isReadyToMove = condition;
        }

        internal void UpdateInputParams(HorizontalnputParams inputParams)
        {
            _xValue = inputParams.HorizontalInputValue;
            _clampValues = new float2(inputParams.HorizontalInputClampNegativeSide,
                inputParams.HorizontalInputClampPositiveSide);
        }

        internal void OnReset()
        {
            StopPlayer();
            _isReadyToPlay = false;
            _isReadyToMove = false;
        }
    }
}