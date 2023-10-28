using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public Camera playerCamera;
    private PlayerControls.PlayerActions _playerControls;
    public float lookSpeed = 0.25f;
    public float verticalLookLimit = 90;
    private float _lookY;

    // Start is called before the first frame update
    private void Start()
    {
        _playerControls = new PlayerControls().Player;
        _playerControls.Enable();
        _playerControls.Look.performed += Look;
    }

    private void Look(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        Vector2 mouseMovement = context.ReadValue<Vector2>() * lookSpeed;
        _lookY -= mouseMovement.y;
        _lookY = Mathf.Clamp(_lookY, -verticalLookLimit, verticalLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(_lookY, 0, 0);
        transform.Rotate(0, mouseMovement.x, 0);
    }
}