
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class ClickToMove : MonoBehaviour
{
    private  NavMeshAgent player;
    private PlayerControls.PlayerActions _playerControls;
    public Camera camera;
    public GameObject DebugSphere;
    private void Start()

    {
        player = GetComponent<NavMeshAgent>();
        _playerControls = new PlayerControls().Player;
        _playerControls.Enable();
        _playerControls.LeftClick.performed += LeftClick;


    }

 /*   private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("click");
            RaycastHit hit;
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray,out hit, Mathf.Infinity, 3);
            if (!hit.Equals(null))
            {
                player.destination = hit.point;
                Debug.Log("hit");
            }
            else
            {
                Debug.Log("Fail to hit");
            }


        }
    }*/
    private void LeftClick(InputAction.CallbackContext context)
    {

        if (!context.performed) return;
        Vector2 mousepos = _playerControls.MousePosition.ReadValue<Vector2>();
    //    Debug.Log("click at " +mousepos.ToString());
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(mousepos);
        Physics.Raycast(ray, out hit, Mathf.Infinity,0b1000);
        if (!hit.Equals(null))
        {
            if (DebugSphere != null)
            {
                DebugSphere.transform.position = hit.point;
            }
            player.destination = hit.point;
     //      Debug.Log("hit");
        }
        else
        {
 //          Debug.Log("Fail to hit");
        }

    }


}
