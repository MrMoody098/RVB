// Some stupid rigidbody based movement by Dani

using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

using Photon.Pun;
using Unity.VisualScripting;
using static UnityEngine.UI.Image;


public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{

    wallrunning wr;

    //Assignables
    private Transform body;
    private Player player;
    
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
    //superjump
    public bool canSuperJump = false;
    public float superJumpCounter = 0f;
    //superSpeed

    public float superSpeedCounter = 0f;
    //dashing
    [Range(0, 50)]
    public float wallDashForce = 15;
    [Range(0, 10)]
    public float wallTimerTime = 2;
    private float wallTimer = 0;
    public bool Wallrunning;
    
    public bool isWalled = false;
    [HideInInspector]
    public bool isAbleToMove = true;
    bool isJumping, isCrouching;
    public bool grounded;
    public bool canDoubleJump = true;
    public int dashCounter = 0;
    [Range(0,2)]
    public float distGround = 0f;
    private CapsuleCollider collider;
    public GameObject standingSurface;
    public Camera cam;
    void Awake()
    {
      
        body = transform.Find("body");
        collider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        player = this.GetComponent<Player>();
    }

    private void Update()
    {
        if (superJumpCounter >0)
        { superJumpCounter -= Time.deltaTime; } 
        else if(superJumpCounter <= 0) {canSuperJump= false;}
        if (superSpeedCounter > 0)
        { superSpeedCounter -= Time.deltaTime; }
        else if (superSpeedCounter <= 0) {  moveSpeed = 40; }

        if (player.view.IsMine)
        {
            if (!isAbleToMove) { return; }
            MyInput();
            if (!Wallrunning) { Movement(); }
            Look();
            if (wallTimer > 0)
            {
                wallTimer -= Time.deltaTime;
                rb.useGravity = false;
                isWalled = true;
            }
            else if (!rb.useGravity)
            {
                rb.useGravity = true;
                rb.useGravity = true;
                isWalled = false;
            }

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
        if (isWalled)
        {
            if (player.ui.controls.quickRotate && rotating)
                rotateMe(back);
            if (Input.GetButton("Jump"))
            {
                rb.velocity = player.camera.transform.forward * wallDashForce;
                isWalled = false;
                rb.useGravity = true;
                wallTimer = 0;
            }
            return;
        }

        if (isCrouching && grounded) { rb.drag = 0; }
        else { rb.drag = 1; }
        float vertical = Input.GetAxisRaw("Horizontal");

        //if (wr.RightWall || wr.LeftWall) { vertical = 0; }
        
        float horizontal = Input.GetAxisRaw("Vertical");
        rb.velocity += vertical * body.right * moveSpeed * Time.deltaTime;
        rb.velocity += horizontal * body.forward * moveSpeed * Time.deltaTime;
       

        //can jump when on the ground
        if (grounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (canSuperJump == true) { rb.velocity += Vector3.up * jumpForce; }
                else { rb.velocity += Vector3.up * jumpForce / 2; }
            }
        }
        //if double jump power enabled and not grounded can jump once in the air
        if (!grounded && canDoubleJump)
        { 
            if (Input.GetButtonDown("Jump"))
            {  rb.velocity += Vector3.up * jumpForce / 2; canDoubleJump = false;}
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

        ///DASHING POWERUP
        if( dashCounter > 0) 
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                GetComponent<AudioSource>().Play();
                rb.velocity = player.camera.transform.forward * jumpForce*0.8f;
                dashCounter -= 1;
            }
            if (Input.GetKeyDown(KeyCode.F)&& Input.GetKey(KeyCode.D))
            {
                GetComponent<AudioSource>().Play();
                rb.velocity = player.camera.transform.right * jumpForce;
                dashCounter -= 1;
            }
            if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.A))
            {
                GetComponent<AudioSource>().Play();
                rb.velocity = -player.camera.transform.right * jumpForce;
                dashCounter -= 1;
            }
            if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.S))
            {
                GetComponent<AudioSource>().Play();
                rb.velocity = -player.camera.transform.forward * jumpForce;
                dashCounter -= 1;
            }
        }
    }
    public void superSpeed() 
    {
        superSpeedCounter = 10;
        moveSpeed = 90;
    }
    public void superJump()
    {canSuperJump = true;
        superJumpCounter= 10;}
    public void dash() {dashCounter += 5;}
    public void doubleJump(){canDoubleJump = true;}
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime;

        //Find current look rotation
        Vector3 rot = player.camera.transform.localRotation.eulerAngles;
        yRotation = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        player.camera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
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
            Debug.DrawLine(ray.origin,ray.origin+ ray.direction * distGround,Color.yellow);
            if (Physics.Raycast(ray, out hit, distGround))
            { grounded = true; standingSurface = hit.collider.gameObject; }
            else { standingSurface = null; grounded = false; }
        }
    
        bool rotating = false;
        Vector3 back;
        public void rotateMe(Vector3 to) //converts vector directions to quaternions
        {
            Vector3 direction = to - transform.forward;
            float d = Vector3.Dot(to, direction.normalized);
            if (d > 0.05)
            {
                transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, direction, 10 * Time.deltaTime, 0));
            }
            else { rotating = false; back = Vector3.zero; }
        }
    private void OnCollisionEnter(Collision collision) {

        //stick to wall if we are not on the ground and not grappling and hit into a wall
        if (!Wallrunning &&!grounded && !player.gun.IsGrappling()
           && collision.gameObject.layer == LayerMask.NameToLayer("WallRunAble"))
        { wallTimer = wallTimerTime;   }
        
        if (!Wallrunning&&!grounded && !player.gun.IsGrappling()
            && collision.gameObject.layer == LayerMask.NameToLayer("stickable"))
        { wallTimer = wallTimerTime; rb.velocity = Vector3.zero; rotating = true; back = -transform.forward; }

        if (collision.gameObject.name == "fallOffPoint")
            { player.Dead(); }

        if (collision.gameObject.tag == "obsticle")
        { player.DownHealth(1); }

    }
        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting) { stream.SendNext(xRotation); stream.SendNext(yRotation); }
            if (stream.IsReading)
            { xRotation = (float)stream.ReceiveNext(); yRotation = (float)stream.ReceiveNext();
                player.camera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
                body.localRotation = Quaternion.Euler(0, yRotation, 0);
            }
        }
    }
