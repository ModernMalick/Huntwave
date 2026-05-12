using System;
using ModernMalick.Huntwave.Player.Movement;
using UnityEngine;

namespace ModernMalick.Huntwave.Player
{
    [System.Serializable]
    public class BobProfile
    {
        [Header("Vertical")]
        public float verticalIntensity = 0.01f;
        public float verticalSpeed = 1f;

        [Header("Horizontal")]
        public float horizontalIntensity = 0.005f;
        public float horizontalSpeed = 0.5f;
        public float rollIntensity = 2f;

        [Header("Lag")]
        public float lagIntensity = 0.02f;
        public float lagMax = 0.1f;
        
        [Header("Dip")]
        public float dipIntensity = 0.1f;
        public float dipSpeed = 5f;
        
        [Header("Smoothing")]
        public float lerpSpeed = 10f;
        
        [Header("Pitch Offset")]
        public float pitchOffsetIntensity = 0.05f;
        public float pitchOffsetVerticalLimit = 90f;
        
        [Header("Sway")]
        public float swayIntensity = 1.5f;
        public float swayMax = 5f;
        public float swaySmoothing = 10f;
        public float swayPositionIntensity = 0.01f;
    }
    
    public class PlayerAnimation : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform target;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private PlayerGround playerGround;
        [SerializeField] private PlayerMovement playerMovement;
        
        [Header("Settings")]
        [SerializeField] private BobProfile bobProfile;

        private Vector3 _defaultPosition;
        private Quaternion _defaultRotation;
        
        private float _bobTimer;
        
        private float _dipAlpha;
        private float _lastVerticalVelocity;

        private Transform _playerCamera;
        
        private Vector3 _lastCameraRotation;
        private Vector2 _swayVelocity;

        private void Awake()
        {
            _playerCamera = Camera.main.transform;
        }

        private void OnEnable()
        {
            playerGround.onLanded.AddListener(OnLanded);
            playerMovement.onJumped.AddListener(OnJumped);
            playerMovement.onDoubleJumped.AddListener(OnJumped);
        }

        private void OnDisable()
        {
            playerGround.onLanded.RemoveListener(OnLanded);
            playerMovement.onJumped.RemoveListener(OnJumped);
            playerMovement.onDoubleJumped.RemoveListener(OnJumped);
        }

        private void Start()
        {
            _defaultPosition = target.localPosition;
            _defaultRotation = target.localRotation;
        }

        private void Update()
        {
            if (!playerGround.IsGrounded())
            {
                _lastVerticalVelocity = rb.linearVelocity.y;
            }
            HandleBobbing();
        }

        private void HandleBobbing()
        {
            var horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            var speed = horizontalVelocity.magnitude;

            var targetPosition = _defaultPosition;
            var targetRotation = _defaultRotation;

            var cameraPitch = _playerCamera.localEulerAngles.x;
            if (cameraPitch > 180) cameraPitch -= 360;
            
            var normalizedPitch = Mathf.Clamp(cameraPitch / bobProfile.pitchOffsetVerticalLimit, -1f, 1f);
            targetPosition.z += normalizedPitch * bobProfile.pitchOffsetIntensity;
            targetPosition.y += normalizedPitch * (bobProfile.pitchOffsetIntensity * 0.5f);
            
            var currentCameraRotation = _playerCamera.localEulerAngles;
            var deltaYaw = Mathf.DeltaAngle(_lastCameraRotation.y, currentCameraRotation.y);
            var deltaPitch = Mathf.DeltaAngle(_lastCameraRotation.x, currentCameraRotation.x);
    
            _lastCameraRotation = currentCameraRotation;
            
            var swayX = deltaPitch * bobProfile.swayIntensity;
            var swayY = deltaYaw * bobProfile.swayIntensity;
            
            _swayVelocity.x = Mathf.Lerp(_swayVelocity.x, Mathf.Clamp(swayX, -bobProfile.swayMax, bobProfile.swayMax), Time.deltaTime * bobProfile.swaySmoothing);
            _swayVelocity.y = Mathf.Lerp(_swayVelocity.y, Mathf.Clamp(swayY, -bobProfile.swayMax, bobProfile.swayMax), Time.deltaTime * bobProfile.swaySmoothing);
            
            targetRotation *= Quaternion.Euler(-_swayVelocity.x, _swayVelocity.y, _swayVelocity.y * 0.5f);
            targetPosition.x -= _swayVelocity.y * bobProfile.swayPositionIntensity;
            targetPosition.y -= _swayVelocity.x * bobProfile.swayPositionIntensity;
            
            if (playerGround.IsGrounded())
            {
                if (speed > 0.1f) {
                    _bobTimer += Time.deltaTime * speed;

                    var xOffset = Mathf.Cos(_bobTimer * bobProfile.horizontalSpeed) * bobProfile.horizontalIntensity;
                    var yOffset = Mathf.Sin(_bobTimer * bobProfile.verticalSpeed) * bobProfile.verticalIntensity;

                    targetPosition += new Vector3(xOffset, yOffset, 0);

                    var horizontalInput = playerMovement.MoveInput.x;
                    var zRoll = horizontalInput * -bobProfile.rollIntensity;
                    targetRotation *= Quaternion.Euler(0, 0, zRoll);
                } else {
                    _bobTimer = 0;
                }
            }

            _dipAlpha = Mathf.Lerp(_dipAlpha, 0, Time.deltaTime * bobProfile.dipSpeed);
            targetPosition.y -= _dipAlpha;
            
            var xLag = playerMovement.MoveInput.x * bobProfile.lagIntensity;
            var zLag = playerMovement.MoveInput.y * bobProfile.lagIntensity;
            targetPosition.x -= Mathf.Clamp(xLag, -bobProfile.lagMax, bobProfile.lagMax);
            targetPosition.z -= Mathf.Clamp(zLag, -bobProfile.lagMax, bobProfile.lagMax);
            
            target.localPosition = Vector3.Lerp(target.localPosition, targetPosition, Time.deltaTime * bobProfile.lerpSpeed);
            target.localRotation = Quaternion.Slerp(target.localRotation, targetRotation, Time.deltaTime * bobProfile.lerpSpeed);
        }
        
        private void TriggerDip(float strength)
        {
            _dipAlpha = strength * bobProfile.dipIntensity;
        }

        private void OnJumped()
        {
            TriggerDip(0.5f);
        }
        
        private void OnLanded()
        {
            TriggerDip(Mathf.Clamp01(Mathf.Abs(_lastVerticalVelocity) / 10f));
        }
    }
}