using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    private int hits;

    public Text hitCountUI;
    public Image[] bombs;
    public Text bombMultiplyerText;
    public Text bombCountText;
    public Image healthBarImage;
    public Image boostBarImage;

    //private float increaseLifeBarMult;

    //For health bar streaching
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

    // Use this for initialization
    void Start ()
    {
        hits = 0;
        duractionOfIncreasingBar = 3;
        timeOfStartIncreaseBar = Time.time - duractionOfIncreasingBar;
    }

    public void doubleLifeBar()
    {
        timeOfStartIncreaseBar = Time.time;
        hitBarStretchSource.Play();
        ring1.enabled = true;
        ring2.enabled = true;
        ring3.enabled = true;
    }
    
    //Updates the UI in case of a change
    public void updateUI(int numBombs, float currentHealthPercentage, float currentBoostPercentage, int numGoldRings)
    {
       
        //Increase the life bar if needed
        if (Time.time - timeOfStartIncreaseBar < duractionOfIncreasingBar)
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
}
