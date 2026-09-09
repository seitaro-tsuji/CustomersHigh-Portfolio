using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SeNameClip
{
    public string name;
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "SeDatabase", menuName = "Scriptable Objects/SeDatabase")]
public class SeDatabase : ScriptableObject
{
    [SerializeField] private List<SeNameClip> seNameClips = new();

    public AudioClip GetSeClip(string clipName)
    {
        SeNameClip seNameClip = seNameClips.Find(x=>x.name == clipName);
        if(seNameClip == null)
        {
            Debug.LogWarning("指定された名前のクリップがありません。");
            return null;
        }

        return seNameClip.clip;
    }
}
