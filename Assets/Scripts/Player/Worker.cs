using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Worker : MonoBehaviour
{
    private const string AnimatorState = "State";

    [SerializeField] private float _moveSpeed = 15;
    [SerializeField] private float _resourceYOffset = 5f;
    [SerializeField] private int _numberOfRays = 17;
    [SerializeField] private int _rayLength = 25;
    [SerializeField] private float _rayAngle = 90;
    [SerializeField] private float _obtackleAvoidRange;

    private PlayerBase _home;
    private Resource _resource;
    private Transform _transform;
    private Quaternion _rotation;
    private Rigidbody _rigidbody;
    private Vector3 _startPosition;
    private State _state = State.Free;
    private int _rayAngleMultiplier = 2;
    private Animator _animator;

    private AnimatorStates _animatorState
    {
        get { return (AnimatorStates)_animator.GetInteger(AnimatorState); }
        set { _animator.SetInteger(AnimatorState, (int)value); }
    }

    public void Initialize(PlayerBase home)
    {
        _home = home;
    }

    public void TakeResource(Resource resource)
    {
        _resource = resource;
        _state = State.FollowingTarget;
    }

    public bool IsFree()
    {
        return _state == State.Free;
    }

    private void StayFree()
    {
        _animatorState = AnimatorStates.Idle;
        _state = State.Free;
    }

    private void ReturnHome()
    {
        _animatorState = AnimatorStates.Run;
        _state = State.ReturningHome;
    }

    private void GoStartPosition()
    {
        _animatorState = AnimatorStates.Run;
        _resource = null;
        _state = State.TakingStartPosition;
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _transform = transform;
        _rotation = transform.rotation;
        _startPosition = new Vector3(
                _transform.position.x, 
                _transform.position.y,
                _transform.position.z
            );
    }

    private void Update()
    {
        if(_state == State.FollowingTarget)
        {
            ApproachTarget(_resource.transform.position);
        } 
        else if(_state == State.ReturningHome)
        {
            ApproachTarget(_home.transform.position);
            UpdateResourcePosition();
        }
        else if (_state == State.TakingStartPosition)
        {
            ApproachTarget(_startPosition);
        }
    }

    private void ApproachTarget(Vector3 target)
    {
        Vector3 direction = target - _transform.position;
        direction.y = 0;

        if (_state == State.TakingStartPosition)
        {
            if (Mathf.Round(direction.x) == 0 && Mathf.Round(direction.z) == 0)
            {
                StayFree();
            }
        }

        bool isObtacleDetected = false;

        if (direction != Vector3.zero)
        {
            _transform.rotation = Quaternion.LookRotation(direction);
            var deltaPosition = Vector3.zero;

            for (int i = 0; i < _numberOfRays; i++)
            {
                var rotationMod = Quaternion.AngleAxis((i / ((float)_numberOfRays - 1)) * _rayAngle * _rayAngleMultiplier - _rayAngle, transform.up);
                var directionRay = _rotation * rotationMod * transform.forward;

                var ray = new Ray(_transform.position, directionRay);
                RaycastHit hitInfo;

                if (Physics.Raycast(ray, out hitInfo, _rayLength))
                {
                    if (_state == State.FollowingTarget && hitInfo.collider.gameObject == _resource.gameObject ||
                        _state == State.ReturningHome && hitInfo.collider.gameObject == _home.gameObject ||
                        _state == State.ReturningHome && hitInfo.collider.gameObject == _resource.gameObject)
                    {
                        deltaPosition += (1.0f / _numberOfRays) * _moveSpeed * Time.deltaTime * directionRay;
                        continue;
                    }

                    isObtacleDetected = true;
                    deltaPosition -= (1.0f / _numberOfRays) * _moveSpeed * Time.deltaTime * directionRay;
                }
                else
                {
                    deltaPosition += (1.0f / _numberOfRays) * _moveSpeed * Time.deltaTime * directionRay;
                }
            }

            if(isObtacleDetected)
            {
                deltaPosition *= _obtackleAvoidRange;
                _transform.rotation = Quaternion.LookRotation(deltaPosition);
                _rigidbody.velocity = deltaPosition;
            }
            else
            {
                _rigidbody.velocity = direction.normalized * _moveSpeed * Time.deltaTime;
            }
        }
    }

    private void UpdateResourcePosition()
    {
        _resource.transform.position = _transform.position + Vector3.up * _resourceYOffset;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_state == State.FollowingTarget && collision.collider.gameObject == _resource.gameObject)
        {
            ReturnHome();
        }
        else if (_state == State.ReturningHome && collision.collider.gameObject == _home.gameObject)
        {
            _home.CollectResource(_resource);
            GoStartPosition();
        }
    }

    enum State
    {
        Free,
        FollowingTarget,
        ReturningHome,
        TakingStartPosition
    }

    enum AnimatorStates
    {
        Idle,
        Run
    }
}