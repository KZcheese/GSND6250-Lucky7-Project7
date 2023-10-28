using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private PlayerControls.PlayerActions _playerControls;

    public float walkSpeed = 2;
    public float jumpPower = 2;
    public float gravity = 0.1f;
    public float terminalVelocity = 30;

    private CharacterController _characterController;
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;
    private bool _isGrounded;

    // Values that needs to change by scale
    private float _scale = 1;
    private Vector3 _velocity = Vector3.zero;
    private float _skinWidth;
    private float _stepOffset;

    // Start is called before the first frame update
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _skinWidth = _characterController.skinWidth;
        _stepOffset = _characterController.stepOffset;

        _playerControls = new PlayerControls().Player;
        _playerControls.Enable();
        _playerControls.Jump.performed += Jump;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Walk();
        _characterController.Move(_velocity * (Time.deltaTime * _scale));
        Scale();
    }

    private void FixedUpdate()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance * _scale, groundMask);
        Gravity();
    }

    private void Scale()
    {
        _scale = transform.localScale.magnitude;
        _characterController.skinWidth = _skinWidth * _scale;
        _characterController.stepOffset = _stepOffset * _scale;
    }

    private void Gravity()
    {
        if(_isGrounded && _velocity.y < 0) _velocity.y = 0;
        else if(_velocity.y > -terminalVelocity) _velocity.y -= gravity;
    }

    private void Walk()
    {
        Vector2 walkInput = _playerControls.Walk.ReadValue<Vector2>() * walkSpeed;
        Transform playerTransform = transform;
        _velocity = playerTransform.forward * walkInput.y + playerTransform.right * walkInput.x + _velocity.y * Vector3.up;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        if(_isGrounded)
            _velocity.y = jumpPower;

    }
}