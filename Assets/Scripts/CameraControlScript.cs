using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlScript : MonoBehaviour {

    public FollowPlayer _followPlayer;
    public MissionCompleteCameraControl _missionComplete;
    
    public void missionCompleteMode()
    {
        _followPlayer.enabled = false;
        _missionComplete.enabled = true;
    }
}
