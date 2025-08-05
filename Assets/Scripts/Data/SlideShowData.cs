using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlideShowData", menuName = "Scriptable Objects/SlideShowData")]
public class SlideShowData : ScriptableObject
{
    public List<SlideSet> slideSets;
}

[System.Serializable]
public class SlideSet
{
    public AudioClip narrationAudio;
    public List<Sprite> slides;
}
