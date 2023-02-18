using ExitGames.Client.Photon.StructWrapping;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthP : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with powerup");
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.UpHealth(1);
            GetComponent<MeshRenderer>().enabled = false;
            Destroy(gameObject,2);
            //this makes the noise
            GetComponent<AudioSource>().Play();
        }
    }
}
