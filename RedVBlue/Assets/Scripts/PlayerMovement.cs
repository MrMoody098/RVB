// Some stupid rigidbody based movement by Dani

using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using System.Collections;
using Photon.Pun;
public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    //roatate
    //Inputs
    float vertical, horizontal;
    //quick rotate
    [Range(0, 2)]
    public float rotateDuration = 0.05f; // The duration of the rotation in seconds

    private bool isRotating = false; // Whether the player is currently rotating
    public bool  quickTurn;
    private Quaternion targetRotation; // The target rotation for the player
    //Assingables
    private Transform body;
    private CharacterAttributes characterAttributes;
    private Camera camera;
    private GrapplingGun grapplingGun;
    [HideInInspector]
    public Rigidbody rb;

    private float xRotation, yRotation;
    [Range(0f, 200f)]
    public float sensitivity = 50f;

    public float moveSpeed = 4500;
    public float maxSpeed = 20;

    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;

    public float jumpForce = 550f;

    //dashing
    [Range(0, 50)]
    public float wallDashForce = 15f;
    [Range(0, 10)]
    public float wallTimerTime = 2;
    private float wallTimer = 0;
    //dashing powerup
    public int dashCounter = 0;

    public float tapS = 0.5f; //tap speed in seconds for double tap mesurment 

    private float timeOfLastTap = 0;

    private bool isWalled = false;
    [HideInInspector]
    public bool isAbleToMove = true;
    bool isJumping, isCrouching;
    public bool grounded;
    public bool canDoubleJump = true;
  
    [Range(0,2)]
    public float distGround = 1.1f;
    private CapsuleCollider collider;


    PhotonView view;
    RoomUI ui;
    void Awake()
    {
        timeOfLastTap = 0;
        body = transform.Find("body");
        collider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        camera = GetComponentInChildren<Camera>();
        grapplingGun = camera.GetComponentInChildren<GrapplingGun>();
        grapplingGun.GetComponent<firing>().player = transform;

        view = GetComponent<PhotonView>();
        RoomUI.player = view;
        ui = FindObjectOfType<RoomUI>();
        characterAttributes = GetComponent<CharacterAttributes>();
    }
    private void FixedUpdate()
    {
        if (!view.IsMine)
        { camera.GetComponent<Camera>().targetDisplay = 2; }


    }



    private void Update()
    {
        //dash forward if double tapping W
        if (Input.GetKeyDown(KeyCode.W))
        {

            if ((Time.time - timeOfLastTap) < tapS)
            {
                rb.velocity += camera.transform.forward * jumpForce * Time.deltaTime * wallDashForce * 10;
                Debug.Log("Double tap");

            }
        }
        //dash left
        if (Input.GetKeyDown(KeyCode.A))
        {

            if ((Time.time - timeOfLastTap) < tapS)
            {
                rb.velocity -= body.right * jumpForce * Time.deltaTime * wallDashForce * 10;
                Debug.Log("Double tap");

            }

            
        }

        timeOfLastTap = Time.time;






    //photon stuff
        if (view.IsMine)
        {
            if (!isAbleToMove) { return; }

            MyInput();
            Movement();
            Look();
            if (wallTimer > 0)
            { wallTimer -= Time.deltaTime;
                rb.useGravity = false;
                isWalled = true; }
            else if (!rb.useGravity)
            { rb.useGravity = true;
                rb.useGravity = true;
                isWalled = false; }

            checkIsGrounded();

        }

    }
    private void MyInput()
    {
        isCrouching = false;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        { collider.height /= 3; moveSpeed *= .5f; isCrouching = true; }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        { collider.height *= 3; moveSpeed *= 2f; isCrouching = false; }
    }
    private void Movement()
    {
        if (!isRotating && Input.GetKeyDown(KeyCode.R))
        {
            // Set the target rotation to a 180 degree rotation around the y-axis
            targetRotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);

            // Start a coroutine to gradually rotate the player to the target rotation
            StartCoroutine(RotatePlayer());
        }
        if (!isWalled) { isRotating = true; }
        if (isWalled)
        {
            
            if (ui.controls.quickRotate && isRotating)
            {
                //lorcans rotation method
                //   rotateMe(back);

                // Set the target rotation to a 180 degree rotation around the y-axis
                targetRotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);

                // Start a coroutine to gradually rotate the player to the target rotation
                StartCoroutine(RotatePlayer());
                isRotating= false;
            }
            if (Input.GetButton("Jump"))
            {
                rb.velocity = camera.transform.forward * jumpForce;
                isWalled = false;
                rb.useGravity = true;
                wallTimer = 0;
            }
            return;
        }
    
        if (isCrouching && grounded) { rb.drag = 0; }
        else { rb.drag = 1; }
        float vertical = Input.GetAxisRaw("Horizontal");
        float horizontal = Input.GetAxisRaw("Vertical");
        float vS = vertical * vertical;
        float vSS = vS/ vertical;
        float hS = horizontal * horizontal;
        float hSS = hS / horizontal;
        ///DASHING POWERUP
        if (dashCounter > 0)
        {
            if (Input.GetKeyDown(KeyCode.F) && Input.GetKeyDown(KeyCode.A))
            {
                GetComponent<AudioSource>().Play();
                rb.velocity += vSS * body.right * moveSpeed * Time.deltaTime*wallDashForce; dashCounter -= 1;
            }
            else if (Input.GetKeyDown(KeyCode.F) && Input.GetKeyDown(KeyCode.W))
            {
                GetComponent<AudioSource>().Play();
                rb.velocity +=  hS* body.right * moveSpeed * Time.deltaTime * wallDashForce; dashCounter -= 1;
            }
        }

        rb.velocity += vertical * body.right * moveSpeed * Time.deltaTime;
        rb.velocity += horizontal * body.forward * moveSpeed * Time.deltaTime;


        //if not grounded dont jump

        //if double jump power enabled and not grounded can jump once in the air

        //can jump when on the ground
        if (grounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                rb.velocity += Vector3.up * jumpForce / 2;

            }
        }
        //double jump
        if (!grounded && canDoubleJump)
        {
            if (Input.GetButtonDown("Jump"))
            {
                rb.velocity += Vector3.up * jumpForce / 2;
                canDoubleJump = false;
            }
        }
        
        //Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

            //Find actual velocity relative to where player is looking
            Vector2 mag = FindVelRelativeToLook();
            float xMag = mag.x, yMag = mag.y;

            ////Counteract sliding and sloppy movement
            CounterMovement(vertical, horizontal, mag);

            //Set max speed
            float maxSpeed = this.maxSpeed;

            //If sliding down a ramp, add force down so player stays grounded and also builds speed
            if (isCrouching && grounded)
            { rb.AddForce(Vector3.down * Time.deltaTime * 3000);
                return; }

            ////If speed is larger than maxspeed, cancel out the input so you don't go over max speed
            if (vertical > 0 && xMag > maxSpeed) vertical = 0;
            if (vertical < 0 && xMag < -maxSpeed) vertical = 0;
            if (horizontal > 0 && yMag > maxSpeed) horizontal = 0;
            if (horizontal < 0 && yMag < -maxSpeed) horizontal = 0;

    
    }


    public void dash() 
    
    {
        dashCounter  = 5;
    }
        public void doubleJump()
        {

            canDoubleJump = true;
        }
        private void Look()
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime;

            //Find current look rotation
            Vector3 rot = camera.transform.localRotation.eulerAngles;
            yRotation = rot.y + mouseX;

            //Rotate, and also make sure we dont over- or under-rotate.
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //Perform the rotations
            camera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
            body.transform.localRotation = Quaternion.Euler(0, yRotation, 0);

        }
        private void CounterMovement(float x, float y, Vector2 mag)
        {
            if (!grounded || isJumping) return;

            //Counter movement
            if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
            {
                rb.AddForce(moveSpeed * body.transform.right * Time.deltaTime * -mag.x * counterMovement);
            }
            if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
            {
                rb.AddForce(moveSpeed * body.transform.forward * Time.deltaTime * -mag.y * counterMovement);
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
            float lookAngle = body.transform.eulerAngles.y;

            float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

            float u = Mathf.DeltaAngle(lookAngle, moveAngle); //magnitude of l+m angles
            float v = 90 - u; //pivots X rotation so they rotate at the same speed
                              //ie Y (left and right) and X rotate at the same speed and finish look ah the same time

            float magnitue = rb.velocity.magnitude; //total over all speed
            float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad); //total required resistance to fight against
            float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad); //X and Y

            return new Vector2(xMag, yMag);
        }
        public void checkIsGrounded()
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, -transform.up);
            if (Physics.Raycast(ray, out hit, distGround, ~LayerMask.NameToLayer("grapplable")))
            { grounded = true; } else { grounded = false; }

        }

      
        private void OnCollisionEnter(Collision collision)
        {   //stick to wall if we are not on the ground and not grappling and hit into a wall
        if (!grounded && !grapplingGun.IsGrappling()
            && collision.gameObject.layer == LayerMask.NameToLayer("grapplable"))
        { wallTimer = wallTimerTime; rb.velocity = Vector3.zero; }// rotating = true; back = -transform.forward; }

            //falling off map ressets position

            if (collision.gameObject.name == "fallOffPoint")
            { gameObject.transform.position = GameObject.Find("spawn(1)").transform.position;
                characterAttributes.DownHealth(1); }

        }


    IEnumerator RotatePlayer()
    {
        isRotating = true;

        float elapsedTime = 0f;
        Quaternion startingRotation = transform.rotation;

        while (elapsedTime < rotateDuration)
        {
            float t = elapsedTime / rotateDuration;
            transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        isRotating = false;
    }



    //lorcans rotation

  //bool rotating = false;
        //Vector3 back;
        //public void rotateMe(Vector3 to) //converts vector directions to quaternions
        //{
        //    Vector3 direction = to - transform.forward;
        //    float d = Vector3.Dot(to, direction.normalized);
        //    if (d > 0.05)
        //    {
        //        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, direction, 10 * Time.deltaTime, 0));
        //    }
        //    else { rotating = false; back = Vector3.zero; }
        //}










    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
            if (stream.IsWriting) { stream.SendNext(xRotation); stream.SendNext(yRotation); }
            if (stream.IsReading)
            { xRotation = (float)stream.ReceiveNext(); yRotation = (float)stream.ReceiveNext();
                camera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
                body.localRotation = Quaternion.Euler(0, yRotation, 0);
            }
        }
    }
