using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfo
{
    public int type;
    public int zCordReq;
    public Vector3 spawnLocation;
    public bool damagableByPlayer;
    //TODO behaviour
    public bool spawned;

    public UnitInfo()
    {
        spawned = false;
        damagableByPlayer = false;
    }
}

public class GameManagerScript : MonoBehaviour {

    public TextAsset levelInfo;

    public GameObject player;
    public UIController _UIcontroller;
    
    public GameObject[] prefabs;
    public GameObject[] actives;

    private UnitInfo[] _unitInfo;

    //public GameObject[] enemies;

    //public GameObject[] bombPickups;

    //public GameObject[] laserPickups;

    //public GameObject[] silverRings;
    //public GameObject[] goldRings;

    //Prefab references for the laser pickups to switch to the other laser pickups
    bool greenLasersActive;
    public GameObject blueLasers;

    private float distanceBehindPlayerToRemove;

    // Use this for initialization
    void Start ()
    {
        greenLasersActive = true;
        distanceBehindPlayerToRemove = 50;
        //loadLevel();
    }

    
	
	// Update is called once per frame
	void Update ()
    {
        checkIfNeedToRemove();
        //checkIfNeedActivate();
        checkIfNeedActiveEnemyWithPath();
    }

    //private void checkIfNeedActiveEnemyWithPath()
    //{
    //    for (int i = 0; i < actives.Length; i++)
    //    {
    //        if (actives[i] != null)
    //        {
    //            if (actives[i].CompareTag("Enemy"))
    //            {
    //                if (actives[i].GetComponent<FollowPath>() != null)
    //                {
    //                    if (!actives[i].GetComponent<FollowPath>().checkIfIsActive())
    //                    {
    //                        if (player.transform.position.z > actives[i].GetComponent<FollowPath>().zCordToActiveAt)
    //                        {
    //                            actives[i].GetComponent<FollowPath>().activateEnemy();
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
    private void checkIfNeedActiveEnemyWithPath()
    {
        for (int i = 0; i < actives.Length; i++)
        {
            if (actives[i] != null)
            {
                if (actives[i].CompareTag("Enemy"))
                {
                    if (actives[i].GetComponent<FollowPathVelocity>() != null)
                    {
                        if (!actives[i].GetComponent<FollowPathVelocity>().checkIfIsActive())
                        {
                            if (player.transform.position.z > actives[i].GetComponent<FollowPathVelocity>().zCordToActiveAt)
                            {
                                actives[i].GetComponent<FollowPathVelocity>().activateEnemy();
                            }
                        }
                    }
                }
            }
        }
    }

    private void checkIfNeedActivate()
    {
        for(int i = 0; i < _unitInfo.Length; i++)
        {
            if(!_unitInfo[i].spawned)
            {
                if(player.transform.position.z > _unitInfo[i].zCordReq)//_unitInfo[i].zCordReq)
                {
                    //Spawn the enemy
                    actives[i] = Instantiate(prefabs[_unitInfo[i].type], _unitInfo[i].spawnLocation, new Quaternion(0, 0, 0, 1));
                    _unitInfo[i].spawned = true;

                    if (_unitInfo[i].damagableByPlayer)
                    {
                        actives[i].GetComponent<DamagableByPlayer>()._UIController = _UIcontroller;
                    }
                }
            }
        }
    }

    private void loadLevel()
    {
        string fs = levelInfo.text;
        string[] fLines = fs.Split('\n');//Regex.Split(fs, "\n|\r|\r\n");

        _unitInfo = new UnitInfo[fLines.Length];
        actives = new GameObject[fLines.Length];

        for (int i = 0; i < fLines.Length; i++)
        {
            string valueLine = fLines[i];
            string[] values = valueLine.Split(',');//, ";"); // your splitter here
            
            _unitInfo[i] = new UnitInfo();
            _unitInfo[i].type = int.Parse(values[0]);
            _unitInfo[i].zCordReq = int.Parse(values[1]);
            _unitInfo[i].spawnLocation.x = int.Parse(values[2]);
            _unitInfo[i].spawnLocation.y = int.Parse(values[3]);
            _unitInfo[i].spawnLocation.z = int.Parse(values[4]);

            if(int.Parse(values[5]) == 1)
            {
                _unitInfo[i].damagableByPlayer = true;
            }
        }
    }

