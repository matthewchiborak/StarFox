﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosionControlScript : MonoBehaviour {

    private float durationOfActiveHitBox;
    private float timeOfExplosion;
    //public float explosionDuration;

    public SphereCollider hitbox;
    public Light explosionLight;
    private float maxLightIntensity;

	// Use this for initialization
	void Start ()
    {
        durationOfActiveHitBox = 2;
        timeOfExplosion = Time.time;
        maxLightIntensity = 100;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(Time.time - timeOfExplosion > durationOfActiveHitBox)
        {
            hitbox.enabled = false;
        }

        explosionLight.intensity = Mathf.Lerp(maxLightIntensity,0, (Time.time - timeOfExplosion) / (durationOfActiveHitBox * 2));
    }
}
