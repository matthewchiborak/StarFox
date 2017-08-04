using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlScript : MonoBehaviour {

    public FollowPlayer _followPlayer;
    public MissionCompleteCameraControl _missionComplete;
    
    public void missionCompleteMode(float time)
    {
        _followPlayer.enabled = false;
        _missionComplete.enabled = true;

        if(_followPlayer.checkIfStartedLoop())
        {
            _missionComplete.setCustomCameraPoint(_followPlayer.getEndOfLoopPosition(), _followPlayer.getStartYPosition());
        }

        _missionComplete.setTime(time);
    }
}
