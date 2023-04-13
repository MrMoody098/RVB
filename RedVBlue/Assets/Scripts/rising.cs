using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rising : MonoBehaviour
{
    public float risingSpeed;
    public Transform spawnPoint;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += new Vector3 (0,risingSpeed*Time.deltaTime,0);
    }

    public  void OnTriggerEnter(Collider collision)
    {
        Debug.Log("trigger");
        if (collision.gameObject.tag == "SpawnLayer")
        {
            Transform child = collision.transform.GetChild(0);
            spawnPoint = child;
            print(spawnPoint.position);
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            print("player collision");
            collision.gameObject.transform.position = spawnPoint.position;
            collision.gameObject.GetComponent<Player>().DownHealth(1);
        }
    }
}
