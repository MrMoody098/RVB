using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rising : MonoBehaviour
{
    public float risingSpeed;
    private Transform spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = transform.Find("spawnPoint");
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += new Vector3 (0,risingSpeed*Time.deltaTime,0);
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name== "player")
        {
            collision.gameObject.transform.position = spawnPoint.position;
            collision.gameObject.GetComponent<CharacterAttributes>().DownHealth(1);
        }
    }
}
