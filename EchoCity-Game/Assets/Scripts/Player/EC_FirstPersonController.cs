using EchoCity;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using static EchoCity.EchoCitySound;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    //[RequireComponent(typeof(PlayerInput))]
#endif
    public class EC_FirstPersonController : MonoBehaviour, IEventSender
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 4.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 6.0f;
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        [System.Serializable]
        public class GroundSound
        {
            public LayerMask layer;
            public SOSoundSource SoundSource;
        }

        [Header("Invoking Events")]
        [SerializeField] private SOSoundEmittedEvent soundEmittedEvent;
        [SerializeField] private SOPlayerMovementEvent playerMovementEvent;

        private AudioContext _audioContext;

        string IEventSender.SenderName => gameObject.name;
        int IEventSender.SenderID => GetInstanceID();
        bool IEventSender.IsManager => false;
        EventSenderCategoriesEnum[] IEventSender.SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.Player };

        [Header("Sound Sources")]
        [Tooltip("Sound played when character lands on ground")]
        public GroundSound[] landingSounds;
        [Tooltip("Sounds played on different types of ground")]
        public GroundSound[] footstepSounds;

        [Header("Footstep/Ground Settings")]
        public float rayDistance = 1.3f;
        public float stepSpeedMultiplier = 0.5f;
        private float _timer;
        [SerializeField] private MovementCodeEnum lastMovementCode = MovementCodeEnum.Idle;

        // Movement state tracking - accesso O(1) tramite cast dell'enum
        private float[] _movementTimers;
        private float[] _movementThresholds;

        // cinemachine
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private float _stepTimer = 0f;
        private float _lookHoldTimer = 0f;
        [SerializeField] private float lookHoldDuration = 0.2f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // landing detection
        private bool _prevGrounded;

#if ENABLE_INPUT_SYSTEM
        // PlayerInput removed: controllers read directly from StarterAssetsInputs
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return UnityEngine.InputSystem.Mouse.current != null;
#else
                return false;
#endif
            }
        }

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _audioContext = new AudioContext(this, soundEmittedEvent);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            _timer = 0f;
            lastMovementCode = MovementCodeEnum.Idle;

            // fallback: try to find StarterAssetsInputs on the Player-tagged object
            if (_input == null)
            {
                _input = GameObject.FindGameObjectWithTag("Player")?.GetComponent<StarterAssetsInputs>();
                if (_input == null)
                {
                    Debug.LogError("StarterAssetsInputs component not found. EC_FirstPersonController requires StarterAssetsInputs to read input values.");
                }
            }

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            // init landing detection
            _prevGrounded = Grounded;

            // Inizializza gli array per i timer di movimento usando MAX
            _movementTimers = new float[(int)MovementCodeEnum.MAX];
            _movementThresholds = new float[(int)MovementCodeEnum.MAX];

            // Configura le soglie per ogni tipo di movimento
            _movementThresholds[(int)MovementCodeEnum.Idle] = 0.5f;
            _movementThresholds[(int)MovementCodeEnum.Look] = 1.5f;
            _movementThresholds[(int)MovementCodeEnum.Move] = 1.5f;
            _movementThresholds[(int)MovementCodeEnum.Sprint] = 1.0f;
            _movementThresholds[(int)MovementCodeEnum.Jump] = 0.0f;  // Immediato
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            _prevGrounded = Grounded;

            JumpAndGravity();
            GroundedCheck();
            Move();
            HandlePlayerFootsteps();

            if (!_prevGrounded && Grounded && _verticalVelocity < -1.0f)
                PlayLandingSound();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void UpdateMovementState(MovementCodeEnum newCode)
        {
            if (_timer < 2.0f)
                return;

            int codeIndex = (int)newCode;

            // Incrementa il timer per il codice corrente
            _movementTimers[codeIndex] += Time.deltaTime;

            // Resetta i timer degli altri codici
            for (int i = 0; i < _movementTimers.Length; i++)
            {
                if (i != codeIndex)
                    _movementTimers[i] = 0f;
            }

            // Verifica se ha raggiunto la soglia
            CheckAndRaiseMovementEvent(newCode);
        }

        private void CheckAndRaiseMovementEvent(MovementCodeEnum code)
        {
            int codeIndex = (int)code;
            if (_movementTimers[codeIndex] >= _movementThresholds[codeIndex])
            {
                playerMovementEvent?.RaiseEvent(this, code);
                lastMovementCode = code;
                _movementTimers[codeIndex] = 0f; // reset timer after raising event
                if (code == MovementCodeEnum.Jump)
                    _movementTimers[(int)MovementCodeEnum.Idle] = 0f;

            }
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            // if there is an input
            if (_input.look.sqrMagnitude >= _threshold)
            {
                //Don't multiply mouse input by Time.deltaTime
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                // clamp our pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Update Cinemachine camera target pitch
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                // rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                // move
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            // move the player
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // Determina lo stato di movimento corrente - priorità: Move/Sprint > Look > Idle
            if (_input.move != Vector2.zero)
            {
                if (_input.sprint)
                    UpdateMovementState(MovementCodeEnum.Sprint);
                else
                    UpdateMovementState(MovementCodeEnum.Move);
            }
            else
            {
                bool hasLookInput = _input.look.sqrMagnitude >= _threshold;
                _lookHoldTimer = hasLookInput ? lookHoldDuration : Mathf.Max(0f, _lookHoldTimer - Time.deltaTime);

                if (hasLookInput || _lookHoldTimer > 0f)
                    UpdateMovementState(MovementCodeEnum.Look);
                else
                    UpdateMovementState(MovementCodeEnum.Idle);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    playerMovementEvent?.RaiseEvent(this, MovementCodeEnum.Jump);

                    _jumpTimeoutDelta = JumpTimeout; // blocca trigger multipli nello stesso salto
                    _input.jump = false;             // evita che resti true nei frame successivi
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }

        private void HandlePlayerFootsteps()
        {
            if (!Grounded) return;

            if (_input.move.sqrMagnitude < 0.01f)
            {
                _stepTimer = 0f;
                return;
            }

            _stepTimer -= Time.deltaTime;

            if (_stepTimer <= 0f)
            {
                PlayFootstepSound();

                float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
                _stepTimer = (1f / targetSpeed) / stepSpeedMultiplier;
            }
        }
        private void PlayFootstepSound()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance))
            {
                int hitLayer = hit.collider.gameObject.layer;

                for (int i = 0; i < footstepSounds.Length; i++)
                {
                    if ((footstepSounds[i].layer.value & (1 << hitLayer)) != 0)
                    {
                        SOSoundSource soundSource = footstepSounds[i].SoundSource;
                        PlayRandomAtPosition(transform.position, soundSource, _audioContext, MixerGroupEnum.SFX);
                        return;
                    }
                }
            }
        }

        private void PlayLandingSound()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance))
            {
                int hitLayer = hit.collider.gameObject.layer;

                for (int i = 0; i < landingSounds.Length; i++)
                {
                    if ((landingSounds[i].layer.value & (1 << hitLayer)) != 0)
                    {
                        SOSoundSource soundSource = landingSounds[i].SoundSource;
                        PlayAtPosition(transform.position, soundSource, _audioContext, MixerGroupEnum.SFX);
                        return;
                    }
                }
            }

        }
    }
}