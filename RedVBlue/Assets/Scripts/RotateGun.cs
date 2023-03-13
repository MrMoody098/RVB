using UnityEngine;

public class RotateGun : MonoBehaviour 
{

    private Gun grabble;

    private Quaternion desiredRotation;
    [Range(0f, 20f)]
    public float rotationSpeed = 5f;

    private void Awake()
    {grabble = gameObject.GetComponentInChildren<Gun>();}
    void Update() 
    {
        if (!grabble.IsGrappling()) 
        { desiredRotation = transform.parent.rotation;}
        else 
        { desiredRotation = Quaternion.LookRotation(grabble.GetGrapplePoint() - transform.position); }

        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }

}
