using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour {

    public UIController _UIController;
    public AudioSource checkPointSource;

    void OnTriggerEnter(Collider other)
    {
        //Collide with bombs
        if (other.gameObject.CompareTag("Player"))
        {
            if(!other.gameObject.GetComponent<PlayerControllerScript>().getIsDead())
            {
                checkPointSource.Play();

                GameObject.FindWithTag("StoredInfo").GetComponent<InfoToTakeInOutOfLevel>().hitCheckPoint(transform.position.z, _UIController.getHits());

                Destroy(gameObject);
            }
        }
    }
}
