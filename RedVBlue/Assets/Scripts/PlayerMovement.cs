using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

using Photon.Pun;
using System.Collections;

public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{

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

<<<<<<< Updated upstream
=======
    //Inputs
    float vertical, horizontal;
    //quick rotate
    [Range(0,2)]
    public float rotateDuration = 0.05f; // The duration of the rotation in seconds

    private bool isRotating = false; // Whether the player is currently rotating
    private Quaternion targetRotation; // The target rotation for the player
>>>>>>> Stashed changes
    //dashing
    [Range(0, 50)]
    public float wallDashForce = 15;
    [Range(0, 10)]
    public float wallTimerTime = 2;  
    private float wallTimer = 0;

    private bool isWalled = false;
    private bool isAbleToMove = true;
    bool isJumping, isCrouching,quickTurn;
    public bool grounded;
   
    private CapsuleCollider collider;
    
    PhotonView view;

    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        body = transform.Find("body");
        collider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        camera = GetComponentInChildren<Camera>();
        grapplingGun = camera.GetComponentInChildren<GrapplingGun>();
        grapplingGun.GetComponent<firing>().player = transform;
        
        view = GetComponent<PhotonView>();

        characterAttributes = GetComponent<CharacterAttributes>();
    }
    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            if (Input.GetButtonDown("Cancel")) 
            { Cursor.visible = true; Cursor.lockState = CursorLockMode.None; isAbleToMove = false; }
            if (Input.GetButton("Fire1")) 
            {Cursor.visible = false; isAbleToMove = true;
                Cursor.lockState = CursorLockMode.Locked; }
            
        }
        else { camera.GetComponent<Camera>().targetDisplay = 2; }
    }

    private void Update()
    {
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

            if (!isRotating && Input.GetKeyDown(KeyCode.Q))
            {
                // Set the target rotation to a 180 degree rotation around the y-axis
                targetRotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);

                // Start a coroutine to gradually rotate the player to the target rotation
                StartCoroutine(RotatePlayer());
            }
        }
    }
    private void MyInput()
    {
<<<<<<< Updated upstream
=======
        vertical = Input.GetAxisRaw("Horizontal");
        horizontal = Input.GetAxisRaw("Vertical");
        // isJumping = Input.GetButtonDown("Jump");
        quickTurn = Input.GetKey(KeyCode.Tab);
        //Crouching
>>>>>>> Stashed changes
        isCrouching = false;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        { collider.height /= 3; moveSpeed *= .5f; isCrouching = true; }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        { collider.height *= 3; moveSpeed *= 2f; isCrouching = false; }
    }
    private void Movement()
    {
       
        if (Input.GetKeyDown("r")) 
        { back = -transform.forward; rotating = true; }
        if (rotating) { rotateMe(back); }

        if (isWalled)
        {
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
        rb.velocity += vertical * body.right * moveSpeed * Time.deltaTime;
        rb.velocity += horizontal * body.forward * moveSpeed * Time.deltaTime;

<<<<<<< Updated upstream
        if (!grounded) { return; }
        if (Input.GetButton("Jump"))
        { rb.velocity += Vector3.up * jumpForce/2; }
=======
        if (Input.GetButton("Jump")&& grounded)
        { rb.velocity += Vector3.up * jumpForce; }
        
        //quick turn
        if (quickTurn) 
        {
            FindVelRelativeToLook();
            print("q");
        }
>>>>>>> Stashed changes

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
        {rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;}

        ////If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (vertical > 0 && xMag > maxSpeed) vertical = 0;
        if (vertical < 0 && xMag < -maxSpeed) vertical = 0;
        if (horizontal > 0 && yMag > maxSpeed) horizontal = 0;
        if (horizontal < 0 && yMag < -maxSpeed) horizontal = 0;
        
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
        float lookAngle = body.transform.eulerAngles.y; //left and right
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg; //current angle between

        float u = Mathf.DeltaAngle(lookAngle, moveAngle); //how far from look your looking
        float v = 90 - u; //pivots X rotation so they rotate at the same speed
        //ie Y (left and right) and X rotate at the same speed and finish look ah the same time
       
        float magnitue = rb.velocity.magnitude; //total over all speed
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad); //total required resistance to fight against
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad); //X and Y

<<<<<<< Updated upstream
        return new Vector2(xMag, yMag); 
=======
        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        
        return new Vector2(xMag, yMag);
>>>>>>> Stashed changes
    }
    public void checkIsGrounded()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(ray, out hit, 1f, ~LayerMask.NameToLayer("ground")))
        { grounded = true; } else { grounded = false; }

    }
    bool rotating = false;
    Vector3 back;
    public void rotateMe(Vector3 to)
    {
        Vector3 direction = to - transform.forward;
        float d = Vector3.Dot(to, direction.normalized);
        if (d > 0.05)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, direction, 10 * Time.deltaTime, 0));
        }
        else { rotating = false; }
    }
    private void OnCollisionEnter(Collision collision)
    {   //stick to wall if we are not on the ground and not grappling and hit into a wall
        if (!grounded && !grapplingGun.IsGrappling() 
            && collision.gameObject.layer == LayerMask.NameToLayer("grapplable"))
        { wallTimer = wallTimerTime; rb.velocity = Vector3.zero; }

        //falling off map ressets position

        if (collision.gameObject.name == "fallOffPoint")
        { gameObject.transform.position = GameObject.Find("spawnPoint").transform.position; 
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
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) { stream.SendNext(xRotation); stream.SendNext(yRotation); }
        if (stream.IsReading) 
        { xRotation = (float) stream.ReceiveNext(); yRotation = (float)stream.ReceiveNext();
            camera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
            body.localRotation = Quaternion.Euler(0, yRotation, 0);
        }
    }
}
