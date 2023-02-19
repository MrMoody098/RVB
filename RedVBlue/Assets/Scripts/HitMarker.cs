using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class HitMarker : MonoBehaviour
{
    float timer = 0;
    Vector3 trackPoint;
    Camera cam;
    void Update()
    {
        if (timer > 0)
        {
            transform.position = cam.WorldToScreenPoint(trackPoint);
            timer -= Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Mark(Camera cam, Vector3 point, float time)
    {
        trackPoint = point;
        this.cam = cam;
        timer = time;
    }
}
