using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.DownHealth(1);
                rb = player.GetComponent<Rigidbody>();
                rb.AddForce(0, 10, 0,ForceMode.Impulse);
            }
        }
    }
}
