using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SectionSelectionRule
{
    [SerializeField] private int minGrade;
    [SerializeField] private int maxGrade;
    [SerializeField] private SectionType type;

    public int MinGrade => minGrade;
    public int MaxGrade => maxGrade;
    public SectionType Type => type;
}

[CreateAssetMenu(fileName = "StageSetting", menuName = "Scriptable Objects/StageSetting")]
public class StageSetting : ScriptableObject
{
    [SerializeField] private List<SectionSelectionRule> preparationRules = new();
    [SerializeField] private MainStageRoot mainStageRoot;

    public IReadOnlyList<SectionSelectionRule> PreparationRules => preparationRules;
    public MainStageRoot MainStageRoot => mainStageRoot;
}
