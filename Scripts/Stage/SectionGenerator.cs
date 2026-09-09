using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class SectionGenerator : MonoBehaviour
{
    [BoxGroup("StageSetting"), SerializeField] private SectionDatabase sectionDatabase;
    [BoxGroup("StageSetting"), SerializeField] private List<StageSetting> stageSettings;    //0がデバッグ用ステージ

    private Vector3 nextSectionStartPosition;       //次に生成する区画のstart pointを置くべきワールド座標
    private List<PreparationSection> preparationSections = new();   //生成する区画を順番に格納するリスト

    private void Start()
    {
        //準備エリアのリストを作る
        MakeStagePreparationsList(StageManager.Instance.CurrentStage);

        //用意した準備エリアリストから生成する
        nextSectionStartPosition = transform.position;  //最初の生成場所
        GenerateSections();

        //準備エリアの後ろに本ステージを移動させる
        SetMainStagePosition(StageManager.Instance.CurrentStage);
    }

    //指定した個数だけの区画をランダムで区画リストに追加する
    private void AddRandomSectionsToList(int minGrade, int maxGrade, SectionType type)
    {
        int generateGrade = Random.Range(minGrade, maxGrade + 1);
        preparationSections.Add(sectionDatabase.GetRondomSectionPrefab((SectionGrade)generateGrade, type));
    }

    //指定したステージの準備エリアリストをつくる
    private void MakeStagePreparationsList(int stage=1)
    {
        if(stage > stageSettings.Count)
        {
            Debug.LogError($"stageSettingsの要素数が足りません。" +
                $"stage:{stage}, settings.Count:{stageSettings.Count}");

            return;
        }

        preparationSections.Clear();
        foreach(var rule in stageSettings[stage].PreparationRules)
        {
            AddRandomSectionsToList(minGrade:rule.MinGrade, maxGrade:rule.MaxGrade, type:rule.Type);
        }
    }

    //準備エリアリストの中身を生成する
    private void GenerateSections()
    {
        foreach(var section in preparationSections)
        {
            //適当な位置に区画を生成
            PreparationSection generatedSection = Instantiate(section);

            //生成された場所から生成したかった場所への差分ベクトルを計算し、その分移動する
            Vector3 startPointDiff = nextSectionStartPosition - generatedSection.StartPoint;
            generatedSection.transform.position += startPointDiff;

            //次の区画生成の座標を今の区画のEndPointにする
            nextSectionStartPosition = generatedSection.EndPoint;
        }
    }

    //指定したステージのメインステージ部分を準備エリアの後ろに生成する
    private void SetMainStagePosition(int stage=1)
    {
        if(stage > stageSettings.Count)
        {
            Debug.LogError($"StageSettingsの要素数が少なすぎます。stage:{stage}, roots.Count:{stageSettings.Count}");
        }

        //stage settingからメインステージのPrefabを取得して適当な位置に生成する
        MainStageRoot mainStageRootPrefab = stageSettings[stage].MainStageRoot;
        var mainStageRoot = Instantiate(mainStageRootPrefab);

        //生成したメインステージを正しい位置に修正する
        Vector3 startPointDiff = nextSectionStartPosition - mainStageRoot.StartPointPos;
        mainStageRoot.transform.position += startPointDiff;
    }
}
