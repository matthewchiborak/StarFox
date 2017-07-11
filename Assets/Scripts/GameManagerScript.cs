using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour {

    public GameObject player;

    public GameObject[] enemies;

    public GameObject[] bombPickups;

    private float distanceBehindPlayerToRemove;

    // Use this for initialization
    void Start ()
    {
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
