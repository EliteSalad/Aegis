using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFall : MonoBehaviour {

    public Vector2 velocity;
    public float platformTimer = 2;
    float platformCurrentTime = 0;
    bool activated = false;
    public bool MultiUse = false;
    public bool backAndForth = false; //can be used to both go up and then again to go down etc.

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
            activated = true;
    }
    void Start()
    {
        platformCurrentTime = platformTimer;
        //Debug.Log(platformCurrentTime);
    }
    void FixedUpdate()
    {
        if (activated == true)
        {
            GetComponent<Rigidbody2D>().velocity = velocity;
            platformCurrentTime -= Time.deltaTime;
           // Debug.Log(platformCurrentTime);
            if (platformCurrentTime < 0)
            {
                activated = false;
                GetComponent<Rigidbody2D>().velocity = velocity - velocity;
            } 
        }


        if (platformCurrentTime < 0 && backAndForth && MultiUse)
        {
          velocity = -velocity;
          platformCurrentTime = platformTimer;
        }
        else if (platformCurrentTime < 0 && MultiUse)
            platformCurrentTime = platformTimer;


    }

}
