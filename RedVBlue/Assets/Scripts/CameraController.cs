using UnityEngine;

public class CameraController : MonoBehaviour {
    
    private Camera camera;
    public bool isChangeFOV = true;
    PlayerMovement pm;
    public float baseFov = 80;
    [Range(.2f,1f)]
    public float fovChangeIntencity = .4f;
    float camVel = 0;
    private void Awake()
    {
        pm = FindObjectOfType<PlayerMovement>();
        camera = GetComponentInChildren<Camera>();
    }
    void LateUpdate() //late update will remove jitter
    {
        transform.position = pm.transform.position;

        if (isChangeFOV)
        {fovChange();}
        
    }

    public void fovChange()
    {
       if (pm.rb.velocity.magnitude < .5f && camVel > 0.1f)
       { camVel *= .75f; }
       else { camVel = pm.rb.velocity.magnitude; }
        
       camera.fieldOfView = baseFov + camVel*fovChangeIntencity;
    }
}
