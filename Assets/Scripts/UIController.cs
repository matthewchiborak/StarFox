using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterID
{
    Fox,
    Falco,
    Krystal,
    Slippy,
    Peppy,
    ROB
}

public class DialogInfo
{
    public int[] character;
    public string[] dialog;
    public int currentPosition;
    public int characterOnScreen;

    public bool isDone()
    {
        if(currentPosition >= dialog.Length)
        {
            return true;
        }

        return false;
    }

    public int getCurrentCharacter()
    {
        return character[currentPosition];
    }
    public string getCurrentDialog()
    {
        return dialog[currentPosition];
    }

    public DialogInfo(int size)
    {
        character = new int[size];
        dialog = new string[size];
        currentPosition = 0;
        characterOnScreen = 0;
    }
}

public class UIController : MonoBehaviour {

    public GameManagerScript gameManager;

    private int hits;

    public Image blackScreen;
    private bool fadeIn;
    private bool fadeOut;
    private float timeForScreenToFade;
    private float timeScreenFadeBegin;

    public Text hitCountUI;
    public Image[] bombs;
    public Text bombMultiplyerText;
    public Text bombCountText;
    public Image healthBarImage;
    public Image boostBarImage;

    //private float increaseLifeBarMult;

    //For health bar streaching
    private bool hasLongLifeBar;
    private float timeOfStartIncreaseBar;
    private float duractionOfIncreasingBar;

    public Image healthBarBack;
    public Image healthBarWhiteTop;
    public Image healthBarWhiteBot;
    public Image healthBarWhiteRight;
    public Image healthBarBlackTop;
    public Image healthBarBlackBot;
    public Image healthBarBlackRight;

    //For gold ring UI
    public Image ring1;
    public Image ring2;
    public Image ring3;
    public AudioSource hitBarStretchSource;

    //Dialog functionality
    public GameObject dialogBox;
    //public TextAsset[] levelDialog;
    //public int[] zCordToTriggerDialog;
    //public bool[] zCordDialogPlayed;
    private DialogInfo _dialogInfo;
    private int nextDialogToBePlayed;
    private bool currentlyPlayingDialog;

    public Image[] portraits;
    public Text dialogNameText;
    public Text dialogText;
    private float timeDialogPopup;
    private float timeDialogRemainsOnScreen;

    //Blinking
    public Image[] eyes;
    private float timeOfLastBlink;
    private float timePassedBeforeNeedBlink;
    private float durationOfBlink;

    //Mouth movement
    public Image[] mouth1;
    public Image[] mouth2;
    private int currentMouth; // 0 1 2 or 3
    private float timeOfLastTalk;
    private float durationOfEachTalk;

    //HealthBar under the portrait
    public GameObject dialogHealthBar;
    public Image dialogHealthBarBar;
    public Text retireText;

    //Boss Health Bar
    public GameObject bossHealthBar;
    public Image bossHealthBarFront;
    private bool bossHealthBarStartGrowing;

    //Mission Complete
    public Image missionCompleteBox;
    public Text[] missionCompleteText;
    private int currentMissionCompleteComponent;
    public AudioSource missionCompleteThud;

    // Use this for initialization
    void Start ()
    {
        hits = 0;
        duractionOfIncreasingBar = 3;
        timeOfStartIncreaseBar = Time.time - duractionOfIncreasingBar;
        nextDialogToBePlayed = 0;
        currentlyPlayingDialog = false;
        timeDialogRemainsOnScreen = 5;
        timeDialogPopup = Time.time - timeDialogRemainsOnScreen;

        timePassedBeforeNeedBlink = 2f;
        durationOfBlink = 0.125f;
        timeOfLastBlink = Time.time - timePassedBeforeNeedBlink - durationOfBlink;

        currentMouth = 0;
        durationOfEachTalk = 0.08f;//0.125f;
        timeOfLastTalk = Time.time - durationOfEachTalk;

        bossHealthBarStartGrowing = false;
        hasLongLifeBar = false;
        
        timeForScreenToFade = 3;
        timeScreenFadeBegin = Time.time - timeForScreenToFade;

        currentMissionCompleteComponent = 0;
    }

