using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeShotControllerScript : MonoBehaviour {

    public SphereCollider hitbox;
    public ParticleSystem chargeShotEffect;
    public GameObject player;
    public Transform chargeShotSpawn;
    private bool active;
    private float speed;
    public Light chargeLight;
    private float lifeTime;
    private float timeOfFire;
    public AudioSource fireSource;
    public AudioSource chargeSource;

    public GameObject homingTarget;

	// Use this for initialization
	void Start ()
    {
        active = false;
        speed = 75;
        hitbox.enabled = false;
        lifeTime = 2;
        timeOfFire = Time.time;
        homingTarget = null;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Stay with player if not active
		if(!active)
        {
            chargeShotEffect.startSize = Mathf.Lerp(0, 6, player.GetComponent<PlayerControllerScript>().getPercentageDurationForChargeShot());
            chargeLight.intensity = Mathf.Lerp(0, 40, player.GetComponent<PlayerControllerScript>().getPercentageDurationForChargeShot());

            chargeSource.volume = Mathf.Lerp(0, 8, player.GetComponent<PlayerControllerScript>().getPercentageDurationForChargeShot());
            chargeSource.pitch = Mathf.Lerp(0, 1, player.GetComponent<PlayerControllerScript>().getPercentageDurationForChargeShot());

            transform.position = chargeShotSpawn.position;
            transform.rotation = player.transform.rotation;

            if(!Input.GetButton("Fire1") && player.GetComponent<PlayerControllerScript>().getPercentageDurationForChargeShot() < 1)
            {
                Destroy(gameObject);
            }
            else if (!Input.GetButton("Fire1") && player.GetComponent<PlayerControllerScript>().getPercentageDurationForChargeShot() >= 1)
            {
               fire();
            }
        }
        else
        {
            if ((Time.time - timeOfFire) > lifeTime)
            {
                Destroy(gameObject);
            }

            if(homingTarget != null)
            {
                GetComponent<Rigidbody>().velocity = (homingTarget.transform.position - transform.position).normalized * speed;
            }
        }
	}

    public void fire()
    {
        active = true;
        hitbox.enabled = true;
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
        timeOfFire = Time.time;
        chargeSource.Stop();
        fireSource.Play();

        if(homingTarget != null)
            homingTarget.GetComponent<DamagableByPlayer>().changeLockOnStatus(false);
    }
}
