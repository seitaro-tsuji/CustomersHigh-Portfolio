using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SectionGrade
{
    Grade1=1,
    Grade2=2, 
    Grade3=3,
    DebugGrade=10,
}

public enum SectionType
{
    Normal,
    SpecialAction
}

[Serializable]
public class SectionData
{
    [SerializeField] private PreparationSection sectionPrefab;
    [SerializeField] private SectionGrade grade;
    [SerializeField] private SectionType type;

    public PreparationSection SectionPrefab => sectionPrefab;
    public SectionGrade Grade => grade;
    public SectionType Type => type;
}

[CreateAssetMenu(fileName = "SectionDatabase", menuName = "Scriptable Objects/SectionDatabase")]
public class SectionDatabase : ScriptableObject
{
    [SerializeField] private List<SectionData> sectionDatas = new List<SectionData>();

    public PreparationSection GetRondomSectionPrefab(SectionGrade grade, SectionType type=SectionType.Normal)
    {
        //指定されたtierのリスト
        List<SectionData> candiDatas = sectionDatas
            .Where(x=>x.Type == type)
            .Where(x=>x.Grade == grade)
            .ToList();

        if(candiDatas.Count == 0)
        {
            throw new InvalidOperationException($"Grade '{grade}' に対応するsectionが登録されていません。");
        }

        //ランダムで一つ返す
        int index = UnityEngine.Random.Range(0, candiDatas.Count);
        return candiDatas[index].SectionPrefab;
    }
}
