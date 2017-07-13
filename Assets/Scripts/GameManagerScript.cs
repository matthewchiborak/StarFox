using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    public GameObject player;
    

    public GameObject[] enemiesPrefabs;

    public GameObject[] enemies;

    public GameObject[] bombPickups;

    public GameObject[] laserPickups;

    public GameObject[] silverRings;
    public GameObject[] goldRings;

    //Prefab references for the laser pickups to switch to the other laser pickups
    bool greenLasersActive;
    public GameObject blueLasers;

    private float distanceBehindPlayerToRemove;

    // Use this for initialization
    void Start ()
    {
        greenLasersActive = true;
        distanceBehindPlayerToRemove = 25;
    }
	
	// Update is called once per frame
	void Update ()
    {
        for(int i = 0; i < bombPickups.Length; i++)
        {
            if (bombPickups[i] != null)
            {
                if (bombPickups[i].transform.position.z < player.transform.position.z)
                {
                    bombPickups[i].GetComponent<BecomeTransparent>().switchTransparent(true);
                }
                else
                {
                    bombPickups[i].GetComponent<BecomeTransparent>().switchTransparent(false);
                }

                if (bombPickups[i].transform.position.z + distanceBehindPlayerToRemove < player.transform.position.z)
                {
                    Destroy(bombPickups[i]);
                }
            }
        }

        for (int i = 0; i < silverRings.Length; i++)
        {
            if (silverRings[i] != null)
            {
                if (silverRings[i].transform.position.z < player.transform.position.z)
                {
                    silverRings[i].GetComponent<BecomeTransparent>().switchTransparent(true);
                }
                else
                {
                    silverRings[i].GetComponent<BecomeTransparent>().switchTransparent(false);
                }

                if (silverRings[i].transform.position.z + distanceBehindPlayerToRemove < player.transform.position.z)
                {
                    Destroy(silverRings[i]);
                }
            }
        }

        for (int i = 0; i < goldRings.Length; i++)
        {
            if (goldRings[i] != null)
            {
                if (goldRings[i].transform.position.z < player.transform.position.z)
                {
                    goldRings[i].GetComponent<BecomeTransparent>().switchTransparent(true);
                }
                else
                {
                    goldRings[i].GetComponent<BecomeTransparent>().switchTransparent(false);
                }

                if (goldRings[i].transform.position.z + distanceBehindPlayerToRemove < player.transform.position.z)
                {
                    Destroy(goldRings[i]);
                }
            }
        }

        for (int i = 0; i < laserPickups.Length; i++)
        {
            if (laserPickups[i] != null)
            {
                //Switch to blue lasers if have twin laser already
                if(greenLasersActive)
                {
                    if(player.GetComponent<PlayerControllerScript>().getCurrentLaser() > 0)
                    {
                        Vector3 pos = laserPickups[i].transform.position;
                        Quaternion rot = laserPickups[i].transform.rotation;
                        Destroy(laserPickups[i]);
                        laserPickups[i] = Instantiate(blueLasers, pos, rot);
                    }
                }

                if (laserPickups[i].transform.position.z < player.transform.position.z)
                {
                    laserPickups[i].GetComponent<BecomeTransparent>().switchTransparent(true);
                }
                else
                {
                    laserPickups[i].GetComponent<BecomeTransparent>().switchTransparent(false);
                }

                if (laserPickups[i].transform.position.z + distanceBehindPlayerToRemove < player.transform.position.z)
                {
                    Destroy(laserPickups[i]);
                }
            }
        }
        //If all the green lasers were swapped, disalbe it to prevent further changes
        if (greenLasersActive)
        {
            if (player.GetComponent<PlayerControllerScript>().getCurrentLaser() > 0)
            {
                greenLasersActive = false;
            }
        }


        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                if (enemies[i].transform.position.z < player.transform.position.z)
                {
                    // Destroy(enemies[i]);
                    // enemies[i].GetComponent<DamagableByPlayer>().rend.material.shader = Shader.Find("Transparent/Diffuse");
                    enemies[i].GetComponent<BecomeTransparent>().switchTransparent(true);
                }

                if (enemies[i].transform.position.z + distanceBehindPlayerToRemove < player.transform.position.z)
                {
                    Destroy(enemies[i]);
                }
                //else
                //{
                //    enemies[i].GetComponent<BecomeTransparent>().switchTransparent(false);
                //}
            }
        }
    }
}
