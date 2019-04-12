using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace HT.Framework.Auxiliary
{
    /// <summary>
    /// 第一人称控制器
    /// </summary>
    [RequireComponent(typeof (CharacterController))]
    public sealed class FirstPersonController : MonoBehaviour
    {
        public static FirstPersonController Current;

        public bool IsCanControl = true;
        public bool IsWalking = false;
        public float WalkSpeed = 4;
        public float RunSpeed = 8;
        [Range(0f, 1f)] public float RunstepLenghten = 0.7f;
        public float JumpSpeed = 3;
        public float StickToGroundForce = 10;
        public float GravityMultiplier = 2;
        public MouseLook Look;
        public bool UseFovKick = true;
        public FOVKick FovKick = new FOVKick();
        public bool UseHeadBob = false;
        public CurveControlledBob HeadBob = new CurveControlledBob();
        public LerpControlledBob JumpBob = new LerpControlledBob();
        public float StepInterval = 3;
        public AudioClip[] FootstepSounds;
        public AudioClip JumpSound;
        public AudioClip LandSound;
        public string GroundTag = "Ground";
        public GameObject HitGroundCircle;

        private Camera _camera;
        private NavMeshAgent _agent;
        private CharacterController _characterController;
        private Vector3 _originalCameraPosition;
        private bool _isNaving = false;
        private Vector3 _navEndTarget;
        private UnityAction _navEndAction;
        private float _stopNavBuffer;
        private bool _jump;
        private float _yRotation;
        private Vector2 _input;
        private Vector3 _moveDir = Vector3.zero;
        private CollisionFlags _collisionFlags;
        private bool _previouslyGrounded;
        private float _stepCycle;
        private float _nextStep;
        private bool _jumping;
        private Ray _detectionRay;
        private RaycastHit _detectionRayHit;
        
        private void Awake()
        {
            Current = this;

            _camera = Main.m_Controller.MainCamera;
            _agent = GetComponent<NavMeshAgent>();
            _characterController = GetComponent<CharacterController>();
            _originalCameraPosition = _camera.transform.localPosition;
            FovKick.Setup(_camera);
            HeadBob.Setup(_camera, StepInterval);
            _stepCycle = 0f;
            _nextStep = _stepCycle / 2f;
            _jumping = false;

            Main.m_Controller.SwitchToFreeControlEvent += OnSwitchToFreeControl;
            Main.m_Controller.SwitchToFirstPersonEvent += OnSwitchToFirstPerson;
        }
        
        private void Update()
        {
            if (IsCanControl)
            {
                if (Main.m_Input.GetButtonDown("MouseLeft") && !GlobalTools.IsPointerOverUGUI())
                {
                    Ray ray = _camera.ScreenPointToRay(Main.m_Input.MousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.transform.CompareTag(GroundTag))
                        {
                            StartNavigation(hit.point);
                        }
                    }
                }

                if (_isNaving)
                {
                    NavigationUpdate();
                }
                else
                {
                    Look.LookRotation(transform, _camera.transform);

                    if (!_jump)
                    {
                        _jump = Main.m_Input.GetButtonDown("Jump");
                    }

                    if (!_previouslyGrounded && _characterController.isGrounded)
                    {
                        StartCoroutine(JumpBob.DoBobCycle());
                        PlayLandingSound();
                        _moveDir.y = 0f;
                        _jumping = false;
                    }
                    if (!_characterController.isGrounded && !_jumping && _previouslyGrounded)
                    {
                        _moveDir.y = 0f;
                    }

                    _previouslyGrounded = _characterController.isGrounded;
                }
            }
        }

        private void FixedUpdate()
        {
            if (IsCanControl)
            {
                if (!_isNaving)
                {
                    float speed;
                    GetInput(out speed);

                    Vector3 desiredMove = transform.forward * _input.y + transform.right * _input.x;

                    RaycastHit hitInfo;
                    Physics.SphereCast(transform.position, _characterController.radius, Vector3.down, out hitInfo, _characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
                    desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

                    _moveDir.x = desiredMove.x * speed;
                    _moveDir.z = desiredMove.z * speed;

                    if (_characterController.isGrounded)
                    {
                        _moveDir.y = -StickToGroundForce;

                        if (_jump)
                        {
                            _moveDir.y = JumpSpeed;
                            PlayJumpSound();
                            _jump = false;
                            _jumping = true;
                        }
                    }
                    else
                    {
                        _moveDir += Physics.gravity * GravityMultiplier * Time.fixedDeltaTime;
                    }
                    _collisionFlags = _characterController.Move(_moveDir * Time.fixedDeltaTime);

                    ProgressStepCycle(speed);
                    UpdateCameraPosition(speed);
                }
            }
        }

        private void OnDestroy()
        {
            Main.m_Controller.SwitchToFreeControlEvent -= OnSwitchToFreeControl;
            Main.m_Controller.SwitchToFirstPersonEvent -= OnSwitchToFirstPerson;
        }

        /// <summary>
        /// 设置主角位置
        /// </summary>
        public void SetPosition(Vector3 pos)
        {
            Vector3 point;
            if (DetectionPointAboveGround(pos, out point))
            {
                _characterController.enabled = false;

                transform.position = point;

                _characterController.enabled = true;
            }
        }
        /// <summary>
        /// 设置主角位置和旋转
        /// </summary>
        public void SetPosition(Vector3 pos, Vector3 rot)
        {
            Vector3 point;
            if (DetectionPointAboveGround(pos, out point))
            {
                _characterController.enabled = false;

                transform.position = point;
                SetLookXAngle(0);
                SetLookYAngle(rot.y);
                Look.LookRotationInstant(transform, _camera.transform);

                _characterController.enabled = true;
            }
        }
        /// <summary>
        /// 设置主角绕Y轴旋转的注释角度
        /// </summary>
        public void SetLookYAngle(float angle)
        {
            Look.SetLookYAngle(angle);
        }
        /// <summary>
        /// 设置主角绕X轴旋转的注释角度
        /// </summary>
        public void SetLookXAngle(float angle)
        {
            Look.SetLookXAngle(angle);
        }
        /// <summary>
        /// 主角看向目标点
        /// </summary>
        public void LookAtTarget(Vector3 target)
        {
            Vector3 dir = target - _camera.transform.position;
            Vector3 rotation = Quaternion.LookRotation(dir).eulerAngles;
            SetLookYAngle(rotation.y);
            SetLookXAngle(rotation.x);
        }

        /// <summary>
        /// 开始导航
        /// </summary>
        public void StartNavigation(Vector3 pos, UnityAction endAction = null)
        {
            pos.y += 0.5f;
            Vector3 point;
            if (DetectionPointInGround(pos, out point))
            {
                _isNaving = true;
                HitGroundCircle.SetActive(true);
                HitGroundCircle.transform.position = point;
                _stopNavBuffer = 0.5f;
                _characterController.enabled = false;
                _agent.enabled = true;
                _navEndTarget = Vector3.zero;
                _navEndAction = endAction;
                SetLookXAngle(0);
                Look.LookRotation(transform, _camera.transform);
                _agent.SetDestination(pos);
            }
        }
        /// <summary>
        /// 开始导航，结束后看向指定目标
        /// </summary>
        public void StartNavigation(Vector3 pos, Vector3 target, UnityAction endAction = null)
        {
            pos.y += 0.5f;
            Vector3 point;
            if (DetectionPointInGround(pos, out point))
            {
                _isNaving = true;
                HitGroundCircle.SetActive(true);
                HitGroundCircle.transform.position = point;
                _stopNavBuffer = 0.5f;
                _characterController.enabled = false;
                _agent.enabled = true;
                _navEndTarget = target;
                _navEndAction = endAction;
                SetLookXAngle(0);
                Look.LookRotation(transform, _camera.transform);
                _agent.SetDestination(pos);
            }
        }
        /// <summary>
        /// 导航移动中
        /// </summary>
        private void NavigationUpdate()
        {
            if (_stopNavBuffer > 0)
            {
                _stopNavBuffer -= Time.deltaTime;
                return;
            }

            if (_isNaving)
            {
                if (_agent.pathStatus == NavMeshPathStatus.PathComplete && _agent.remainingDistance < 0.1f && _agent.remainingDistance > -0.1f)
                {
                    StopNavigation();
                }
            }
        }
        /// <summary>
        /// 停止导航移动
        /// </summary>
        public void StopNavigation()
        {
            if (_isNaving)
            {
                _isNaving = false;
                HitGroundCircle.SetActive(false);
                _agent.isStopped = true;
                _agent.enabled = false;
                _characterController.enabled = true;
                SetLookYAngle(transform.localRotation.eulerAngles.y);
                if (_navEndTarget != Vector3.zero)
                {
                    LookAtTarget(_navEndTarget);
                }
                if (_navEndAction != null)
                {
                    _navEndAction();
                    _navEndAction = null;
                }
            }
        }
        
        private void PlayLandingSound()
        {
            Main.m_Audio.PlayMultipleSound(LandSound);
            _nextStep = _stepCycle + .5f;
        }
        private void PlayJumpSound()
        {
            Main.m_Audio.PlayMultipleSound(JumpSound);
        }
        private void ProgressStepCycle(float speed)
        {
            if (_characterController.velocity.sqrMagnitude > 0 && (_input.x != 0 || _input.y != 0))
            {
                _stepCycle += (_characterController.velocity.magnitude + (speed * (IsWalking ? 1f : RunstepLenghten))) * Time.fixedDeltaTime;
            }

            if (!(_stepCycle > _nextStep))
            {
                return;
            }

            _nextStep = _stepCycle + StepInterval;

            PlayFootStepAudio();
        }
        private void PlayFootStepAudio()
        {
            if (!_characterController.isGrounded)
            {
                return;
            }
            
            int n = Random.Range(1, FootstepSounds.Length);
            AudioClip clip = FootstepSounds[n];
            Main.m_Audio.PlayMultipleSound(clip);
            FootstepSounds[n] = FootstepSounds[0];
            FootstepSounds[0] = clip;
        }
        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!UseHeadBob)
            {
                return;
            }
            if (_characterController.velocity.magnitude > 0 && _characterController.isGrounded)
            {
                _camera.transform.localPosition =
                    HeadBob.DoHeadBob(_characterController.velocity.magnitude +
                                      (speed*(IsWalking ? 1f : RunstepLenghten)));
                newCameraPosition = _camera.transform.localPosition;
                newCameraPosition.y = _camera.transform.localPosition.y - JumpBob.Offset;
            }
            else
            {
                newCameraPosition = _camera.transform.localPosition;
                newCameraPosition.y = _originalCameraPosition.y - JumpBob.Offset;
            }
            _camera.transform.localPosition = newCameraPosition;
        }
        private void GetInput(out float speed)
        {
            float horizontal = Main.m_Input.GetAxis("Horizontal");
            float vertical = Main.m_Input.GetAxis("Vertical");

            bool wasWalking = IsWalking;
            IsWalking = true;
            speed = IsWalking ? WalkSpeed : RunSpeed;
            _input = new Vector2(horizontal, vertical);

            if (_input.sqrMagnitude > 1)
            {
                _input.Normalize();
            }

            if (IsWalking != wasWalking && UseFovKick && _characterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!IsWalking ? FovKick.FOVKickUp() : FovKick.FOVKickDown());
            }
        }
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            
            if (_collisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }

            body.AddForceAtPosition(_characterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
        private bool DetectionPointAboveGround(Vector3 point, out Vector3 correctPoint)
        {
            //检查point是否处于Ground之上，并限制其y值高于Ground当前点y值【半个角色身高 + 角色外表高度】，使角色不至于超出Ground
            _detectionRay.origin = point;
            _detectionRay.direction = Vector3.down;
            if (Physics.Raycast(_detectionRay, out _detectionRayHit))
            {
                if (_detectionRayHit.transform.CompareTag(GroundTag))
                {
                    correctPoint = point;
                    correctPoint.y = _detectionRayHit.point.y + _characterController.height / 2 + _characterController.skinWidth;
                    return true;
                }
                else
                {
                    correctPoint = point;
                    return false;
                }
            }
            else
            {
                correctPoint = point;
                return false;
            }
        }
        private bool DetectionPointInGround(Vector3 point, out Vector3 correctPoint)
        {
            //检查point是否处于Ground之上，并限制其y值等于Ground当前点y值
            _detectionRay.origin = point;
            _detectionRay.direction = Vector3.down;
            if (Physics.Raycast(_detectionRay, out _detectionRayHit))
            {
                if (_detectionRayHit.transform.CompareTag(GroundTag))
                {
                    correctPoint = point;
                    correctPoint.y = _detectionRayHit.point.y + 0.05f;
                    return true;
                }
                else
                {
                    correctPoint = point;
                    return false;
                }
            }
            else
            {
                correctPoint = point;
                return false;
            }
        }
        private void OnSwitchToFreeControl()
        {
            IsCanControl = false;
            _camera.transform.SetParent(Main.m_Controller.transform);
        }
        private void OnSwitchToFirstPerson()
        {
            IsCanControl = true;
            _camera.transform.SetParent(transform);
            _camera.transform.localPosition = new Vector3(0, 0.8f, 0);
        }
    }
}
