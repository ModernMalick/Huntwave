using ModernMalick.Core.Patterns.MonoBehaviourExtensions;
using ModernMalick.Huntwave.Components.Timing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ModernMalick.Huntwave.Player.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviourExtended
    {
        [Header("Components")]
        [SerializeField] private PlayerGround playerGround;
        
        [Header("Horizontal")]
        [SerializeField] private float walkSpeed = 10f;
        [SerializeField, Range(0f, 1f)] private float airControlFactor = 1f;
        
        [Header("Jump")]
        [SerializeField] private float jumpForce = 15f;
        [SerializeField] private float coyoteTime = 0.3f;
        [SerializeField] private bool canDoubleJump = true;
        [SerializeField] private float doubleJumpForce = 15f;

        [Header("Dash")] 
        [SerializeField] private bool canDash;
        [SerializeField] private float dashSpeed = 60f;
        [SerializeField] private float dashDuration = 0.125f;
        [SerializeField] private Cooldown dashCooldown;
        
        [Component] private Rigidbody _playerBody;

        [Header("Events")] [Space(10)] 
        public UnityEvent onJumped;
        public UnityEvent onDoubleJumped;
        public UnityEvent onDashStarted;
        public UnityEvent onDashEnded;

        private Camera _camera;
        
        public Vector2 MoveInput { get; private set; }
        private float _currentSpeed;

        private float _coyoteTimeCounter;
        private bool _hasJumped;
        private bool _canDoubleJump;
        
        private float _dashTimer;
        private bool _isDashing;
        
        private new void Awake()
        {
            base.Awake();
            _camera = Camera.main;
            _currentSpeed = walkSpeed;
        }

        private void OnEnable()
        {
            playerGround.onLanded.AddListener(OnLanded);
        }

        private void OnDisable()
        {            
            playerGround.onLanded.RemoveListener(OnLanded);
        }
        
        private void Update()
        {
            UpdateCoyoteTime();
            UpdateDash();
        }
        
        private void FixedUpdate()
        {
            var input = new Vector3(MoveInput.x, 0f, MoveInput.y);
            
            if (input.sqrMagnitude > 1f)
            {
                input.Normalize();
            }

            var worldInput = _camera.transform.TransformDirection(input);

            var speed = _currentSpeed;

            if (playerGround && !playerGround.IsGrounded())
            {
                speed *= airControlFactor;
            }

            var targetVelocity = worldInput * speed;

            _playerBody.linearVelocity = new Vector3(targetVelocity.x, _playerBody.linearVelocity.y, targetVelocity.z);
        }
        
        public void OnMove(InputValue value)
        {
            MoveInput = value.Get<Vector2>();
        }
        
        public void OnJump(InputValue value)
        {
            if(!value.isPressed || Time.timeScale == 0) return;
            
            if(playerGround.IsGrounded() || _coyoteTimeCounter > 0)
            {
                PerformJump();
                onJumped.Invoke();
            } else if (canDoubleJump && _hasJumped && _canDoubleJump && !playerGround.IsGrounded())
            {
                PerformDoubleJump();
                onDoubleJumped.Invoke();
            }
        }
        
        private void PerformJump()
        {
            _coyoteTimeCounter = 0;
            _hasJumped = true;
            _playerBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        
        private void PerformDoubleJump()
        {
            _playerBody.linearVelocity = new Vector3(_playerBody.linearVelocity.x, 0);
            _playerBody.AddForce(Vector3.up * doubleJumpForce, ForceMode.Impulse);
            _canDoubleJump = false;
        }
        
        private void OnLanded()
        {
            _coyoteTimeCounter = coyoteTime;
            _hasJumped = false;
            _canDoubleJump = true;
        }

        private void UpdateCoyoteTime()
        {
            if(playerGround.IsGrounded()) return;
            if (_coyoteTimeCounter > 0)
            {
                _coyoteTimeCounter -= Time.deltaTime;
            }
            else
            {
                _coyoteTimeCounter = 0;
            }
        }
        
        public void OnDash(InputValue value)
        {
            if (!canDash || !dashCooldown.IsReady || _isDashing) return;

            dashCooldown.Reset();

            _isDashing = true;
            _dashTimer = dashDuration;

            _currentSpeed = dashSpeed;
            _playerBody.useGravity = false;
            
            onDashStarted.Invoke();
        }
        
        private void UpdateDash()
        {
            dashCooldown.Tick(Time.deltaTime);

            if (!_isDashing) return;
            
            _dashTimer -= Time.deltaTime;

            if (_dashTimer <= 0f)
            {
                StopDash();
            }
        }
        
        private void StopDash()
        {
            _isDashing = false;

            _currentSpeed = walkSpeed;
            _playerBody.useGravity = true;
            
            onDashEnded.Invoke();
        }

        public bool IsWalking()
        {
            return playerGround.IsGrounded() && MoveInput != Vector2.zero;
        }

        public void UnlockDoubleJump()
        {
            canDoubleJump = true;
        }

        public void UnlockDash()
        {
            canDash = true;
        }
    }
}