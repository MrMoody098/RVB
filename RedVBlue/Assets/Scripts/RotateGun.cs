using UnityEngine;

public class RotateGun : MonoBehaviour 
{

    private Gun grapple;

    private Quaternion desiredRotation;
    [Range(0f, 20f)]
    public float rotationSpeed = 5f;

    private void Awake()
    {grapple = gameObject.GetComponentInChildren<Gun>();}
    void Update() 
    {
        if (!grapple.IsGrappling())
        { desiredRotation = transform.parent.rotation;}
        else 
        { desiredRotation = Quaternion.LookRotation(grapple.GetGrapplePoint() - transform.position); }

        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }

}
