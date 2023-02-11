using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

public class Obsticle : MonoBehaviour
{
    private rising rising;
    public float maxDis = 100f;
    // Start is called before the first frame update
    void Start()
    {
     rising = FindObjectOfType<rising>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rising.transform.position.y >= gameObject.transform.position.y+50f)
        {
            Destroy(gameObject);
        }
    }
}
