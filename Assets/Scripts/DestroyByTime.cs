using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByTime : MonoBehaviour {

    public float lifetime;

    // Use this for initialization
    // Remove the object so the game isn't filled with them
    void Start ()
    {
        Destroy(gameObject, lifetime);
    }
}
