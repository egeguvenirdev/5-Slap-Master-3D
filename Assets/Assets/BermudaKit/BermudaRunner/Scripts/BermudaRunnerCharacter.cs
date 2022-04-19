using Bermuda.Runner;
using Bermuda.Animation;
using PathCreation;
using UnityEngine;
using Ali.Helper;
using DG.Tweening;
using System.Collections;

namespace Bermuda.Runner
{
    public class BermudaRunnerCharacter : MonoBehaviour
    {
        [SerializeField] private Transform _localMover;
        [SerializeField] private Transform _localMoverTarget;
        [SerializeField] private PathCreator _pathCreator;
        [SerializeField] private SimpleAnimancer _animancer;
        [SerializeField] private PlayerSwerve _playerSwerve;
        [Space]
        [SerializeField] private string _idleAnimName = "Idle";
        [SerializeField] private float _idleAnimSpeed = 1f;
        [SerializeField] private string _runAnimName = "Walking";
        [SerializeField] private float _runAnimSpeed = 2f;
        [Space]
        [SerializeField] private float _startDistance = 5f;
        [SerializeField] private float _forwardSpeed = 1f;
        [SerializeField] private float _strafeSpeed = 1f;
        [SerializeField] private float _strafeLerpSpeed = 1f;
        [SerializeField] private float _clampLocalX = 2f;
        [SerializeField] private float _rotateSpeed = 100f;
        [SerializeField] private float _rotateAngle = 100f;
        [Space]
        [SerializeField] private float _dodgeBackDistance = 2f;
        [SerializeField] private float _dodgeBackDuration = 2f;
        [Space]
        [SerializeField] private bool _enabled = true;
        [SerializeField] private bool _rotateEnabled = true;

        private Vector3 _oldPosition;
        private float _distance = 0;
        private bool _running = false;
        private bool _canSwerve = true;
        private bool _dodgingBack = false;
        private Tweener _forwardSpeedTweeen;

        void Awake()
        {
            _playerSwerve.OnSwerve += PlayerSwerve_OnSwerve;
            _distance = _startDistance;
            _oldPosition = _localMoverTarget.localPosition;
        }

        public void Init()
        {
            _pathCreator = FindObjectOfType<PathCreator>();
        }

        void UpdateRotation()
        {
            if (!_enabled)
            {
                return;
            }

            if (!_rotateEnabled)
            {
                return;
            }

            Vector3 direction = _localMoverTarget.localPosition - _oldPosition;
            direction.z += 0.6f;
            _animancer.GetAnimatorTransform().forward = Vector3.Lerp(_animancer.GetAnimatorTransform().forward, direction.normalized, _rotateSpeed * Time.deltaTime);
        }

        public void SetSwerve(bool value)
        {
            _canSwerve = value;
        }

        public void SetRotateEnabled(bool value)
        {
            _rotateEnabled = value;
        }

        public void SetEnabled(bool value)
        {
            _enabled = value;
        }

        private void PlayerSwerve_OnSwerve(Vector2 direction)
        {
            if (_running && _canSwerve)
            {
                _localMoverTarget.localPosition = _localMoverTarget.localPosition + Vector3.right * direction.x * _strafeSpeed * Time.deltaTime;
                ClampLocalPosition();
            }
        }

        void ClampLocalPosition()
        {
            Vector3 pos = _localMoverTarget.localPosition;
            pos.x = Mathf.Clamp(pos.x, -_clampLocalX, _clampLocalX);
            _localMoverTarget.localPosition = pos;

        }

        void Update()
        {

            MoveForward();
            FollowLocalMoverTarget();
            UpdateRotation();
            UpdatePath();
            _oldPosition = _localMover.localPosition;
        }

        public void StartToRun()
        {
            if (_enabled)
            {
                _running = true;
                RunAnimation();
            }
        }

        public void PlayAnimation(string animName, float animSpeed)
        {
            _animancer.PlayAnimation(animName);
            _animancer.SetStateSpeed(animSpeed);
        }

        public float GetForwardSpeed()
        {
            return _forwardSpeed;
        }

        public void SetForwardSpeed(float value)
        {
            if (_forwardSpeedTweeen != null)
            {
                _forwardSpeedTweeen.Kill();
            }
            _forwardSpeed = value;
        }

        public void SetForwardSpeed(float value, float duration)
        {
            if (_forwardSpeedTweeen != null)
            {
                _forwardSpeedTweeen.Kill();
            }
            _forwardSpeedTweeen = DOTween.To(() => _forwardSpeed, x => _forwardSpeed = x, value, duration);
        }

        public void SetLocalRotation(Vector3 eulerAngles)
        {
            _animancer.transform.localEulerAngles = eulerAngles;
        }

        public void IdleAnimation()
        {
            PlayAnimation(_idleAnimName, _idleAnimSpeed);
        }

        public void RunAnimation()
        {
            PlayAnimation(_runAnimName, _runAnimSpeed);
        }

        public float GetHorizontalRatio()
        {
            return GameUtility.GetRatioFromValue(_localMover.localPosition.x, -_clampLocalX, _clampLocalX);
        }

        public Transform GetLocalMover()
        {
            return _localMover;
        }

        public Transform GetLocalMoverTarget()
        {
            return _localMoverTarget;
        }

        void MoveForward()
        {
            if (_enabled && _running && !_dodgingBack)
            {
                _distance += _forwardSpeed * Time.deltaTime;
            }
        }

        void FollowLocalMoverTarget()
        {
            if (!_canSwerve)
            {
                return;
            }
            Vector3 nextPos = new Vector3(_localMoverTarget.localPosition.x, _localMover.localPosition.y, _localMover.localPosition.z); ;
            _localMover.localPosition = Vector3.Lerp(_localMover.localPosition, nextPos, _strafeLerpSpeed * Time.deltaTime);
        }

        void UpdatePath()
        {
            if (_enabled)
            {
                transform.position = _pathCreator.path.GetPointAtDistance(_distance);
                transform.eulerAngles = _pathCreator.path.GetRotationAtDistance(_distance).eulerAngles + new Vector3(0f, 0f, 90f);
            }
        }

        public Transform GetAnimancerTransform()
        {
            return _animancer.transform;
        }

        public bool IsDodgingBack()
        {
            return _dodgingBack;
        }

        public void DodgeBack()
        {
            StartCoroutine(DodgeBackProcess());
        }

        IEnumerator DodgeBackProcess()
        {
            _canSwerve = false;
            _dodgingBack = true;
            _animancer.PlayAnimation("Dodging Back");
            transform.eulerAngles = Vector3.zero;
            yield return DOTween.To(() => _distance, x => _distance = x, _distance - _dodgeBackDistance, _dodgeBackDuration).WaitForCompletion() ;
            _animancer.PlayAnimation(_runAnimName);
            _dodgingBack = false;
            _canSwerve = true;
        }

    }
}