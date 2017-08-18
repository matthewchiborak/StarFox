using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BriefingDialogInfo
{
    public int[] character;
    public int[] charOnLeft;
    public int[] charOnRight;
    public string[] dialog;
    public int currentPosition;
    public int characterOnScreen;

    public bool isDone()
    {
        if (currentPosition >= dialog.Length)
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
    public int getCurrentCharOnLeft()
    {
        return charOnLeft[currentPosition];
    }
    public int getCurrentCharOnRight()
    {
        return charOnRight[currentPosition];
    }

    public BriefingDialogInfo(int size)
    {
        character = new int[size];
        charOnLeft = new int[size];
        charOnRight = new int[size];
        dialog = new string[size];
        currentPosition = 0;
        characterOnScreen = 0;
    }
}

public class BriefingControllerScript : MonoBehaviour {

    private GameObject stored;

    public Image background;
    public Image blackScreen;
    public Text planetName;
    public Text characterName;
    public Text dialog;

    public string[] allPlanetNames;
    public TextAsset[] briefingDialog;
    public GameObject[] charactersLeft;
    public GameObject[] charactersRight;

    public Image leftCover;
    public Image rightCover;

    private bool fadingInCoverL;
    private bool fadingOutCoverL;
    private bool fadingInCoverR;
    private bool fadingOutCoverR;
    private float timeFadeBegan;
    public float durationOfCharacterFade;

    public float durationOfBriefingAppear;
    private float timeBriefingBegan;

    private int briefingPhase;
    
    private BriefingDialogInfo _dialogInfo;

    private float timeDialogPopup;
    public float durationOfDialogOnScreen;
    

    // Use this for initialization
    void Start ()
    {
        characterName.text = "";
        dialog.text = "";

        briefingPhase = 0;
        timeBriefingBegan = Time.time;

        stored = GameObject.FindWithTag("StoredInfo");
        planetName.text = allPlanetNames[GameObject.FindWithTag("StoredInfo").GetComponent<InfoToTakeInOutOfLevel>().getLevelId()];
    }

    private void oldDialogSystem()
    {
        if (briefingPhase == 0)
        {
            //background.rectTransform.localScale = new Vector3((Time.time - timeBriefingBegan) / durationOfBriefingAppear, (Time.time - timeBriefingBegan) / durationOfBriefingAppear, 0);
            //if((Time.time - timeBriefingBegan) > durationOfBriefingAppear)
            //{
            //    briefingPhase++;
            //}
            blackScreen.color = new Vector4(0, 0, 0, Mathf.Lerp(1, 0, (Time.time - timeBriefingBegan) / durationOfBriefingAppear));
            if ((Time.time - timeBriefingBegan) > durationOfBriefingAppear)
            {
                //Load the dialog to be read
                loadDialog(briefingDialog[GameObject.FindWithTag("StoredInfo").GetComponent<InfoToTakeInOutOfLevel>().getLevelId()]);

                briefingPhase++;
            }
        }

        if (briefingPhase == 1)
        {
            if (Time.time - timeDialogPopup > durationOfDialogOnScreen)
            {
                timeDialogPopup = Time.time;

                if (!_dialogInfo.isDone())
                {
                    for (int i = 0; i < charactersLeft.Length; i++)
                    {
                        if (i == _dialogInfo.getCurrentCharacter())
                        {
                            _dialogInfo.characterOnScreen = i;
                            characterName.text = ((CharacterID)i).ToString();
                        }

                        //Change the character models
                        if (i == _dialogInfo.getCurrentCharOnLeft())
                        {
                            charactersLeft[i].SetActive(true);
                        }
                        else
                        {
                            charactersLeft[i].SetActive(false);
                        }
                        if (i == _dialogInfo.getCurrentCharOnRight())
                        {
                            charactersRight[i].SetActive(true);
                        }
                        else
                        {
                            charactersRight[i].SetActive(false);
                        }
                    }

                    dialog.text = _dialogInfo.getCurrentDialog();

                    _dialogInfo.currentPosition++;
                }
                else
                {
                    //All dialog exhaused
                    timeBriefingBegan = Time.time;
                    briefingPhase++;
                    characterName.text = "";
                    dialog.text = "";
                }
            }
        }

        if (briefingPhase == 2)
        {
            blackScreen.color = new Vector4(0, 0, 0, Mathf.Lerp(0, 1, (Time.time - timeBriefingBegan) / durationOfBriefingAppear));
            if ((Time.time - timeBriefingBegan) > durationOfBriefingAppear)
            {
                //Load level
                SceneManager.LoadScene(GameObject.FindWithTag("StoredInfo").GetComponent<InfoToTakeInOutOfLevel>().getSceneNameOfLevel(), LoadSceneMode.Single);
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Load the dialog
		if(briefingPhase == 0)
        {
            blackScreen.color = new Vector4(0, 0, 0, Mathf.Lerp(1, 0, (Time.time - timeBriefingBegan) / durationOfBriefingAppear));
            if ((Time.time - timeBriefingBegan) > durationOfBriefingAppear)
            {
                //Load the dialog to be read
                loadDialog(briefingDialog[GameObject.FindWithTag("StoredInfo").GetComponent<InfoToTakeInOutOfLevel>().getLevelId()]);

                fadingInCoverL = true;
                fadingInCoverR = true;

                briefingPhase = 3;
            }
        }
        
        //Check if time to switch dialog
        if (briefingPhase == 1)
        {
            if (Time.time - timeDialogPopup > durationOfDialogOnScreen)
            {
                if (_dialogInfo.isDone())
                {
                    //All dialog exhaused
                    timeBriefingBegan = Time.time;
                    briefingPhase = 6;
                    characterName.text = "";
                    dialog.text = "";
                }
                else
                {
                    //Set all the neccessary variables
                    if(_dialogInfo.getCurrentCharOnLeft() >= 0)
                    {
                        //Change is required
                        fadingOutCoverL = true;
                        fadingInCoverL = true;
                    }
                    if (_dialogInfo.getCurrentCharOnRight() >= 0)
                    {
                        //Change is required
                        fadingInCoverR = true;
                        fadingOutCoverR = true;
                    }
                    
                    timeFadeBegan = Time.time;
                    briefingPhase = 2;
                }
            }
        }

        if(briefingPhase == 2)
        {
            if (fadingOutCoverL || fadingInCoverR)
            {
                if (fadingOutCoverL)
                {
                    leftCover.color = new Vector4(1, 1, 1, Mathf.Lerp(0, 1, (Time.time - timeFadeBegan) / durationOfCharacterFade));
                }
                if (fadingOutCoverR)
                {
                    rightCover.color = new Vector4(1, 1, 1, Mathf.Lerp(0, 1, (Time.time - timeFadeBegan) / durationOfCharacterFade));
                }

                if ((Time.time - timeFadeBegan) > durationOfCharacterFade)
                {
                    fadingOutCoverL = false;
                    fadingOutCoverR = false;
                    briefingPhase = 3;
                }
            }
            else
            {
                briefingPhase = 3;
            }
        }

        //Change the models
        if(briefingPhase == 3)
        {
            if (_dialogInfo.getCurrentCharOnLeft() >= 0 || _dialogInfo.getCurrentCharOnRight() >= 0)
            {
                for (int i = 0; i < charactersLeft.Length; i++)
                {
                    //Change the character models
                    //Only change if a change is required
                    if (_dialogInfo.getCurrentCharOnLeft() >= 0)
                    {
                        if (i == _dialogInfo.getCurrentCharOnLeft())
                        {
                            charactersLeft[i].SetActive(true);
                        }
                        else
                        {
                            charactersLeft[i].SetActive(false);
                        }
                    }
                    if (_dialogInfo.getCurrentCharOnRight() >= 0)
                    {
                        if (i == _dialogInfo.getCurrentCharOnRight())
                        {
                            charactersRight[i].SetActive(true);
                        }
                        else
                        {
                            charactersRight[i].SetActive(false);
                        }
                    }
                }
            }

            timeFadeBegan = Time.time;
            briefingPhase = 4;
        }

        //Fade in the characters if needed
        if(briefingPhase == 4)
        {
            if(fadingInCoverL || fadingInCoverR)
            {
                if(fadingInCoverL)
                {
                    leftCover.color = new Vector4(1, 1, 1, Mathf.Lerp(1, 0, (Time.time - timeFadeBegan) / durationOfCharacterFade));
                }
                if(fadingInCoverR)
                {
                    rightCover.color = new Vector4(1, 1, 1, Mathf.Lerp(1, 0, (Time.time - timeFadeBegan) / durationOfCharacterFade));
                }

                if((Time.time - timeFadeBegan) > durationOfCharacterFade)
                {
                    fadingInCoverL = false;
                    fadingInCoverR = false;
                    briefingPhase = 5;
                }
            }
            else
            {
                briefingPhase = 5;
            }
        }

        //Change dialog and name
        if (briefingPhase == 5)
        {
            for (int i = 0; i < charactersLeft.Length; i++)
            {
                if (i == _dialogInfo.getCurrentCharacter())
                {
                    _dialogInfo.characterOnScreen = i;
                    characterName.text = ((CharacterID)i).ToString();
                }
            }

            dialog.text = _dialogInfo.getCurrentDialog();

            _dialogInfo.currentPosition++;

            timeDialogPopup = Time.time;

            briefingPhase = 1;
        }

        //End mission briefing
        if(briefingPhase == 6)
        {
            blackScreen.color = new Vector4(0, 0, 0, Mathf.Lerp(0, 1, (Time.time - timeBriefingBegan) / durationOfBriefingAppear));
            if ((Time.time - timeBriefingBegan) > durationOfBriefingAppear)
            {
                //Load level
                SceneManager.LoadScene(GameObject.FindWithTag("StoredInfo").GetComponent<InfoToTakeInOutOfLevel>().getSceneNameOfLevel(), LoadSceneMode.Single);
            }
        }
    }

    private void loadDialog(TextAsset textAsset)
    {
        timeDialogPopup = Time.time - durationOfDialogOnScreen;

        string fs = textAsset.text;
        string[] fLines = fs.Split('\n');

        _dialogInfo = new BriefingDialogInfo(fLines.Length);

        for (int i = 0; i < fLines.Length; i++)
        {
            string valueLine = fLines[i];
            string[] values = valueLine.Split(';');

            _dialogInfo.character[i] = (int)Enum.Parse(typeof(CharacterID), values[0]);

            //Only record changes in character
            if(values[1] == "N")
            {
                _dialogInfo.charOnLeft[i] = -1;
            }
            else
            {
                _dialogInfo.charOnLeft[i] = (int)Enum.Parse(typeof(CharacterID), values[1]);
            }
            if(values[2] == "N")
            {
                _dialogInfo.charOnRight[i] = -1;
            }
            else
            {
                _dialogInfo.charOnRight[i] = (int)Enum.Parse(typeof(CharacterID), values[2]);
            }

            _dialogInfo.dialog[i] = values[3];
        }
    }
}
