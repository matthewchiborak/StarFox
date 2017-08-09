using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public string sceneName;
    public TextAsset levelInfo;

    public GameObject player;
    public UIController _UIcontroller;
    public BackgroundMusicControl _bgmusicControl;
    public CameraControlScript _cameraControl;

    //All range mode level bounds
    public Vector3 minLevelBounds;
    public Vector3 maxLevelBounds;
    
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

    //Teammates
    public GameObject[] teammates;
    private float maxTeammateHealth;
    private float currentHealthKris;
    private float currentHealthFalco;
    private float currentHealthSlip;

    public TextAsset[] shootingTeammatesDialog;
    private float lastTimeShotTeammate;
    private float shotTeammateCooldownDialog;
    public TextAsset[] retireTeammatesDialog;
    private bool FalcoRetire;
    private bool KrisRetire;
    private bool SlipRetire;

    public TextAsset[] missionCompleteText;
    public TextAsset[] repairsCompleteText;
    private int currentMissionCompleteDialog;
    private float timeBeforePlayMissionCompleteNextDialog;
    private float timeDialogMissionCompleteStart;
    private bool playingMissionOverDialog;

    //Gameover info
    private bool playerIsDead;
    private float durationOfGameOverSeq;
    private float timeOfDeath;
    public TextAsset[] playerDiedTeammatesDialog;
    private bool fadeInTriggered;

    //Level Dialog
    public TextAsset[] levelDialog;
    public int[] zCordToTriggerDialog;
    public bool[] zCordDialogPlayed;

    //Boss
    public bool hasBoss;
    public float zCordToTriggerBoss;
    //public bool keepPlayerAndBossMoving;
    private bool isAtBoss;
    public GameObject boss;
    public Vector3 bossSpawnLocation;
    private GameObject _boss;
    private bool bossDestroyed;
    private bool updateBossHealthBar;

    public bool bossChangesToAR;
    private bool finishedTransitionTOAR;
    private bool startedTransitionTOAR;

    //Triggering boss in all range mode
    public float timeNeededToPassForBossAppearAR;
    private float timeARStarted;

    public TextAsset switchingToARDialog;

    public bool hasLevelGeo;
    public float resetToCordinate;
    public float resetFromCordinate;
    public GameObject bossPortion1;
    public GameObject bossPortion2;
    public GameObject behindBossPortion;

    

    public float timeAfterBossDestroyedToDisappear;
    private float timeBossDestroyed;
    private bool victoryPlaying;

    private bool levelWinFadeOutActivated;
    private float timeForLevelOrderFadeOutToStart;
    private float timeOfVictoryActivated;

    private bool displayingMissionComplete;
    private float timeBeforeDisplayingMissionComplete;

    private float timeBetweenMissionComplete;
    private float timeSinceActivateMissionComplete;

    private float totalTimeDisplayingMissionComplete;

    //Teammate paths
    public TextAsset[] teammatePathInstructions; //Contains the teammate, the points of the path, and the amounts of time it takes to reach each one.
    public int[] zCordToTriggerTheTeammates;
    private int nextTeammatePath;

    private Vector3[] pointsF;
    private float[] timesF;
    private Vector3[] pointsK;
    private float[] timesK;
    private Vector3[] pointsS;
    private float[] timesS;
    private bool FOnPath;
    private bool KOnPath;
    private bool SOnPath;

    // Use this for initialization
    void Start ()
    {
        greenLasersActive = true;
        distanceBehindPlayerToRemove = 50;
        //loadLevel();

        shotTeammateCooldownDialog = 10f;
        lastTimeShotTeammate = Time.time - shotTeammateCooldownDialog;

        //Team mate health
        maxTeammateHealth = 100;
        currentHealthKris = maxTeammateHealth;
        currentHealthSlip = maxTeammateHealth;
        currentHealthFalco = maxTeammateHealth;

        bossDestroyed = false;
        updateBossHealthBar = false;
        
        victoryPlaying = false;

        playerIsDead = false;
        durationOfGameOverSeq = 5;
        timeOfDeath = Time.time - durationOfGameOverSeq;

        timeForLevelOrderFadeOutToStart = 40;
        timeOfVictoryActivated = Time.time - timeForLevelOrderFadeOutToStart;

        timeBeforeDisplayingMissionComplete = 3;
        timeBetweenMissionComplete = 0.3f;
        totalTimeDisplayingMissionComplete = 15;

        currentMissionCompleteDialog = 0;
        //timeBeforePlayMissionCompleteNextDialog = 10;
        timeBeforePlayMissionCompleteNextDialog = 6;
        timeDialogMissionCompleteStart = Time.time - timeBeforePlayMissionCompleteNextDialog;
        playingMissionOverDialog = false;

        nextTeammatePath = 0;
        FOnPath = false;
        KOnPath = false;
        SOnPath = false;

        FalcoRetire = false;
        KrisRetire = false;
        SlipRetire = false;

        finishedTransitionTOAR = false;
        startedTransitionTOAR = false;

        if (player.GetComponent<PlayerControllerScript>().isInAllRange())
        {
            timeARStarted = Time.time;
        }

        _UIcontroller.activateFadeOut();
    }

    public void switchToAllRange()
    {
        player.GetComponent<PlayerControllerScript>().switchToAllRange();
        timeARStarted = Time.time;
    }

    public float getTeammateHealthPercentage(int teamMateID)
    {
        if(teamMateID == (int)CharacterID.Falco)
        {
            return currentHealthFalco / maxTeammateHealth;
        }
        if (teamMateID == (int)CharacterID.Krystal)
        {
            return currentHealthKris / maxTeammateHealth;
        }
        if (teamMateID == (int)CharacterID.Slippy)
        {
            return currentHealthSlip / maxTeammateHealth;
        }

        return 0;
    }

    public void allowHealthBarUpdates()
    {
        updateBossHealthBar = true;
    }

    public void playATeammateGameOverDialog()
    {
        _UIcontroller.loadDialog(playerDiedTeammatesDialog[Random.Range(0, playerDiedTeammatesDialog.Length)]);
    }

    public float damageTeammate(int teamMateID, float damage, bool friendlyFire)
    {
        if(playerIsDead)
        {
            return 0;
        }

        if (teamMateID == (int)CharacterID.Falco)
        {
            currentHealthFalco -= damage;
            if (friendlyFire)
            {
                if (Time.time - lastTimeShotTeammate > shotTeammateCooldownDialog)
                {
                    _UIcontroller.loadDialog(shootingTeammatesDialog[0]);
                    lastTimeShotTeammate = Time.time;
                }
            }
            
            if(currentHealthFalco <= 0)
            {
                currentHealthFalco = 0;
                //TODO Retire the teammate
                if (!FalcoRetire)
                {
                    FalcoRetire = true;
                    _UIcontroller.loadDialog(retireTeammatesDialog[0]);
                    _UIcontroller.enableRetireText();

                    if(teammates[0].GetComponent<TeammateARControlScript>())
                    {
                        if(teammates[0].GetComponent<TeammateARControlScript>().enabled)
                        {
                            teammates[0].GetComponent<TeammateARControlScript>().switchModes(TeammateARControlMode.retired);
                        }
                    }
                }
            }
        }
        if (teamMateID == (int)CharacterID.Krystal)
        {
            currentHealthKris -= damage;
            if (friendlyFire)
            {
                if (Time.time - lastTimeShotTeammate > shotTeammateCooldownDialog)
                {
                    _UIcontroller.loadDialog(shootingTeammatesDialog[1]);
                    lastTimeShotTeammate = Time.time;
                }
            }

            if (currentHealthKris <= 0)
            {
                currentHealthKris = 0;
                //TODO Retire the teammate
                if (!KrisRetire)
                {
                    KrisRetire = true;
                    _UIcontroller.loadDialog(retireTeammatesDialog[1]);
                    _UIcontroller.enableRetireText();

                    if (teammates[1].GetComponent<TeammateARControlScript>())
                    {
                        if (teammates[1].GetComponent<TeammateARControlScript>().enabled)
                        {
                            teammates[1].GetComponent<TeammateARControlScript>().switchModes(TeammateARControlMode.retired);
                        }
                    }
                }
            }
        }
        if (teamMateID == (int)CharacterID.Slippy)
        {
            currentHealthSlip -= damage;
            if (friendlyFire)
            {
                if (Time.time - lastTimeShotTeammate > shotTeammateCooldownDialog)
                {
                    _UIcontroller.loadDialog(shootingTeammatesDialog[2]);
                    lastTimeShotTeammate = Time.time;
                }
            }

            if (currentHealthSlip <= 0)
            {
                currentHealthSlip = 0;
                //TODO Retire the teammate
                if (!SlipRetire)
                {
                    SlipRetire = true;
                    _UIcontroller.loadDialog(retireTeammatesDialog[2]);
                    _UIcontroller.enableRetireText();

                    if (teammates[2].GetComponent<TeammateARControlScript>())
                    {
                        if (teammates[2].GetComponent<TeammateARControlScript>().enabled)
                        {
                            teammates[2].GetComponent<TeammateARControlScript>().switchModes(TeammateARControlMode.retired);
                        }
                    }
                }
            }
        }

        return 0;
    }

    // Update is called once per frame
    void Update ()
    {
        //Check if player is dead
        if (!playerIsDead && player.GetComponent<PlayerControllerScript>().getCurrentHealth() <= 0)
        {
            playerIsDead = true;
            timeOfDeath = Time.time;

            //Play gameover dialog from a teammate
            playATeammateGameOverDialog();

            _bgmusicControl.playGameOverTrack();
        }

        if(playerIsDead)
        {
            //Countdown time to reset
            if(Time.time - timeOfDeath > durationOfGameOverSeq)
            {
                if(!fadeInTriggered)
                {
                    fadeInTriggered = true;
                    _UIcontroller.activateFadeIn();
                }
                if (!_UIcontroller.checkIfFadeInFinished())
                {
                    SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
                }
            }

            //TODO allow player to immediately reset the level if push button

            return;
        }
        
        if (!player.GetComponent<PlayerControllerScript>().isInAllRange())
        {
            //Check if a teammate needs to do something
            if(nextTeammatePath < zCordToTriggerTheTeammates.Length)
            {
                if(player.transform.position.z > zCordToTriggerTheTeammates[nextTeammatePath])
                {
                    //Read the file
                    string fs = teammatePathInstructions[nextTeammatePath].text;
                    string[] fLines = fs.Split('\n');

                    //Vector3[] points = new Vector3[fLines.Length - 1];
                    //float[] times = new float[fLines.Length - 1];
                    //Line 0 has the teammate
                    
                    //Check if the teammate has enough health to appear
                    if (int.Parse(fLines[0]) == 0)
                    {
                        if(currentHealthFalco > 0)
                        {
                            pointsF = new Vector3[fLines.Length - 1];
                            timesF = new float[fLines.Length - 1];

                            for (int i = 1; i < fLines.Length; i++)
                            {
                                string valueLine = fLines[i];
                                string[] values = valueLine.Split(';');//, ";"); // your splitter here

                                pointsF[i - 1].x = float.Parse(values[0]);
                                pointsF[i - 1].y = float.Parse(values[1]);
                                pointsF[i - 1].z = float.Parse(values[2]);
                                timesF[i - 1] = float.Parse(values[3]);
                            }

                            FOnPath = true;
                            //teammates[0].SetActive(true);
                            teammates[0].GetComponent<TeammateControlScript>().giveNewPoints(pointsF, timesF);
                        }
                    }
                    else if (int.Parse(fLines[0]) == 1)
                    {
                        if (currentHealthKris > 0)
                        {
                            pointsK = new Vector3[fLines.Length - 1];
                            timesK = new float[fLines.Length - 1];

                            for (int i = 1; i < fLines.Length; i++)
                            {
                                string valueLine = fLines[i];
                                string[] values = valueLine.Split(';');//, ";"); // your splitter here

                                pointsK[i - 1].x = float.Parse(values[0]);
                                pointsK[i - 1].y = float.Parse(values[1]);
                                pointsK[i - 1].z = float.Parse(values[2]);
                                timesK[i - 1] = float.Parse(values[3]);
                            }

                            KOnPath = true;
                            //teammates[1].SetActive(true);
                            teammates[1].GetComponent<TeammateControlScript>().giveNewPoints(pointsK, timesK);
                        }
                    }
                    else if (int.Parse(fLines[0]) == 2)
                    {
                        if (currentHealthSlip > 0)
                        {
                            pointsS = new Vector3[fLines.Length - 1];
                            timesS = new float[fLines.Length - 1];

                            for (int i = 1; i < fLines.Length; i++)
                            {
                                string valueLine = fLines[i];
                                string[] values = valueLine.Split(';');//, ";"); // your splitter here

                                pointsS[i - 1].x = float.Parse(values[0]);
                                pointsS[i - 1].y = float.Parse(values[1]);
                                pointsS[i - 1].z = float.Parse(values[2]);
                                timesS[i - 1] = float.Parse(values[3]);
                            }

                            SOnPath = true;
                            //teammates[2].SetActive(true);
                            teammates[2].GetComponent<TeammateControlScript>().giveNewPoints(pointsS, timesS);
                        }
                    }

                    nextTeammatePath++;
                }
            }
            //Disable teammates if they need to be disabled
            if(FOnPath)
            {
                if (teammates[0].GetComponent<TeammateControlScript>().isFinishedPath())
                {
                    //teammates[0].SetActive(false);
                    teammates[0].transform.position = new Vector3(0, 0, -1000);
                    FOnPath = false;
                }
            }
            if (KOnPath)
            {
                if (teammates[1].GetComponent<TeammateControlScript>().isFinishedPath())
                {
                    //teammates[1].SetActive(false);
                    teammates[1].transform.position = new Vector3(0, 0, -1000);
                    KOnPath = false;
                }
            }
            if (SOnPath)
            {
                if (teammates[2].GetComponent<TeammateControlScript>().isFinishedPath())
                {
                    //teammates[2].SetActive(false);
                    teammates[2].transform.position = new Vector3(0, 0, -1000);
                    SOnPath = false;
                }
            }
            //for(int i = 0; i < 3; i++)
            //{
            //    if(teammates[i].GetComponent<TeammateControlScript>().isFinishedPath())
            //    {
            //        teammates[i].SetActive(false);
            //        //teammates[i].transform.position = new Vector3(0, 0, -1000);
            //    }
            //}

            //Check if play level dialog
            for (int i = 0; i < levelDialog.Length; i++)
            {
                if (player.transform.position.z > zCordToTriggerDialog[i] && !zCordDialogPlayed[i])
                {
                    _UIcontroller.loadDialog(levelDialog[i]);
                    zCordDialogPlayed[i] = true;
                }
            }

            checkIfNeedToRemove();
            //checkIfNeedActivate();
            checkIfNeedActiveEnemyWithPath();

            if(hasBoss && !isAtBoss && player.transform.position.z > zCordToTriggerBoss)
            {
                if(bossChangesToAR)
                {
                    switchToAllRange();

                    //isAtBoss = true;
                    //boss.SetActive(true);
                    //boss.GetComponent<BossControlScript>().resetHealth();
                    //_bgmusicControl.playBossMusic();

                    _bgmusicControl.stopMusic();
                    startedTransitionTOAR = true;
                    _cameraControl.transitionToAR(timeNeededToPassForBossAppearAR);
                    return;
                }

                isAtBoss = true;
                player.GetComponent<PlayerControllerScript>().setAtBoss(true);
                //_boss = Instantiate(boss, bossSpawnLocation, Quaternion.identity);
                boss.SetActive(true);
                boss.GetComponent<BossControlScript>().resetHealth();
                _bgmusicControl.playBossMusic();
                //Activate health bar
                //_UIcontroller.activateHealthBar();

                //Start moving the geometry
                if(hasLevelGeo)
                {
                    bossPortion1.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -player.GetComponent<PlayerControllerScript>().getDefaultForwardSpeed());
                    bossPortion2.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -player.GetComponent<PlayerControllerScript>().getDefaultForwardSpeed());
                    //behindBossPortion.SetActive(true);
                }
            }

            //Update the health UI
            if(isAtBoss)
            {
                //Set health on the UI
                if(updateBossHealthBar)
                {
                    _UIcontroller.updateBossHealth(boss.GetComponent<BossControlScript>().getCurrentHealthPercentage());
                }

                //Move the geo to give the appearance of movement
                if(hasLevelGeo)
                {
                    if(bossPortion1.transform.position.z < resetFromCordinate)
                    {
                        bossPortion1.transform.position = new Vector3(bossPortion1.transform.position.x, bossPortion1.transform.position.y, resetToCordinate);
                        behindBossPortion.SetActive(true);
                    }
                    if (bossPortion2.transform.position.z < resetFromCordinate)
                    {
                        bossPortion2.transform.position = new Vector3(bossPortion2.transform.position.x, bossPortion2.transform.position.y, resetToCordinate);
                        behindBossPortion.SetActive(true);
                    }
                }

                //Check if the boss is done
                if(!bossDestroyed && boss.GetComponent<BossControlScript>().getCurrentHealthPercentage() <= 0)
                {
                    //_UIController.increaseHits(hits);
                    _UIcontroller.increaseHits(boss.GetComponent<BossControlScript>().hits);
                    //boss.SetActive(false);
                    bossDestroyed = true;
                    timeBossDestroyed = Time.time;
                    _bgmusicControl.stopMusic();
                    //Debug.Log("Boss Destroyed");
                    player.GetComponent<PlayerControllerScript>().setPlayerControlEnable(false);

                    //Move the camera so the player is in the same place of the victory camera position
                    _cameraControl.missionCompleteMode(timeAfterBossDestroyedToDisappear);
                }
                
                if(!victoryPlaying && bossDestroyed && Time.time - timeBossDestroyed > timeAfterBossDestroyedToDisappear)
                {
                    _bgmusicControl.playVictoryTrack();
                    victoryPlaying = true;
                    timeOfVictoryActivated = Time.time;
                    //_cameraControl.missionCompleteMode();
                    player.GetComponent<PlayerControllerScript>().disableCursor();
                    _UIcontroller.setUIStatus(false);

                    //Move the teammates
                    if(currentHealthFalco > 0)
                    {
                        teammates[0].SetActive(true);
                        teammates[0].transform.position = new Vector3(player.transform.position.x - 30, player.transform.position.y, player.transform.position.z - 50);
                    }
                    else
                    {
                        teammates[0].SetActive(false);
                    }
                    if (currentHealthKris > 0)
                    {
                        teammates[1].SetActive(true);
                        teammates[1].transform.position = new Vector3(player.transform.position.x + 30, player.transform.position.y, player.transform.position.z - 50);
                    }
                    else
                    {
                        teammates[1].SetActive(false);
                    }
                    if (currentHealthSlip > 0)
                    {
                        teammates[2].SetActive(true);
                        teammates[2].transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z - 70);
                    }
                    else
                    {
                        teammates[2].SetActive(false);
                    }
                }

                //Activate the mission complete text
                if (victoryPlaying && !displayingMissionComplete && Time.time - timeOfVictoryActivated > timeBeforeDisplayingMissionComplete)
                {
                    displayingMissionComplete = true;
                    _UIcontroller.activateNextMissionComplete();
                    timeSinceActivateMissionComplete = Time.time;

                    timeDialogMissionCompleteStart = Time.time;
                }
                if(displayingMissionComplete && Time.time - timeSinceActivateMissionComplete > timeBetweenMissionComplete)
                {
                    _UIcontroller.activateNextMissionComplete();
                    timeSinceActivateMissionComplete = Time.time;
                }
                if(!playingMissionOverDialog && displayingMissionComplete && Time.time - timeOfVictoryActivated > (totalTimeDisplayingMissionComplete + timeBetweenMissionComplete))
                {
                    _UIcontroller.turnOffMissionComplete();
                    playingMissionOverDialog = true;
                    //timeDialogMissionCompleteStart = Time.time;
                }

                //End of mission Dialog
                if(playingMissionOverDialog && currentMissionCompleteDialog < 4 && Time.time - timeDialogMissionCompleteStart > timeBeforePlayMissionCompleteNextDialog)
                {
                    // _UIcontroller
                    switch(currentMissionCompleteDialog)
                    {
                        case 0:
                            _UIcontroller.loadDialog(missionCompleteText[0]);
                            break;
                        case 1:
                            if(currentHealthFalco > 0)
                            {
                                _UIcontroller.loadDialog(missionCompleteText[1]);
                            }
                            else
                            {
                                _UIcontroller.loadDialog(repairsCompleteText[0]);
                            }
                            break;
                        case 2:
                            if (currentHealthKris > 0)
                            {
                                _UIcontroller.loadDialog(missionCompleteText[2]);
                            }
                            else
                            {
                                _UIcontroller.loadDialog(repairsCompleteText[1]);
                            }
                            break;
                        case 3:
                            if (currentHealthSlip > 0)
                            {
                                _UIcontroller.loadDialog(missionCompleteText[3]);
                            }
                            else
                            {
                                _UIcontroller.loadDialog(repairsCompleteText[2]);
                            }
                            break;
                    }

                    timeDialogMissionCompleteStart = Time.time;
                    currentMissionCompleteDialog++;
                }

                if (victoryPlaying && !levelWinFadeOutActivated && Time.time - timeOfVictoryActivated > timeForLevelOrderFadeOutToStart)
                {
                    levelWinFadeOutActivated = true;
                    _UIcontroller.activateFadeIn();
                }

                if(levelWinFadeOutActivated)
                {
                    if(!_UIcontroller.checkIfFadeInFinished())
                    {
                        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
                    }
                }
            }
        }
        else
        {
            //Keep the player in the game area
            if(player.GetComponent<PlayerControllerScript>().getPlayerControlEnabled() && (player.transform.position.x < minLevelBounds.x || player.transform.position.x > maxLevelBounds.x ||
                player.transform.position.z < minLevelBounds.z || player.transform.position.z > maxLevelBounds.z))
            {
                player.GetComponent<PlayerControllerScript>().movePlayerBackIntoBounds();
                player.GetComponent<PlayerControllerScript>().setPlayerControlEnable(false);
            }

            if(!player.GetComponent<PlayerControllerScript>().getPlayerControlEnabled() &&
                (player.transform.position.x > minLevelBounds.x && player.transform.position.x < maxLevelBounds.x &&
                player.transform.position.z > minLevelBounds.z && player.transform.position.z < maxLevelBounds.z))
            {
                if(!bossDestroyed && finishedTransitionTOAR)
                player.GetComponent<PlayerControllerScript>().setPlayerControlEnable(true);
            }
            else if(!player.GetComponent<PlayerControllerScript>().getIsUturning() && !player.GetComponent<PlayerControllerScript>().getPlayerControlEnabled() &&
                ((player.transform.position.x < minLevelBounds.x && player.GetComponent<Rigidbody>().velocity.x < 0) ||
                (player.transform.position.x > maxLevelBounds.x && player.GetComponent<Rigidbody>().velocity.x > 0) ||
                (player.transform.position.z < minLevelBounds.z && player.GetComponent<Rigidbody>().velocity.z < 0) ||
                (player.transform.position.z > maxLevelBounds.z && player.GetComponent<Rigidbody>().velocity.z > 0)))
            {
                player.GetComponent<PlayerControllerScript>().movePlayerBackIntoBounds();
            }

            if(player.transform.position.y < minLevelBounds.y)
            {
                player.transform.position = new Vector3(player.transform.position.x, minLevelBounds.y, player.transform.position.z);
            }
            if(player.transform.position.y > maxLevelBounds.y)
            {
                player.transform.position = new Vector3(player.transform.position.x, maxLevelBounds.y, player.transform.position.z);
            }
            /////End of keep player in game area

            ////Transition to allrange
            //if(bossChangesToAR && !finishedTransitionTOAR && (Time.time - timeARStarted) < timeNeededToPassForBossAppearAR)
            //{
            //    player.GetComponent<PlayerControllerScript>().setPlayerControlEnable(false);
            //}
            //else
            //{
            //    finishedTransitionTOAR = true;
            //}

            if(startedTransitionTOAR)
            {
                startedTransitionTOAR = false;
                player.GetComponent<PlayerControllerScript>().setPlayerControlEnable(false);
                _UIcontroller.loadDialog(switchingToARDialog);
            }

            //Trigger boss TODO
            if (hasBoss && !isAtBoss && (Time.time - timeARStarted) > timeNeededToPassForBossAppearAR)
            {
                finishedTransitionTOAR = true;
                isAtBoss = true;
                boss.SetActive(true);
                boss.GetComponent<BossControlScript>().resetHealth();
                _bgmusicControl.playBossMusic();
            }

            //Update the health UI
            if (isAtBoss)
            {
                //Set health on the UI
                if (updateBossHealthBar)
                {
                    _UIcontroller.updateBossHealth(boss.GetComponent<BossControlScript>().getCurrentHealthPercentage());
                }
                
                //Check if the boss is done
                if (!bossDestroyed && boss.GetComponent<BossControlScript>().getCurrentHealthPercentage() <= 0)
                {
                    _UIcontroller.increaseHits(boss.GetComponent<BossControlScript>().hits);
                    bossDestroyed = true;
                    timeBossDestroyed = Time.time;
                    _bgmusicControl.stopMusic();
                    player.GetComponent<PlayerControllerScript>().setPlayerControlEnable(false);

                    //Move the camera so the player is in the same place of the victory camera position
                    _cameraControl.missionCompleteMode(timeAfterBossDestroyedToDisappear);
                }

                if (!victoryPlaying && bossDestroyed && Time.time - timeBossDestroyed > timeAfterBossDestroyedToDisappear)
                {
                    _bgmusicControl.playVictoryTrack();
                    victoryPlaying = true;
                    timeOfVictoryActivated = Time.time;
                    player.GetComponent<PlayerControllerScript>().disableCursor();
                    _UIcontroller.setUIStatus(false);

                    //Remove the teammates
                    //teammates[0].SetActive(false);
                    //teammates[1].SetActive(false);
                    //teammates[2].SetActive(false);
                }

                //Activate the mission complete text
                if (victoryPlaying && !displayingMissionComplete && Time.time - timeOfVictoryActivated > timeBeforeDisplayingMissionComplete)
                {
                    displayingMissionComplete = true;
                    _UIcontroller.activateNextMissionComplete();
                    timeSinceActivateMissionComplete = Time.time;

                    timeDialogMissionCompleteStart = Time.time;
                }
                if (displayingMissionComplete && Time.time - timeSinceActivateMissionComplete > timeBetweenMissionComplete)
                {
                    _UIcontroller.activateNextMissionComplete();
                    timeSinceActivateMissionComplete = Time.time;
                }
                if (!playingMissionOverDialog && displayingMissionComplete && Time.time - timeOfVictoryActivated > (totalTimeDisplayingMissionComplete + timeBetweenMissionComplete))
                {
                    _UIcontroller.turnOffMissionComplete();
                    playingMissionOverDialog = true;
                }

                //End of mission Dialog
                if (playingMissionOverDialog && currentMissionCompleteDialog < 4 && Time.time - timeDialogMissionCompleteStart > timeBeforePlayMissionCompleteNextDialog)
                {
                    // _UIcontroller
                    switch (currentMissionCompleteDialog)
                    {
                        case 0:
                            _UIcontroller.loadDialog(missionCompleteText[0]);
                            break;
                        case 1:
                            if (currentHealthFalco > 0)
                            {
                                _UIcontroller.loadDialog(missionCompleteText[1]);
                            }
                            else
                            {
                                _UIcontroller.loadDialog(repairsCompleteText[0]);
                            }
                            break;
                        case 2:
                            if (currentHealthKris > 0)
                            {
                                _UIcontroller.loadDialog(missionCompleteText[2]);
                            }
                            else
                            {
                                _UIcontroller.loadDialog(repairsCompleteText[1]);
                            }
                            break;
                        case 3:
                            if (currentHealthSlip > 0)
                            {
                                _UIcontroller.loadDialog(missionCompleteText[3]);
                            }
                            else
                            {
                                _UIcontroller.loadDialog(repairsCompleteText[2]);
                            }
                            break;
                    }

                    timeDialogMissionCompleteStart = Time.time;
                    currentMissionCompleteDialog++;
                }

                if (victoryPlaying && !levelWinFadeOutActivated && Time.time - timeOfVictoryActivated > timeForLevelOrderFadeOutToStart)
                {
                    levelWinFadeOutActivated = true;
                    _UIcontroller.activateFadeIn();
                }

                if (levelWinFadeOutActivated)
                {
                    if (!_UIcontroller.checkIfFadeInFinished())
                    {
                        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
                    }
                }
            }
        }
    }

    //public void playerHasDied()
    //{
        
    //}

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
    
    public bool checkIfBossIsDestroyed()
    {
        return bossDestroyed;
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

    //public float getTransitionTime()
    //{
    //    return timeNeededToPassForBossAppearAR;
    //}

}
