using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    private float wallRunGravity;
    private float wallRunJumpForce;
    private float wallRunSpeed;
    private LayerMask whatIsWall;

    private Rigidbody rb;
    private CapsuleCollider playerCollider;

    private bool isWallRunning = false;
    private bool canWallRun = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        if (canWallRun)
        {
            //semd two rays with the length of the player collider radius out to both sides meaning
            //it will touch where the player makes contact with anything
            if (Physics.Raycast(transform.position, transform.right, playerCollider.radius + 0.15f, whatIsWall) 
            || Physics.Raycast(transform.position, -transform.right, playerCollider.radius + 0.15f, whatIsWall))
            {
                StartWallRun();
            }
            else if (isWallRunning)
            {
                StopWallRun();
            }
        }
    }

    private void StartWallRun()
    {
        isWallRunning = true;
        //want to disable gravity so i dont fall
        rb.useGravity = false;
        //when i press space push character away from the wall
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.up * wallRunJumpForce, ForceMode.Impulse);
        }
        else
        {
            //otherwise add force forward
            rb.AddForce(transform.forward * wallRunSpeed, ForceMode.Force);
        }

        Vector3 extraGravity = (Physics.gravity * wallRunGravity) - Physics.gravity;
        rb.AddForce(extraGravity, ForceMode.Force);
    }

    private void StopWallRun()
    {
        isWallRunning = false;
        rb.useGravity = true;
    }

    public void SetCanWallRun(bool canWallRun)
    {
        this.canWallRun = canWallRun;
    }
}