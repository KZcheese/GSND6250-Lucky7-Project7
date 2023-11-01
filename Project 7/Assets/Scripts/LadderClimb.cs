using System;
using UnityEngine;


[Obsolete("Bitan's Ladder code is now integrated into PlayerController.")]
public class LadderClimb : MonoBehaviour
{
    public float climbSpeed = 3.0f; // How fast the player climbs
    private bool _isClimbing; // Check if the player is on the ladder

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")) // Make sure your player has the "Player" tag
            _isClimbing = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            _isClimbing = false;
    }

    private void Update()
    {
        if(!_isClimbing) return;

        float verticalMovement = Input.GetAxis("Vertical"); // Get vertical input (W and S by default)
        Vector3 climbMovement = new Vector3(0, verticalMovement * climbSpeed, 0);
        PlayerMovement(climbMovement);
    }

    private static void PlayerMovement(Vector3 climbMovement)
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerTransform.Translate(climbMovement * Time.deltaTime);
    }
}