using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class CharacterAttributes : MonoBehaviourPunCallbacks, IPunObservable
{
    public float health;
    public float maxHealth = 3;
    [HideInInspector]
    public bool alive = true;
    [HideInInspector]
    public PhotonView view;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        view = GetComponent<PhotonView>();
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

        print(gameObject.name + "is dead");
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        if(stream.IsReading) { health = (int) stream.ReceiveNext(); 
            print("recieved health"); }
    }
}
