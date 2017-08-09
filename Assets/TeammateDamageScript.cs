using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeammateDamageScript : MonoBehaviour {

    public GameManagerScript gameManager;
    public int id;
    public AudioSource hitSource;

    //For damaage flash
    private float durationOfDamageFlash;
    private float currentTimeOfDamageFlash;
    private float timeBetweenFlashes;
    private bool flashOn;
    private float currentTimeBetweenFlashes;

    public Renderer[] rend;

    void Start()
    {
        durationOfDamageFlash = 1;
        currentTimeOfDamageFlash = Time.time - durationOfDamageFlash;
        timeBetweenFlashes = 0.05f;
        currentTimeBetweenFlashes = Time.time - timeBetweenFlashes;
        flashOn = false;
    }

    void Update()
    {
        //Damage Flash
        if (Time.time - currentTimeOfDamageFlash < durationOfDamageFlash)
        {
            if (Time.time - currentTimeBetweenFlashes > timeBetweenFlashes)
            {
                currentTimeBetweenFlashes = Time.time;

                if (flashOn)
                {
                    flashOn = false;
                    for (int i = 0; i < rend.Length; i++)
                    {
                        rend[i].material.SetFloat("_FlashTintBool", 0);
                    }
                }
                else
                {
                    flashOn = true;
                    for (int i = 0; i < rend.Length; i++)
                    {
                        rend[i].material.SetFloat("_FlashTintBool", 1);
                    }
                }
            }
        }
        else if (flashOn)
        {
            flashOn = false;
            for (int i = 0; i < rend.Length; i++)
            {
                rend[i].material.SetFloat("_FlashTintBool", 0);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerShot") || other.gameObject.CompareTag("ChargeShot") || other.gameObject.CompareTag("EnemyShot"))
        {
            gameManager.damageTeammate(id, other.gameObject.GetComponent<LaserInformation>().damage, !other.gameObject.CompareTag("EnemyShot"));
            Destroy(other.gameObject);
            hitSource.Play();
            currentTimeOfDamageFlash = Time.time;
        }
    }
}
