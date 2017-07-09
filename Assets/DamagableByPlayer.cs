﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableByPlayer : MonoBehaviour {

    private float laserDamage;
    private float bombDamage;

    public UIController _UIController;

    public float maxHealth;
    private float currentHealth;
    public int hits;

    public AudioSource hitSource;

    public GameObject enemyExplosion;

    private float durationOfDamageFlash;
    private float currentTimeOfDamageFlash;
    private float timeBetweenFlashes;
    private bool flashOn;
    private float currentTimeBetweenFlashes;

    public Renderer rend;

    // Use this for initialization
    void Start ()
    {
        laserDamage = 20;
        bombDamage = 100;
        currentHealth = maxHealth;

        durationOfDamageFlash = 1;
        currentTimeOfDamageFlash = 0;
        timeBetweenFlashes = 0.05f;
        currentTimeBetweenFlashes = 0;

        flashOn = false;
    }

    void Update()
    {
        if (Time.time - currentTimeOfDamageFlash < durationOfDamageFlash)
        {
            if (Time.time - currentTimeBetweenFlashes > timeBetweenFlashes)
            {
                currentTimeBetweenFlashes = Time.time;

                if (flashOn)
                {
                    flashOn = false;
                    rend.material.SetFloat("_FlashTintBool", 0);
                }
                else
                {
                    flashOn = true;
                    rend.material.SetFloat("_FlashTintBool", 1);
                }
            }
        }
        else if (flashOn)
        {
            flashOn = false;
            rend.material.SetFloat("_FlashTintBool", 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Collide with bombs
        if (other.gameObject.CompareTag("BombExplosion"))
        {
            currentHealth -= bombDamage;
            hitSource.Play();
            currentTimeOfDamageFlash = Time.time;

            if (currentHealth <= 0)
            {
                _UIController.increaseHits(hits);
                Instantiate(enemyExplosion, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
        else if (other.gameObject.CompareTag("PlayerShot"))
        {
            currentHealth -= laserDamage;
            hitSource.Play();
            Destroy(other.gameObject);
            currentTimeOfDamageFlash = Time.time;

            if (currentHealth <= 0)
            {
                _UIController.increaseHits(hits);
                Instantiate(enemyExplosion, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }
}
