using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class SceneReference
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset _asset;
#endif

    [SerializeField] private string _name;

    public string Name => _name;
}
