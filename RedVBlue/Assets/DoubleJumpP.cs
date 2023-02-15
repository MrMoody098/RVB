using ExitGames.Client.Photon.StructWrapping;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpP : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with powerup");
        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.doubleJump();
            GetComponent<MeshRenderer>().enabled = false;
            Destroy(gameObject, 2);
            //this makes the noise
            GetComponent<AudioSource>().Play();
        }
    }
}
