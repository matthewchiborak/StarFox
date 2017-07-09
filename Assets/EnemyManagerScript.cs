using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManagerScript : MonoBehaviour {

    public GameObject player;

    public GameObject[] enemies;
    
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                if (enemies[i].transform.position.z < player.transform.position.z)
                {
                    // Destroy(enemies[i]);
                    // enemies[i].GetComponent<DamagableByPlayer>().rend.material.shader = Shader.Find("Transparent/Diffuse");
                    enemies[i].GetComponent<DamagableByPlayer>().BecomeTransparent();
                }
            }
        }
	}
}
