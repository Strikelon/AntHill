using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 50f;
    [SerializeField] private float _screenMargin = 50f;
    [SerializeField] private float _scrollSpeed = 5f;
    [SerializeField] private float _maxXPosotion;
    [SerializeField] private float _minXPosotion;
    [SerializeField] private float _maxYPosotion;
    [SerializeField] private float _minYPosotion;
    [SerializeField] private float _maxZPosotion;
    [SerializeField] private float _minZPosotion;
    [SerializeField] private Collider _playerBase;

    private Transform _transform;
    private float _sizeDivider = 4;

    private void Awake()
    {
        transform.position = new Vector3(
        _playerBase.bounds.center.x + _playerBase.bounds.size.magnitude / _sizeDivider,
        transform.position.y,
        _playerBase.bounds.center.z - _playerBase.bounds.size.magnitude
       );
        _transform = transform;
    }

    private void Update()
    {
        Vector3 moveDirection = Vector3.zero;
        float zoomValue = 0;

        if (Input.mousePosition.y >= Screen.height - _screenMargin)
        {
            moveDirection = Vector3.forward;
        }
        else if (Input.mousePosition.y <= _screenMargin)
        {
            moveDirection = Vector3.back;
        }
        else if (Input.mousePosition.x >= Screen.width - _screenMargin)
        {
            moveDirection = Vector3.right;
        }
        else if (Input.mousePosition.x <= _screenMargin)
        {
            moveDirection = Vector3.left;
        }

        if (moveDirection != Vector3.zero)
        {
            Vector3 newPosition = _transform.position
                + moveDirection * _moveSpeed * Time.deltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, _minXPosotion, _maxXPosotion);
            newPosition.z = Mathf.Clamp(newPosition.z, _minZPosotion, _maxZPosotion);
            _transform.position = newPosition;
        }

        float scrollInput = - Input.GetAxis("Mouse ScrollWheel");

        if(scrollInput != 0)
        {
            zoomValue = _scrollSpeed * scrollInput * Time.deltaTime;
            Vector3 newPosition = _transform.position;
            newPosition.y += zoomValue;
            newPosition.y = Mathf.Clamp(newPosition.y, _minYPosotion, _maxYPosotion);
            _transform.position = newPosition;
        }
    }
}