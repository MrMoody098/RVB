using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public Transform player;
    public bool fovChange = true;
    PlayerMovement pm;
    public float baseFov = 80;
    float camVel = 0;
    private void Awake()
    {
        pm = FindObjectOfType<PlayerMovement>();
    }
    void Update() {
        transform.position = player.transform.position;

        if(fovChange)
        {
            if (pm.rb.velocity.magnitude < .5f && camVel > 0.1f)
            { camVel *= .75f; }
           // else if (camVel < pm.rb.velocity.magnitude) { camVel *= 1.75f; }
            else { camVel = pm.rb.velocity.magnitude; }
        
            GetComponentInChildren<Camera>().fieldOfView = baseFov + camVel*.6f;
        }

    }


}
