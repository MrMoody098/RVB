using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Power { dash, doublejump, superspeed, superjump, health };

public class PickUp : MonoBehaviour
{
    public Power power;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("triggered!!!!");
        if (other.gameObject.GetComponent<Player>())
        {
            Debug.Log(power.ToString() + " initialized");
            Player player = other.gameObject.GetComponent<Player>();
            if (player != null)
            {
                GetComponent<Collider>().enabled = false;
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<Light>().enabled = false;
                Destroy(gameObject, 2);
                GetComponent<AudioSource>().Play();

                if (power == Power.dash) { player.movement.dash(); }
                if (power == Power.superspeed) { player.movement.superSpeed(); }
                if (power == Power.doublejump) { player.movement.doubleJump(); }
                if (power == Power.superjump) { player.movement.superJump(); }
                if (power == Power.health) { player.UpHealth(1); }
            }
        }
    }
}