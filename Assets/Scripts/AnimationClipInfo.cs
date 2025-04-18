using UnityEngine;

public class AnimationClipInfo
{
    private AnimationClip _clip;
    public AnimationClip clip {  get { return _clip; } }

    private float _clipLength;
    public float clipLength { get { return _clipLength; } }

    private float _targetFPS;
    public float targetFPS { get { return _targetFPS; } }

    private int _numFramesAtFPS;
    public int numFramesAtFPS { get { return _numFramesAtFPS; } }

    private float _timeStep;
    public float timeStep { get { return _timeStep; } }
    
    // constructor
    public AnimationClipInfo(AnimationClip clip, float targetFPS)
    {
        _clip = clip;
        _clipLength = clip.length;

        // deal with some edge cases where our target FPS doesn't equal the clip FPS
        // but it's fine as long as the target can be divided by the clip FPS (i.e.
        float clipFPS = _clip.frameRate;
        if (targetFPS < clipFPS)
        {
            Debug.LogWarningFormat("Target FPS of {0} is less than clip's FPS of {1}; " +
                "some animation detail might be lost", targetFPS, clipFPS);
        }
        else if (targetFPS > clipFPS && targetFPS % clipFPS != 0)
        {
            Debug.LogWarningFormat("Target FPS of {0} is not divisible by clip FPS of {1};" +
                " May cause strange artifacts or skip keyframes.", targetFPS, clipFPS);
        }

        _targetFPS = targetFPS;
        _numFramesAtFPS = Mathf.RoundToInt(_targetFPS * _clipLength) + 1;
        _timeStep = 1f / _targetFPS;

        Debug.LogFormat("Clip {0} has {1} frames to render, timestep of {2}", 
            clip.name, _numFramesAtFPS, _timeStep);
    }
}
