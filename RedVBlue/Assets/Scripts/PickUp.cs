using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SerializeField]
public enum Power { dash,doublejump,superspeed,superjump,health };
public class PickUp : MonoBehaviour
{
    public Power power;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Dash initialized");
        Player player = collision.gameObject.GetComponent<Player>();
       // player.power = this.power
        if (player != null)
        {
            //player.dash();
            GetComponent<Collider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Light>().enabled = false;
            Destroy(gameObject, 2);
            
            //this makes the noise
            GetComponent<AudioSource>().Play();

            if (power == Power.dash) { player.movement.dash(); }
            if (power == Power.superspeed) { player.movement.superSpeed(); }
            if (power == Power.doublejump) { player.movement.doubleJump(); }
            if (power == Power.superjump) { player.movement.superJump(); }
            if (power == Power.health) { player.UpHealth(1); }
        }
    }
}