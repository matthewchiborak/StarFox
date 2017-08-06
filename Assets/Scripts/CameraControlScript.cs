using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlScript : MonoBehaviour {
    
    public FollowPlayer _followPlayer;
    public MissionCompleteCameraControl _missionComplete;
    public TransitionToARCamera _transToAR;

    private bool isTransitioning;
    
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

    public void transitionToAR(float transitionTime)
    {
        _followPlayer.enabled = false;
        isTransitioning = true;

        //_transToAR.setTransitionTime()
        _transToAR.setTransitionTime(transitionTime);
        _transToAR.enabled = true;
    }

    void Update()
    {
        if(isTransitioning)
        {
            if(_transToAR.isFinished)
            {
                isTransitioning = false;
                _transToAR.enabled = false;
                _followPlayer.enabled = true;
                _transToAR.isFinished = false;
            }
        }
    }
}
