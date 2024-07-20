using System;
using UnityEngine;

[ Serializable ]
public class CutsceneSegmentData 
{
    [ SerializeField ] private Sprite image;
    
    [ SerializeField ] private string text;
    
    [ SerializeField ] private int transitionSpeedInSeconds;

    public Sprite Image => image;

    public string Text => text;

    public int TransitionSpeedInSeconds => transitionSpeedInSeconds;
}
