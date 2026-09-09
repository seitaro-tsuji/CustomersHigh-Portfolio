using System;
using System.Collections.Generic;
using UnityEngine;

public enum Rank
{
    SS,S,A,B,C,D
}

[Serializable]
public class RankTable
{
    [SerializeField] private Rank _rank;
    [SerializeField] private int _border;

    public Rank Rank => _rank;
    public int Border => _border;

    public bool IsOverBorder(int score)
    {
        return score >= _border;
    }
}

[CreateAssetMenu(fileName = "ScoreRankData", menuName = "Scriptable Objects/ScoreRankData")]
public class ScoreRankData : ScriptableObject
{
    [SerializeField] private List<RankTable> rankTables = new List<RankTable>();

    public Rank GetRank(int score)
    {
        foreach (RankTable table in rankTables)
        {
            if (table.IsOverBorder(score))
            {
                return table.Rank;
            }
        }

        //‚Ç‚ê‚É‚à“–‚Ä‚Í‚Ü‚ç‚È‚¢ê‡
        return Rank.D;
    }
}
