using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public class RankViewTable
{
    [SerializeField] private Rank _rank;
    [SerializeField] private TextMeshProUGUI _viewText;
    [SerializeField] private Material _fontMaterial;

    public Rank Rank => _rank;
    public TextMeshProUGUI ViewText => _viewText;

    public Material FontMaterial => _fontMaterial;
}

[CreateAssetMenu(fileName = "ScoreRankViewTable", menuName = "Scriptable Objects/ScoreRankViewTable")]
public class ScoreRankViewTable : ScriptableObject
{
    [SerializeField] private List<RankViewTable> _spriteTables=new List<RankViewTable>();

    public TextMeshProUGUI GetTMP(Rank rank)
    {
        return _spriteTables
            .FindAll(t => t.Rank == rank)[0]
            .ViewText;
    }

    public Material GetFontMaterial(Rank rank)
    {
        return _spriteTables
            .FindAll(t => t.Rank == rank)[0]
            .FontMaterial;
    }
}
