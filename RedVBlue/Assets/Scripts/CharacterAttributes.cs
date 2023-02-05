using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class CharacterAttributes : MonoBehaviour
{
    public float health;
    public float maxHealth = 3;
    [HideInInspector]
    public bool alive = true;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }
    public void UpHealth(float amount) {
        health += amount;
        print(health);
    
    }
    public void DownHealth(float amount)
    {
        if (health < 1) { alive = false; Dead(); }
        else { health -= amount; }
    }
    public void Dead()
    {
        PhotonNetwork.Destroy(gameObject);
        DestroyImmediate(gameObject);

    }
}
