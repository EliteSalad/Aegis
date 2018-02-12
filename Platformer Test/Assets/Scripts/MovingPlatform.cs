using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

	public Vector2 velocity;
    public float platformTimer = 2;
    float platformCurrentTime = 0;
    

    void Start()
    {
        platformCurrentTime = platformTimer;
    }
	void FixedUpdate () {
        platformCurrentTime -= Time.deltaTime;
        //Debug.Log(platformCurrentTime);
        if (platformCurrentTime > 0)
        {
            GetComponent<Rigidbody2D>().velocity = velocity;
        }
        else
        {
            platformCurrentTime = platformTimer;
            velocity = -velocity;
        }
	}
}
