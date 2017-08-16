using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BriefingControllerScript : MonoBehaviour {

    private GameObject stored;

    public Image background;
    public Image blackScreen;
    public Text planetName;
    public Text characterName;
    public Text dialog;

    public string[] allPlanetNames;
    public TextAsset[] briefingDialog;
    public GameObject[] characterPortraits;

    public float durationOfBriefingAppear;
    private float timeBriefingBegan;

    private int briefingPhase;
    
    private DialogInfo _dialogInfo;

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
	
	// Update is called once per frame
	void Update ()
    {
		if(briefingPhase == 0)
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
                    for (int i = 0; i < characterPortraits.Length; i++)
                    {
                        if (i == _dialogInfo.getCurrentCharacter())
                        {
                            _dialogInfo.characterOnScreen = i;
                            characterName.text = ((CharacterID)i).ToString();
                           
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

        if(briefingPhase == 2)
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

        _dialogInfo = new DialogInfo(fLines.Length);//[fLines.Length];

        for (int i = 0; i < fLines.Length; i++)
        {
            string valueLine = fLines[i];
            string[] values = valueLine.Split(';');//, ";"); // your splitter here

            _dialogInfo.character[i] = (int)Enum.Parse(typeof(CharacterID), values[0]);
            _dialogInfo.dialog[i] = values[1];
        }
    }
}
