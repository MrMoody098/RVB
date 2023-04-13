using UnityEngine;

public class ROTATOR : MonoBehaviour
{
    public float speed = 10.0f;
    public RotationAxis rotationAxis = RotationAxis.X;
    public LineRenderer lineRenderer;
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    // Update is called once per frame
    void Update()
    {
        switch (rotationAxis)
        {
            case RotationAxis.X:
                transform.Rotate(Vector3.right * speed * Time.deltaTime);
                lineRenderer.SetPosition(0, transform.position);
                break;
            case RotationAxis.Y:
                transform.Rotate(Vector3.up * speed * Time.deltaTime);
                break;
            case RotationAxis.Z:
                transform.Rotate(Vector3.forward * speed * Time.deltaTime);
                break;
        }
    }
}