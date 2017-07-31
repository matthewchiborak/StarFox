using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakpointControl : MonoBehaviour {

    public BossControlScript ownerOfWeakpoint;
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	//void Update () {
		
	//}

    void OnTriggerEnter(Collider other)
    {
        //Collide with bombs
        //if (other.gameObject.CompareTag("BombExplosion"))
        //{
        //    ownerOfWeakpoint.damageBoss(other.gameObject.GetComponent<BombExplosionControlScript>().getBombDamage());
            
        //    Debug.Log(other.gameObject.GetComponent<BombExplosionControlScript>().getBombDamage());
        //}
        if (other.gameObject.CompareTag("PlayerShot") || other.gameObject.CompareTag("ChargeShot"))
        {
            ownerOfWeakpoint.damageBoss(other.gameObject.GetComponent<LaserInformation>().damage);
            
            Destroy(other.gameObject);
        }
    }
}
