using System.Collections;
using UnityEngine;

public class wallrunning : MonoBehaviour
{
    //wallrunning
    public LayerMask Wall;
    public float wallRunningForce;
    public float maxWallRunTime;
    public float WallRunTimer;
    private float cameraYRotation;
    public float wallJumpSideForce;
    public Vector3 cameraDirection;
    //inp 
    private float horInp;
    private float verInp;
    //direction
    public float wallCheckDistance;
    public float JumpHeight;
    private RaycastHit wallLeftHit;
    private RaycastHit wallRightHit;
    public bool LeftWall;
    public bool RightWall;

    //refs
    public Transform orientation;
    private PlayerMovement pm;
    private Rigidbody rb;

    private void Update()
    {
        Quaternion cameraRotation = Camera.main.transform.rotation;
        float yRotation = cameraRotation.eulerAngles.x;
        cameraYRotation = yRotation;
        
        //Debug.LogWarning(cameraYRotation);

        WallCheck();
        StateMachine();
       
    }
    private void FixedUpdate()
    {
        if (pm.Wallrunning) { WallRunning(); }
    }
    private void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void WallCheck() 
    {
        Ray rightRay= new Ray (transform.position, orientation.right);
        Ray leftRay = new Ray(transform.position, -orientation.right);
        RightWall = Physics.Raycast(rightRay,out wallRightHit, wallCheckDistance,LayerMask.GetMask("wallRunAble"));
        LeftWall = Physics.Raycast(leftRay,out wallLeftHit, wallCheckDistance,LayerMask.GetMask("wallRunAble"));
        Debug.DrawLine(transform.position, rightRay.origin+rightRay.direction*wallCheckDistance, Color.red);
        Debug.DrawLine(transform.position, leftRay.origin + leftRay.direction * wallCheckDistance, Color.blue);
  

    }
    private void StateMachine()
    {
        horInp = Input.GetAxisRaw("Horizontal");
        verInp = Input.GetAxisRaw("Vertical");
        //state 1 -wall running
        if ((LeftWall || RightWall) && verInp > 0 && !pm.grounded &!pm.isWalled)
        {
           
            //start wall run
            if (!pm.Wallrunning)
            { StartWallRun(); }

            if (Input.GetButtonDown("Jump")) { WallJump(); return; };
        }
        //state 3 -none
        else
        {
            if (pm.Wallrunning)
            { StopWallRun(); }
        }
        
    }
    private void StartWallRun() 
    {
     
        wallRunningForce = pm.moveSpeed;
        pm.Wallrunning= true;
    }
    private void WallRunning()
    {
        //remove gravity and reset y velocity
        rb.velocity = new Vector3(rb.velocity.x,0,rb.velocity.z);
       // rb.useGravity= false;


        //adding force towards players y direction to allow for climbing walls
        Quaternion cameraRotation = Camera.main.transform.rotation;
        float yRotation = cameraRotation.eulerAngles.x;
        cameraYRotation = yRotation;

        //Debug.LogWarning(cameraYRotation); //debug to showcamera rotation i used this to figure out how i should proccess the camerYRotation

        Vector3 upForce = new Vector3(0,cameraYRotation,0);
        //when looking down in the downward range add force of -CameraYrotation this is because we want to go down
        if (cameraYRotation <= 90) { rb.AddForce(-upForce*0.8f,ForceMode.Force); }
        //otherwise if the player is looking up in a range of 
        else if (cameraYRotation <= 360 && cameraYRotation >= 270) { cameraYRotation -= 360; rb.AddForce(upForce*0.8f,ForceMode.Force); Debug.LogWarning(cameraYRotation); }
        

        //finding normal and forward direction of wall to allow player to run along it
        Vector3 wallNormal = RightWall ? wallRightHit.normal : wallLeftHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);
       
        //this script basicaly compares the two magniutes to find if the player should be running backwards or forwads on a wall using their orientation
        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        { 
        wallForward= -wallForward;
        }

        //adding forward force
        rb.AddForce((wallForward+cameraDirection)*wallRunningForce,ForceMode.Force);
        //add force to the wall if pressing the approriate direction towards the wall
        if ((LeftWall && horInp > 0&&verInp>0) || (RightWall && horInp < 0 && verInp > 0))
        {
            rb.AddForce(-wallNormal * 10, ForceMode.Force);
        }
    }
    private void StopWallRun() 
    {
        Debug.Log("StopWallRun called");
       
        rb.useGravity = true; // enable gravity
        pm.Wallrunning = false; 
    }
    private void WallJump() 
    {
        print("JUMPINIG");
        Vector3 wallNormal = RightWall ? wallRightHit.normal : wallLeftHit.normal;
           rb.velocity = wallNormal * JumpHeight;
        StopWallRun();



    }
}
