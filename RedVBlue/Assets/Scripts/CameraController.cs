using UnityEngine;

public class CameraController : MonoBehaviour {
    
    public Camera camera;

    public bool isChangeFOV = true;
    public float baseFov = 80;
    [Range(.2f,1f)]
    public float fovChangeIntencity = .4f;
    float camVel = 0;

    private Rigidbody rb;
    
    private void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        rb = GetComponentInParent<Rigidbody>();
    }
    void LateUpdate() //late update will remove jitter
    {
        //UI will enable and disable this
        if (isChangeFOV){fovChange();}
        
    }

    public void fovChange()
    {
       if (rb.velocity.magnitude < .5f && camVel > 0.1f)
       { camVel *= .75f; }else{ camVel =rb.velocity.magnitude;}
        
       camera.fieldOfView = baseFov+camVel*fovChangeIntencity;
    }
}