    private void checkIfNeedToRemove()
    {
        //If all the green lasers were swapped, disalbe it to prevent further changes
        if (greenLasersActive)
        {
            if (player.GetComponent<PlayerControllerScript>().getCurrentLaser() > 0)
            {
                greenLasersActive = false;
            }
        }

        for (int i = 0; i < actives.Length; i++)
        {
            if (actives[i] != null)
            {
                //Switch to blue lasers if have twin laser already
                if (actives[i].CompareTag("LaserPickup"))
                {
                    if (greenLasersActive)
                    {
                        if (player.GetComponent<PlayerControllerScript>().getCurrentLaser() > 0)
                        {
                            Vector3 pos = actives[i].transform.position;
                            Quaternion rot = actives[i].transform.rotation;
                            Destroy(actives[i]);
                            actives[i] = Instantiate(blueLasers, pos, rot);
                        }
                    }
                }

                if (actives[i].transform.position.z < player.transform.position.z)
                {
                    actives[i].GetComponent<BecomeTransparent>().switchTransparent(true);
                }
                else
                {
                    actives[i].GetComponent<BecomeTransparent>().switchTransparent(false);
                }

                if (actives[i].transform.position.z + distanceBehindPlayerToRemove < player.transform.position.z)
                {
                    Destroy(actives[i]);
                }
            }
        }

        //for (int i = 0; i < bombPickups.Length; i++)
        //{
        //    if (bombPickups[i] != null)
        //    {
        //        if (bombPickups[i].transform.position.z < player.transform.position.z)
        //        {
        //            bombPickups[i].GetComponent<BecomeTransparent>().switchTransparent(true);
        //        }
        //        else
        //        {
        //            bombPickups[i].GetComponent<BecomeTransparent>().switchTransparent(false);
        //        }

        //        if (bombPickups[i].transform.position.z + distanceBehindPlayerToRemove < player.transform.position.z)
        //        {
        //            Destroy(bombPickups[i]);
        //        }
        //    }
        //}

        //for (int i = 0; i < silverRings.Length; i++)
        //{
        //    if (silverRings[i] != null)
        //    {
        //        if (silverRings[i].transform.position.z < player.transform.position.z)
        //        {
        //            silverRings[i].GetComponent<BecomeTransparent>().switchTransparent(true);
        //        }
        //        else
        //        {
        //            silverRings[i].GetComponent<BecomeTransparent>().switchTransparent(false);
        //        }

        //        if (silverRings[i].transform.position.z + distanceBehindPlayerToRemove < player.transform.position.z)
        //        {
        //            Destroy(silverRings[i]);
        //        }
        //    }
        //}

        //for (int i = 0; i < goldRings.Length; i++)
        //{
        //    if (goldRings[i] != null)
        //    {
        //        if (goldRings[i].transform.position.z < player.transform.position.z)
        //        {
        //            goldRings[i].GetComponent<BecomeTransparent>().switchTransparent(true);
        //        }
        //        else
        //        {
        //            goldRings[i].GetComponent<BecomeTransparent>().switchTransparent(false);
        //        }

        //        if (goldRings[i].transform.position.z + distanceBehindPlayerToRemove < player.transform.position.z)
        //        {
        //            Destroy(goldRings[i]);
        //        }
        //    }
        //}

        //for (int i = 0; i < laserPickups.Length; i++)
        //{
        //    if (laserPickups[i] != null)
        //    {
        //        //Switch to blue lasers if have twin laser already
        //        if (greenLasersActive)
        //        {
        //            if (player.GetComponent<PlayerControllerScript>().getCurrentLaser() > 0)
        //            {
        //                Vector3 pos = laserPickups[i].transform.position;
        //                Quaternion rot = laserPickups[i].transform.rotation;
        //                Destroy(laserPickups[i]);
        //                laserPickups[i] = Instantiate(blueLasers, pos, rot);
        //            }
        //        }

        //        if (laserPickups[i].transform.position.z < player.transform.position.z)
        //        {
        //            laserPickups[i].GetComponent<BecomeTransparent>().switchTransparent(true);
        //        }
        //        else
        //        {
        //            laserPickups[i].GetComponent<BecomeTransparent>().switchTransparent(false);
        //        }

        //        if (laserPickups[i].transform.position.z + distanceBehindPlayerToRemove < player.transform.position.z)
        //        {
        //            Destroy(laserPickups[i]);
        //        }
        //    }
        //}

        //for (int i = 0; i < enemies.Length; i++)
        //{
        //    if (enemies[i] != null)
        //    {
        //        if (enemies[i].transform.position.z < player.transform.position.z)
        //        {
        //            // Destroy(enemies[i]);
        //            // enemies[i].GetComponent<DamagableByPlayer>().rend.material.shader = Shader.Find("Transparent/Diffuse");
        //            enemies[i].GetComponent<BecomeTransparent>().switchTransparent(true);
        //        }

        //        if (enemies[i].transform.position.z + distanceBehindPlayerToRemove < player.transform.position.z)
        //        {
        //            Destroy(enemies[i]);
        //        }
        //        //else
        //        //{
        //        //    enemies[i].GetComponent<BecomeTransparent>().switchTransparent(false);
        //        //}
        //    }
        //}
    }

}