    public void activateNextMissionComplete()
    {
        switch(currentMissionCompleteComponent)
        {
            case 0:
                missionCompleteBox.enabled = true;
                break;
            case 1:
                missionCompleteText[0].enabled = true;
                missionCompleteThud.Play();
                break;
            case 2:
                missionCompleteText[1].enabled = true;
                missionCompleteThud.Play();
                break;
            case 3:
                missionCompleteText[2].enabled = true;
                missionCompleteThud.Play();
                break;
            default:
                currentMissionCompleteComponent--;
                break;
        }

        currentMissionCompleteComponent++;
    }
    public void turnOffMissionComplete()
    {
        missionCompleteBox.enabled = false;
        missionCompleteText[0].enabled = false;
        missionCompleteText[1].enabled = false;
        missionCompleteText[2].enabled = false;
    }

    public void doubleLifeBar()
    {
        hasLongLifeBar = true;
        timeOfStartIncreaseBar = Time.time;
        hitBarStretchSource.Play();
        ring1.enabled = true;
        ring2.enabled = true;
        ring3.enabled = true;
    }

    public void activateFadeIn()
    {
        fadeIn = true;
        timeScreenFadeBegin = Time.time;
    }
    public void activateFadeOut()
    {
        fadeOut = true;
        timeScreenFadeBegin = Time.time;
    }

    public void activateHealthBar()
    {
        bossHealthBar.SetActive(true);
    }
    
    public void updateBossHealth(float healthPercent)
    {
        bossHealthBarFront.transform.localScale = new Vector3(1, healthPercent, 1);
    }

    public bool checkIfFadeInFinished()
    {
        return fadeIn;
    }
    
