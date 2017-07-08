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

	// Use this for initialization
	void Start ()
    {
        hits = 0;
	}
    
    //Updates the UI in case of a change
    public void updateUI(int numBombs, float currentHealthPercentage, float currentBoostPercentage)
    {
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
