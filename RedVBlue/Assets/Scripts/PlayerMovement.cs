// Some stupid rigidbody based movement by Dani

using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

using Photon.Pun;
public class PlayerMovement : MonoBehaviour
{

    //Assingables
    public Transform playerCam;
    public Transform orientation;
    private CharacterAttributes characterAttributes;
    Transform cam;
    GrapplingGun gg;
    //Other
    [HideInInspector]
    public Rigidbody rb;

    //Rotation and look
    private float xRotation;
    [Range(0f, 200f)]
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
    [Range(0, 50)]
    public float wallDashForce = 15;
    [Range(0, 10)]
    public float wallTimerTime = 2;
    private float wallTimer = 0;
    private bool isWalled = false;
    private bool isAbleToMove = true;
    CapsuleCollider collider;
    PhotonView view;

    void Awake()
    {
        collider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        cam = transform.Find("camera").transform;
        cam.GetComponent<CameraController>().pm = this;

        cam.transform.parent = null;
        gg = cam.transform.GetComponentInChildren<GrapplingGun>();
        gg.GetComponent<firing>().player = transform;

        view = GetComponent<PhotonView>();
        characterAttributes = getComponenent<CharaterAtrributes>();
    }

    void Start()
    {
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


    }


    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            if (Input.GetButtonDown("Cancel")) { Application.Quit(); }
            Movement();
        }
        else
        { cam.GetComponentInChildren<Camera>().targetDisplay = 2; }

    }

    private void Update()
    {
        if (view.IsMine)
        {
            MyInput();
            Look();
            if (wallTimer > 0) { wallTimer -= Time.deltaTime; rb.useGravity = false; isWalled = true; }
            else if (!rb.useGravity) { rb.useGravity = true; rb.useGravity = true; isWalled = false; }

            checkIsGrounded();
        }

        ///print(rb.velocity); //disiplays movement vector
    }

    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");

        jumping = Keyboard.current.spaceKey.isPressed;

        //Crouching
        crouching = false;
        if (Input.GetKeyDown(KeyCode.LeftControl))
        { collider.height /= 3; moveSpeed *= .5f; crouching = true; }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        { collider.height *= 3; moveSpeed *= 2f; crouching = false; }
    }


    private void Movement()
    {
        if (!isAbleToMove) { return; }
        if (isWalled)
        {
            if (Input.GetButton("Jump"))
            {
                rb.velocity = cam.transform.forward * jumpForce;
                isWalled = false;
                rb.useGravity = true;
                wallTimer = 0;
            }
            return;
        }

        if (crouching && grounded) { rb.drag = 0; }
        else { rb.drag = 1; }

        rb.velocity += x * orientation.right * moveSpeed * Time.deltaTime;
        rb.velocity += y * orientation.forward * moveSpeed * Time.deltaTime;


        if (!grounded) { return; }
        if (Keyboard.current.spaceKey.isPressed)
        { rb.velocity += Vector3.up * jumpForce; }

        //Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        ////Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        //Set max speed
        float maxSpeed = this.maxSpeed;

        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        ////If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

    }

    private void Jump()
    {

        if (grounded)
        { rb.velocity = Vector3.up * jumpForce; }
    }

    private float desiredX;
    private void Look()
    {
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
        Ray ray = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(ray, out hit, 1.3f, ~LayerMask.NameToLayer("ground")))
        { grounded = true; }
        else { grounded = false; } //checks what is under our feet
    }

    private void OnCollisionEnter(Collision collision)
    {   //stick to wall if we are not on the ground and not grappling and hit into a wall
        if (!grounded && !gg.IsGrappling() && collision.gameObject.layer == LayerMask.NameToLayer("grapplable"))
        { wallTimer = wallTimerTime; rb.velocity = Vector3.zero; }

        //falling off map ressets position
        if (collision.gameObject.name == "fallOffPoint")
        { gameObject.transform.position = GameObject.Find("spawn(1)").transform.position; characterAttributes.DownHealth(1); }
    }

    private void setLookRotation(Transform self, Transform other, float speed)
    {
        Vector3 direction = other.transform.position - self.transform.position;

        float dot = Vector3.Dot(direction, direction.normalized);
        if (dot < .95f)
        {
            self.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(self.forward, direction, speed * Time.deltaTime, 0));
        }
        else { self.transform.LookAt(other); }

    }

}
