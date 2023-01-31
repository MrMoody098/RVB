// Some stupid rigidbody based movement by Dani

using System;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerMovement : MonoBehaviour {

    //Assingables
    public Transform playerCam;
    public Transform orientation;

    Transform cam;
    GrapplingGun gg;
    //Other
    [HideInInspector]
    public Rigidbody rb;

    //Rotation and look
    private float xRotation;
    [Range(0f,200f)]
    public float sensitivity = 50f;
    private float sensMultiplier = 1f;
    
    //Movement
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public bool grounded;
    public LayerMask whatIsGround;

    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    //Crouch & Slide
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;

    //Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;

    //Input
    float x, y;
    bool jumping, sprinting, crouching;

    //Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;
    //dashing
    [Range (0,50)]
    public float wallDashForce= 15;
    [Range(0,10)]
    public float wallTimerTime = 2;
    private float wallTimer = 0;
    private bool isWalled = false;
    private bool isAbleToMove = true;
    void Awake() {
        rb = GetComponent<Rigidbody>();
        cam = transform.Find("camera").transform;
        cam.transform.parent = null;
        gg = cam.transform.GetComponentInChildren<GrapplingGun>();

    }
    
    void Start() {
        playerScale =  transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    private void FixedUpdate() {
        Movement();
    }

    private void Update() {
        MyInput();
        Look();
        if (wallTimer > 0) { wallTimer -= Time.deltaTime; rb.useGravity = false; isWalled = true; }
        else if(!rb.useGravity) { rb.useGravity = true; isWalled = false; }

        checkIsGrounded();
        ///print(rb.velocity); //disiplays movement vector
    }

    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    private void MyInput() {
       // if (isWalled) { return; }
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);
      
        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();
    }

    private void StartCrouch() {
        // transform.localScale = crouchScale;
        GetComponent<CapsuleCollider>().height /= 3; 
        //transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
       // transform.position = Vector3.MoveTowards(transform.position, transform.position - new Vector3(0, -.5f), 20 * Time.deltaTime);
        if (rb.velocity.magnitude > 0.5f) {
            if (grounded) {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }

    private void StopCrouch() {
        GetComponent<CapsuleCollider>().height *= 3;
        //transform.localScale = playerScale;
        //transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void Movement() {
        if (!isAbleToMove) { return; }
        if (isWalled) {
            if (Input.GetButton("Jump"))
            {
                //jump forward
                rb.velocity = GameObject.Find("camera").transform.forward * jumpForce;

                //i wish to calculate exactly where this jump is going to land

                //wall timer at zero means no sticking to wall
                wallTimer = 0;
            }return;} 

        //Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        ////Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        //Set max speed
        float maxSpeed = this.maxSpeed;
        
        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded && readyToJump) {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        ////If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;
        
        // Movement in air
        if (!grounded) {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        // Movement while sliding
        if (grounded && crouching) { multiplierV = .5f; }

        //Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
    }

    private void Jump() {

        if (grounded)
        {rb.velocity = Vector3.up * jumpForce;}    
    }
    
    private float desiredX;
    private void Look() {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;
        
        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;


        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }


    public void checkIsGrounded()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position,  - transform.up);
        if (Physics.Raycast(ray, out hit, 1.3f,~LayerMask.NameToLayer("ground")))
        {grounded = true;}
        else { grounded = false; } //checks what is under our feet
    }

    private void OnCollisionEnter(Collision collision)
    {   //stick to wall if we are not on the ground and not grappling and hit into a wall
        if (!grounded && !gg.IsGrappling() && collision.gameObject.layer == LayerMask.NameToLayer("grapplable"))
        {wallTimer = wallTimerTime; rb.velocity = Vector3.zero;}
   
        //falling off map ressets position
        if (collision.gameObject.name == "fallOffPoint") 
        { gameObject.transform.position = GameObject.Find("spawn(1)").transform.position; }
    }
   

}
