using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slidingDoor : MonoBehaviour
{
    public bool slidingAway;
    public bool slidingBack;
    public bool idle;
    public float maxDist= 4f;
    public float minDist = 0f;
    public float Dist=0;
    private void Update()
    {
        if (slidingAway) 
        {
            if (Dist <= maxDist) { Dist += 1*Time.deltaTime; gameObject.transform.Translate( 0, 0, -10 * Time.deltaTime); }
        }
        else if (slidingBack) 
        { 
            if (Dist >minDist) { Dist -= 1*Time.deltaTime;  gameObject.transform.Translate( 0, 0 ,10 * Time.deltaTime); } 
        }
        
    }
    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        { slidingAway = true; slidingBack = false; }
    }
    public void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            slidingAway = false;  slidingBack= true; }
    }
}
