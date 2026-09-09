using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] private bool useKillEnemyCountScore;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI coinScore;
    [SerializeField] private TextMeshProUGUI timeScore;
    [SerializeField] private TextMeshProUGUI damageCountScore;
    [ShowIf(nameof(useKillEnemyCountScore)), SerializeField] private TextMeshProUGUI killEnemyCountScore;
    [SerializeField] private TextMeshProUGUI SumScore;
    [SerializeField] private List<TextMeshProUGUI> scoreKindTexts;  //「コイン枚数」などのテキスト　失敗リザルトでは消すから
    [SerializeField] private RectTransform rankRect;
    [SerializeField] private Button goToTitleButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button nextStageButton;

    private List<TextMeshProUGUI> texts;


    //パネル表示時に呼び出す関数
    public void Initialize(bool clearFlag = true)
    {
        if (goToTitleButton == null || replayButton == null) 
        {
            Debug.LogError("タイトルボタンまたはリプレイボタンがnullです。");
            return;
        }

        SetTexts(clearFlag);

        texts = new List<TextMeshProUGUI>();
        texts.Add(resultText);
        texts.Add(coinScore);
        texts.Add(timeScore);
        texts.Add(damageCountScore);
        if (useKillEnemyCountScore)
        {
            texts.Add(killEnemyCountScore);
        }
        texts.Add(SumScore);

        //テキストやボタンは最初全部消しておく
        foreach (var text in texts)
        {
            text.enabled = false;
        }
        goToTitleButton.gameObject.SetActive(false);
        replayButton.gameObject.SetActive(false);
        nextStageButton.gameObject.SetActive(false);

        //クリアしたときは順に表示
        if (clearFlag)
            ShowAllResults().Forget();
        else
        {
            //リザルトメッセージ表示
            resultText.enabled = true;

            //スコア種類の表示を消す
            foreach (var text in scoreKindTexts)
            {
                text.enabled = false;
            }

            //タイトル、リプレイボタンは表示
            goToTitleButton.gameObject.SetActive(true);
            replayButton.gameObject.SetActive(true);
        }


    }

    //1秒間隔で順番に結果とボタンを表示していく
    private async UniTask ShowAllResults()
    {
        var cancellationToken = this.GetCancellationTokenOnDestroy();

        foreach (var text in texts)
        {
            text.enabled=true;
            await UniTask.Delay(System.TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);
        }

        //ランク表示演出
        await ShowRank();

        //ボタン表示(nextステージは最終ステージ以外のみ)
        goToTitleButton.gameObject.SetActive(true);
        replayButton.gameObject.SetActive(true);
        if (!StageManager.Instance.IsFinalStage())
        {
            nextStageButton.gameObject.SetActive(true);
        }
    }

    private async UniTask ShowRank()
    {
        //ランクの生成
        int score = StageManager.Instance.GetScore(ScoreKind.All);
        TextMeshProUGUI scoreTextPrefab = GameManager.Instance.GetRankText(score);
        var rankText = Instantiate(scoreTextPrefab, rankRect);
        rankText.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

        //演出用の初期化
        Vector3 targetScale = Vector3.one;
        float targetAlpha = 1f;
        float duration = 0.5f;

        //最初に大きく、透明にしておく
        rankText.transform.localScale = Vector3.one * 5f;
        Color c = rankText.color;
        c.a = 0f;
        rankText.color = c;

        //小さくしながら、回転して、透過度を上げる
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rankText.transform
            .DOScale(targetScale, duration)
            );
        sequence.Join(rankText
            .DOFade(targetAlpha, duration)
            );
        sequence.Join(rankText.transform
            .DORotate(new Vector3(0f,0f,1080f), duration, RotateMode.FastBeyond360)
            );

        await sequence.AsyncWaitForCompletion();
    }

    private void SetTexts(bool clearFlag=true)
    {
        //失敗時はリザルトテキストを変更
        if (!clearFlag)
        {
            resultText.text = "死んでしまった…。";
            resultText.color = Color.red;
            return;
        }

        //コインによるスコア
        coinScore.text = $"{StageManager.Instance.GetCoinCount()} 枚:   " +
            $"+{StageManager.Instance.GetScore(ScoreKind.Coin)}";

        //タイムによるスコア
        timeScore.text = $"{(int)StageManager.Instance.TimeCount} 秒:   " +
            $"+{StageManager.Instance.GetScore(ScoreKind.Time)}";

        //被弾回数によるスコア
        damageCountScore.text = $"{StageManager.Instance.DamageCount} 回:   " +
            $"+{StageManager.Instance.GetScore(ScoreKind.Damage)}";

        //敵撃破によるスコア
        if (useKillEnemyCountScore)
        {
            killEnemyCountScore.text = $"{StageManager.Instance.KillEnemyCount} 体:   " +
                $"+{StageManager.Instance.GetScore(ScoreKind.Kill)}";
        }

        //合計スコア
        SumScore.text = $"{StageManager.Instance.GetScore(ScoreKind.All)}";
    }
}
