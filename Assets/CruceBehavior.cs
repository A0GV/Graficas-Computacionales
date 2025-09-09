using System.Collections.Generic;
using UnityEngine;

public class CruceBehavior : MonoBehaviour
{
    public TCPIPServerAsync server;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (server == null)
        {
            server = FindFirstObjectByType<TCPIPServerAsync>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (server != null)
        {
            // Debug.Log(mesg);
        }
        
    }
}
