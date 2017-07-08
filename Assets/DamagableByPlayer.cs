using System.Collections;
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

	// Use this for initialization
	void Start ()
    {
        laserDamage = 20;
        bombDamage = 100;
        currentHealth = maxHealth;
	}

    void OnTriggerEnter(Collider other)
    {
        //Collide with bombs
        if (other.gameObject.CompareTag("BombExplosion"))
        {
            currentHealth -= bombDamage;
            hitSource.Play();

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

            if (currentHealth <= 0)
            {
                _UIController.increaseHits(hits);
                Instantiate(enemyExplosion, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }
}
