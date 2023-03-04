using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class userData : MonoBehaviour
{
    public String Username = "guest77777";
    private void Awake()
    { DontDestroyOnLoad(this);}

}