    //Updates the UI in case of a change
    public void updateUI(int numBombs, float currentHealthPercentage, float currentBoostPercentage, int numGoldRings, float zCord)
    {
        //Fade the black screen in or out if neccessary
        if(fadeIn)
        {
            blackScreen.color = new Vector4(0, 0, 0, Mathf.Lerp(0, 1, (Time.time - timeScreenFadeBegin) / timeForScreenToFade));
            if(Time.time - timeScreenFadeBegin > timeForScreenToFade)
            {
                fadeIn = false;
            }
        }
        if(fadeOut)
        {
            blackScreen.color = new Vector4(0, 0, 0, Mathf.Lerp(1, 0, (Time.time - timeScreenFadeBegin) / timeForScreenToFade));
            if (Time.time - timeScreenFadeBegin > timeForScreenToFade)
            {
                fadeOut = false;
            }
        }

        //Update dialog if there is one playing
        if (currentlyPlayingDialog)
        {
            //Blinking
            if (Time.time - timeOfLastBlink > timePassedBeforeNeedBlink)
            {
                if(eyes[_dialogInfo.characterOnScreen] != null)
                    eyes[_dialogInfo.characterOnScreen].enabled = true;

                if(Time.time - timeOfLastBlink > timePassedBeforeNeedBlink + durationOfBlink)
                {
                    if (eyes[_dialogInfo.characterOnScreen] != null)
                        eyes[_dialogInfo.characterOnScreen].enabled = false;
                    timeOfLastBlink = Time.time;
                }
            }

            //Talking
            if(Time.time - timeOfLastTalk > durationOfEachTalk)
            {
                currentMouth++;
                currentMouth = currentMouth % 4;

                if (currentMouth == 0)
                {
                    mouth1[_dialogInfo.characterOnScreen].enabled = false;
                    mouth2[_dialogInfo.characterOnScreen].enabled = false;
                }
                else if (currentMouth == 1)
                {
                    mouth1[_dialogInfo.characterOnScreen].enabled = true;
                    mouth2[_dialogInfo.characterOnScreen].enabled = false;
                }
                else if (currentMouth == 2)
                {
                    mouth1[_dialogInfo.characterOnScreen].enabled = false;
                    mouth2[_dialogInfo.characterOnScreen].enabled = true;
                }
                else if (currentMouth == 3)
                {
                    mouth1[_dialogInfo.characterOnScreen].enabled = true;
                    mouth2[_dialogInfo.characterOnScreen].enabled = false;
                }
                
                timeOfLastTalk = Time.time;
            }

            if (Time.time - timeDialogPopup > timeDialogRemainsOnScreen)
            {
                timeDialogPopup = Time.time;

                if (_dialogInfo.isDone())
                {
                    currentlyPlayingDialog = false;
                    dialogBox.SetActive(false);
                }
                else
                {
                    for (int i = 0; i < portraits.Length; i++)
                    {
                        if (i == _dialogInfo.getCurrentCharacter())
                        {
                            portraits[i].enabled = true;
                            _dialogInfo.characterOnScreen = i;
                            dialogNameText.text = ((CharacterID)i).ToString();
                            //disable and reset the eyes
                            if (eyes[i] != null)
                                eyes[i].enabled = false;
                            mouth1[i].enabled = false;
                            mouth2[i].enabled = false;
                            currentMouth = 0;
                            timeOfLastTalk = Time.time;
                            // timeOfLastBlink = Time.time;

                            //Health bar below portrait
                            if(i == (int)CharacterID.Falco || i == (int)CharacterID.Fox || i == (int)CharacterID.Krystal || i == (int)CharacterID.Slippy)
                            {
                                dialogHealthBar.SetActive(true);

                                if(i == (int)CharacterID.Fox)
                                {
                                    dialogHealthBarBar.transform.localScale = new Vector3(currentHealthPercentage, 1, 1);
                                }
                                else
                                {
                                    dialogHealthBarBar.transform.localScale = new Vector3(gameManager.getTeammateHealthPercentage(i), 1, 1);
                                }
                            }
                            else
                            {
                                dialogHealthBar.SetActive(false);
                            }
                        }
                        else
                        {
                            portraits[i].enabled = false;
                            //Disable the eyes
                            if (eyes[i] != null)
                            {
                                eyes[i].enabled = false;
                            }
                            mouth1[i].enabled = false;
                            mouth2[i].enabled = false;
                        }
                    }

                    dialogText.text = _dialogInfo.getCurrentDialog();
                    dialogBox.SetActive(true);

                    _dialogInfo.currentPosition++;
                }
            }
        }

        //Increase the life bar if needed
        if (hasLongLifeBar && Time.time - timeOfStartIncreaseBar < duractionOfIncreasingBar)
        {
            hitBarStretchSource.pitch = Mathf.Lerp(0.5f, 2.5f, (Time.time - timeOfStartIncreaseBar) / duractionOfIncreasingBar);

            healthBarImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(225, 450, (Time.time - timeOfStartIncreaseBar) / duractionOfIncreasingBar));
            healthBarBack.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(225, 450, (Time.time - timeOfStartIncreaseBar) / duractionOfIncreasingBar));
            healthBarWhiteTop.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(225, 450, (Time.time - timeOfStartIncreaseBar) / duractionOfIncreasingBar));
            healthBarWhiteBot.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(225, 450, (Time.time - timeOfStartIncreaseBar) / duractionOfIncreasingBar));

            //healthBarWhiteRight.rectTransform.SetPositionAndRotation(new Vector3(Mathf.Lerp(220, 445, (Time.time - timeOfStartIncreaseBar) / duractionOfIncreasingBar), 0, 0), new Quaternion(0, 0, 0, 1));
            healthBarWhiteRight.rectTransform.localPosition = new Vector3(Mathf.Lerp(220, 445, (Time.time - timeOfStartIncreaseBar) / duractionOfIncreasingBar), 0, 0);

            healthBarBlackTop.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(215, 440, (Time.time - timeOfStartIncreaseBar) / duractionOfIncreasingBar));
            healthBarBlackBot.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(215, 440, (Time.time - timeOfStartIncreaseBar) / duractionOfIncreasingBar));
            //healthBarBlackRight.rectTransform.SetPositionAndRotation(new Vector3(Mathf.Lerp(219, 444, (Time.time - timeOfStartIncreaseBar) / duractionOfIncreasingBar), 0, 0), new Quaternion(0, 0, 0, 1));
            healthBarBlackRight.rectTransform.localPosition = new Vector3(Mathf.Lerp(219, 444, (Time.time - timeOfStartIncreaseBar) / duractionOfIncreasingBar), -5, 0);
        }
        else
        {
            hitBarStretchSource.Stop();
            //Num of gold rings
            if (numGoldRings == 0)
            {
                ring1.enabled = false;
                ring2.enabled = false;
                ring3.enabled = false;
            }
            if (numGoldRings == 1)
            {
                ring1.enabled = true;
                ring2.enabled = false;
                ring3.enabled = false;
            }
            if (numGoldRings == 2)
            {
                ring1.enabled = true;
                ring2.enabled = true;
                ring3.enabled = false;
            }
            if (numGoldRings > 2)
            {
                ring1.enabled = true;
                ring2.enabled = true;
                ring3.enabled = true;
            }
        }

        //Hit count
        if(hits >= 100)
        {
            hitCountUI.text = hits.ToString();
        }
        else if(hits >= 10)
        {
            hitCountUI.text = "0" + hits.ToString();
        }
        else
        {
            hitCountUI.text = "00" + hits.ToString();
        }

        //Health bar
        healthBarImage.transform.localScale = new Vector3(currentHealthPercentage, 1, 1);

        //Boost bar
        boostBarImage.transform.localScale = new Vector3(currentBoostPercentage, 1, 1);

        //Bombs
        if (numBombs > 6)
        {
            for(int i = 1; i < 6; i++)
            {
                bombs[i].enabled = false;
            }

            bombs[0].enabled = true;
            bombMultiplyerText.enabled = true;
            bombCountText.enabled = true;
            bombCountText.text = numBombs.ToString();
        }
        else
        {
            bombMultiplyerText.enabled = false;
            bombCountText.enabled = false;

            for (int i = 0; i < 6; i++)
            {
                if(i < numBombs)
                {
                    bombs[i].enabled = true;
                }
                else
                {
                    bombs[i].enabled = false;
                }
            }
        }
       
    }

    public void increaseHits(int hitsToAdd)
    {
        hits += hitsToAdd;
    }

    //public void loadDialog(int dialogIndex)
    //{
    //    retireText.enabled = false;
    //    currentlyPlayingDialog = true;
    //    timeDialogPopup = Time.time - timeDialogRemainsOnScreen;

    //    string fs = levelDialog[dialogIndex].text;
    //    string[] fLines = fs.Split('\n');

    //    _dialogInfo = new DialogInfo(fLines.Length);//[fLines.Length];

    //    for (int i = 0; i < fLines.Length; i++)
    //    {
    //        string valueLine = fLines[i];
    //        string[] values = valueLine.Split(';');//, ";"); // your splitter here
            
    //        _dialogInfo.character[i] = (int)Enum.Parse(typeof(CharacterID), values[0]);
    //        _dialogInfo.dialog[i] = values[1];
    //    }
    //}

    //Interrupting Dialog
    public void loadDialog(TextAsset textAsset)
    {
        retireText.enabled = false;
        currentlyPlayingDialog = true;
        timeDialogPopup = Time.time - timeDialogRemainsOnScreen;

        string fs = textAsset.text;
        string[] fLines = fs.Split('\n');

        _dialogInfo = new DialogInfo(fLines.Length);//[fLines.Length];

        for (int i = 0; i < fLines.Length; i++)
        {
            string valueLine = fLines[i];
            string[] values = valueLine.Split(';');//, ";"); // your splitter here

            _dialogInfo.character[i] = (int)Enum.Parse(typeof(CharacterID), values[0]);
            _dialogInfo.dialog[i] = values[1];
        }
    }

    public void enableRetireText()
    {
        retireText.enabled = true;
    }
}
