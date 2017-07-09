using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BecomeTransparent : MonoBehaviour {

    public Renderer[] rend;
    public Material[] defaultMaterial;
    public Material[] transparentMaterial;


    public void switchTransparent(bool becomeTransparent)
    {
        if (becomeTransparent)
        {
            for(int i = 0; i < rend.Length; i++)
            {
                rend[i].material = transparentMaterial[i];
            }
            
        }
        else
        {
            for (int i = 0; i < rend.Length; i++)
                rend[i].material = defaultMaterial[i];
        }
    }

    // Use this for initialization
    //   void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}
}
