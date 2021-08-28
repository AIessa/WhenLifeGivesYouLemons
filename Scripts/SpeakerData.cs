using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Dialog/Speaker")]
public class SpeakerData : ScriptableObject
{
    public const string DEFAULTPIC = "default_pic";
    public const string ALTERNATIVE = "alternative";

    public string speakerName;
    public Sprite default_pic, alternativepic;

    public bool player_knows_name = false;

    public Sprite GetCorrectPicture(string description)
    {
        switch (description)
        {
            default:
            case DEFAULTPIC: return default_pic;
            case ALTERNATIVE: return alternativepic;
        }
    }
}
