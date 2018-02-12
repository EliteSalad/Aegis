using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climb : MonoBehaviour {

    bool climbing = false;
    bool goingUp = false;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       
        if (Input.GetAxis("Vertical") != 0)
            goingUp = true;

    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "climbable" && goingUp)
            { }
    

    }
}
