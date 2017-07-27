using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossControlScript : MonoBehaviour {

    public float maxHealth;
    private float currentHealth;
    
    public AudioSource hitSource;
    public int hits;

	// Use this for initialization
	void Start ()
    {
        //maxHealth = 100;
	}
	
	// Update is called once per frame
	//void Update () {
		
	//}

    public void resetHealth()
    {
        currentHealth = maxHealth;
    }

    public float getCurrentHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    public void damageBoss(float damage)
    {
        currentHealth -= damage;

        hitSource.Play();

        if(currentHealth < 0)
        {
            currentHealth = 0;
        }
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    //Collide with bombs
    //    if (other.gameObject.CompareTag("BombExplosion"))
    //    {
    //        currentHealth -= bombDamage;
    //        hitSource.Play();
    //        currentTimeOfDamageFlash = Time.time;

    //        if (currentHealth <= 0)
    //        {
    //            _UIController.increaseHits(hits);
    //            Instantiate(enemyExplosion, transform.position, transform.rotation);
    //            Destroy(gameObject);
    //        }
    //    }
    //    else if (other.gameObject.CompareTag("PlayerShot") || other.gameObject.CompareTag("ChargeShot"))
    //    {
    //        currentHealth -= other.gameObject.GetComponent<LaserInformation>().damage;
    //        hitSource.Play();
    //        Destroy(other.gameObject);
    //        currentTimeOfDamageFlash = Time.time;

    //        if (currentHealth <= 0)
    //        {
    //            _UIController.increaseHits(hits);
    //            Instantiate(enemyExplosion, transform.position, transform.rotation);
    //            Destroy(gameObject);
    //        }
    //    }
    //}
}
