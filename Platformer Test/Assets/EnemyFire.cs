using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour {

    public float fireRate = 0;
    public float damage = 10;
    public LayerMask dontHit;

    Transform firePoint;
    float timeToFire = 0;

	// Use this for initialization
    void Awake()
    {
        firePoint = transform.FindChild("FirePoint");
        Debug.Log(firePoint);
    }
	void Start () {
        dontHit = gameObject.layer;
        Debug.Log(firePoint);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
