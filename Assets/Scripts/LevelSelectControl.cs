using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class LevelSelectControl : MonoBehaviour {

    public GameObject storedInfoToPassIntoLevel;

    public string briefingRoomSceneName;

    public Image cursor;
    public Text totalHiScore;

    public Image wholeMenu;
    public int levelsPerPage;
    public float pageSize;
    private float startPos;
    private float endPos;
    private bool changingPage;
    private float timePageChangeBegan;
    public float durationOfPageChange;

    public int numberOfLevels;
    public string[] sceneNames;
    public Image[] levelLockedImages;
    public Text[] highScoreText;
    public Vector3[] cursorPositions;

    private int[] highscores;

    private int currentPosition;

    private int lastLevelUnlocked;

    private GameObject stored;

    public AudioSource menuMove;
    public AudioSource menuSelect;
    public AudioSource menuError;
    public AudioSource menuCancel;

    // Use this for initialization
    void Start ()
    {
        timePageChangeBegan = Time.time - durationOfPageChange;

        currentPosition = 0;
        cursor.rectTransform.localPosition = cursorPositions[currentPosition];

        highscores = new int[numberOfLevels];

        //Check if save file exists
        if (!File.Exists("Assets/Resources/Save.txt"))
        {
            lastLevelUnlocked = 0;

            //Read and set the highscores for each level
            for (int i = 0; i < numberOfLevels; i++)
            {
                highscores[i] = 0;
            }
        }
        else
        {
            //Load save file
            loadSaveFile();
        }

        //If no stored info exists create one
        stored = GameObject.FindWithTag("StoredInfo");
        if (stored == null)
        {
            stored = Instantiate(storedInfoToPassIntoLevel);
        }

        //Check if returning from level. 
        if(stored.GetComponent<InfoToTakeInOutOfLevel>().getCameFromlevel())
        {
            //If return form level, update scores/levels unlocked and save the new info
            if(stored.GetComponent<InfoToTakeInOutOfLevel>().getStoredHits() > highscores[stored.GetComponent<InfoToTakeInOutOfLevel>().getLevelId()])
            {
                highscores[stored.GetComponent<InfoToTakeInOutOfLevel>().getLevelId()] = stored.GetComponent<InfoToTakeInOutOfLevel>().getStoredHits();
            }

            if(lastLevelUnlocked < numberOfLevels && stored.GetComponent<InfoToTakeInOutOfLevel>().getLevelId() == lastLevelUnlocked)
            {
                lastLevelUnlocked++;
            }

            currentPosition = stored.GetComponent<InfoToTakeInOutOfLevel>().getLevelId();
            cursor.rectTransform.localPosition = cursorPositions[currentPosition];

            saveGame();
        }

        //Update visuals
        updateVisualsBasedOnReadData();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Progress the page change
        if(changingPage)
        {
            wholeMenu.rectTransform.localPosition = new Vector3(0, Mathf.Lerp(startPos, endPos, (Time.time - timePageChangeBegan) / durationOfPageChange), 0);
            
            if((Time.time - timePageChangeBegan) > durationOfPageChange)
            {
                changingPage = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentPosition > 0)
            {
                currentPosition--;
                cursor.rectTransform.localPosition = cursorPositions[currentPosition];

                menuMove.time = 0.9f;
                menuMove.Play();

                //Check on page change
                if(currentPosition % levelsPerPage == (levelsPerPage - 1))
                {
                    startPos = wholeMenu.rectTransform.localPosition.y;
                    endPos = pageSize * (currentPosition / levelsPerPage);
                    timePageChangeBegan = Time.time;
                    changingPage = true;
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            if (currentPosition < numberOfLevels - 1)
            {
                currentPosition++;
                cursor.rectTransform.localPosition = cursorPositions[currentPosition];

                menuMove.time = 0.9f;
                menuMove.Play();


                //Check on page change
                if (currentPosition % levelsPerPage == (0))
                {
                    startPos = wholeMenu.rectTransform.localPosition.y;
                    endPos = pageSize * (currentPosition / levelsPerPage);
                    timePageChangeBegan = Time.time;
                    changingPage = true;
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            //Set the level id to the info going into level
            if (currentPosition <= lastLevelUnlocked)
            {
                menuSelect.time = 0.9f;
                menuSelect.Play();

                GameObject.FindWithTag("StoredInfo").GetComponent<InfoToTakeInOutOfLevel>().reset();
                GameObject.FindWithTag("StoredInfo").GetComponent<InfoToTakeInOutOfLevel>().setLevelId(currentPosition);
                GameObject.FindWithTag("StoredInfo").GetComponent<InfoToTakeInOutOfLevel>().setSceneNameOfLevel(sceneNames[currentPosition]);
                
                SceneManager.LoadScene(briefingRoomSceneName, LoadSceneMode.Single);
            }
            else
            {
                menuError.time = 0.9f;
                menuError.Play();
            }
        }
    }

    private void updateVisualsBasedOnReadData()
    {
        int total = 0;

        for (int i = 0; i < numberOfLevels; i++)
        {
            //Unlock/lock the boxes accordingly
            if (i <= lastLevelUnlocked)
            {
                levelLockedImages[i].enabled = false;
            }
            else
            {
                levelLockedImages[i].enabled = true;
            }

            //Update the highscore text
            if (highscores[i] >= 100)
            {
                highScoreText[i].text = highscores[i].ToString();
            }
            else if (highscores[i] >= 10)
            {
                highScoreText[i].text = "0" + highscores[i].ToString();
            }
            else
            {
                highScoreText[i].text = "00" + highscores[i].ToString();
            }

            total += highscores[i];
        }

        //Update the total highscore text
        if (total >= 1000)
        {
            totalHiScore.text = "Total Hits: " + total.ToString();
        }
        else if (total >= 100)
        {
            totalHiScore.text = "Total Hits: " + "0" + total.ToString();
        }
        else if(total >= 10)
        {
            totalHiScore.text = "Total Hits: " + "00" + total.ToString();
        }
        else
        {
            totalHiScore.text = "Total Hits: " + "000" + total.ToString();
        }
    }

    private void loadSaveFile()
    {
        string assetText;

        using (var streamReader = new StreamReader("Assets/Resources/Save.txt", Encoding.UTF8))
        {
            assetText = streamReader.ReadToEnd();
        }

        string[] fLines = assetText.Split(';');

        lastLevelUnlocked = Int32.Parse(fLines[0]);
        
        //Read and set the highscores for each level
        for(int i = 1; i < fLines.Length; i++)
        {
            highscores[i - 1] = Int32.Parse(fLines[i]);
        }
    }

    private void saveGame()
    {
        string saveProgress = lastLevelUnlocked.ToString() + ";";

        for(int i = 0; i < numberOfLevels; i++)
        {
            saveProgress += highscores[i].ToString();

            if(i + 1 < numberOfLevels)
            {
                saveProgress += ";";
            }
        }

        System.IO.File.WriteAllText("Assets/Resources/Save.txt", saveProgress.ToString());
    }
}
