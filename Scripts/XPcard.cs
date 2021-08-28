using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Experience Card")]
public class XPcard : ScriptableObject
{
    public string cardname;
    public Sprite cardpic;
    public int xpEmotion;
    public int xpPerspective;
    public int xpHealth;
    public int xpSelfAdvocacy;
    public string card_description;
}