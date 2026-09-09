using TMPro;
using UnityEngine;

public class StageEnterButton : LoadSceneButton
{
    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private ScoreRankViewTable _scoreRankViewTable;
    [SerializeField] private TextMeshProUGUI _highScoreRankText;

    private int _nextStageInt;

    protected override void Awake()
    {
        base.Awake();

        _nextStageInt = (int)NextScene - (int)SceneType.Stage1 + 1;

        _buttonText.text = NextScene.ToString();
    }

    private void Start()
    {
        SetHighScore();
    }

    private void SetHighScore()
    {
        //ハイスコア取得
        int highScore = GameManager.Instance.GetHighScore(_nextStageInt);
        Rank rank = GameManager.Instance.GetRank(highScore);
        Debug.Log($"high score:{highScore}");

        //ハイスコア未登録の時は表示しない
        if(highScore <= 0)
        {
            _highScoreText.text = "";
            _highScoreRankText.text = "";
            return;
        }

        //ハイスコアとランクの表示
        _highScoreText.text = $"HIGH SCORE : {highScore}";
        _highScoreRankText.text = rank.ToString();
        _highScoreRankText.fontSharedMaterial = _scoreRankViewTable.GetFontMaterial(rank);
    }
}
