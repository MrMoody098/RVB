using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharcterAtributes : MonoBehaviour
{
    public float health;
    public float maxHealth = 3;

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
        health -= amount;
        print(health);

    }
}
