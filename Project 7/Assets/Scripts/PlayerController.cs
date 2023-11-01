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

    // Climbing Stuff
    public float climbAngle = 90;
    public float climbSpeed = 3; // How fast the player climbs
    private bool _isClimbing; // Check if the player is on the ladder
    private float _slopeLimit;

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

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _skinWidth = _characterController.skinWidth;
        _stepOffset = _characterController.stepOffset;
        _slopeLimit = _characterController.slopeLimit;

        _playerControls = new PlayerControls().Player;
        _playerControls.Enable();
        _playerControls.Jump.performed += Jump;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if(_isClimbing)
        {
            Climb();
            _isClimbing = false;
            _characterController.slopeLimit = _slopeLimit;
        } 
        else Walk();
        _characterController.Move(_velocity * (Time.deltaTime * _scale));
        Scale();

    }

    private void FixedUpdate()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance * _scale, groundMask);
        Gravity();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, groundDistance * _scale);
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

    private void Climb()
    {
        Vector2 climbInput = _playerControls.Walk.ReadValue<Vector2>();
        _velocity = climbInput.y * climbSpeed * Vector3.up + transform.right * climbInput.x;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        if(_isGrounded) _velocity.y = jumpPower;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(!hit.gameObject.CompareTag("Ladder") || _isClimbing) return;

        _isClimbing = true;
        _characterController.slopeLimit = climbAngle;
    }
    
}