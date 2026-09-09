using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using R3;
using System.Collections.Generic;
using UnityEngine;

public enum ScoreKind
{
    Coin,
    Time,
    Damage,
    Kill,
    All
}

public class StageManager : MonoBehaviour
{
    [SerializeField] private BlackOutScreen blackOutScreen;
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private ScoreRankData _scoreRankData;
    [SerializeField] private AudioClip stageBgmClip;

    private int _currentStage;
    public int CurrentStage => _currentStage;

    static public StageManager Instance { get; private set; }   //シングルトン

    private bool finishedPreparation;
    private bool _getAttackAction = false;//敵撃破可能アクションをゲットしたか

    private float _timeCount;
    public float TimeCount => _timeCount;

    //プレイヤーの被弾回数
    public int DamageCount {  get; private set; }

    //敵撃破回数
    public int KillEnemyCount { get; private set; }

    //スコアの保持
    private Dictionary<string, int> _scoreDic=new Dictionary<string, int>();
    bool _updateHighScore;

    private void Awake()
    {
        if(Instance != null && Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;


        _timeCount = 0f;
        finishedPreparation = false;
        DamageCount = 0;
        KillEnemyCount = 0;
        _updateHighScore = false;
    }

    private void Start()
    {
        _currentStage = GameManager.Instance.CurrentStage;

        //UIManager.Instance.StartPreparationTextAnimation();

        if (blackOutScreen == null)
        {
            Debug.LogError("black out screenが渡されていません。");
            return;
        }
        if (playerInteraction == null)
        {
            Debug.LogError("player interactionが渡されていません。");
            return;
        }

        if(stageBgmClip == null)
        {
            Debug.LogError("StageBGMが渡されていません。");
            return;
        }
        AudioManager.Instance.StartPlayingBgm(stageBgmClip);

        //フェードインが終了したら準備エリア開始演出を出すようにする
        GameManager.Instance.IsFadingInOut
            .Pairwise()
            .Subscribe(pair =>
            {
                bool prev = pair.Previous;
                bool cur = pair.Current;

                if (prev && !cur)
                {
                    UIManager.Instance.StartPreparationTextAnimation();
                }
            });
    }

    private void Update()
    {
        if (!finishedPreparation)
            return;

        _timeCount += Time.deltaTime;
    }

    public void GetAttackAction()
    {
        _getAttackAction = true;
    }
    
    //メインステージスタート
    public void MainStageStart()
    {
        if (finishedPreparation)
            return;

        finishedPreparation = true;
        UIManager.Instance.OnMainStageStart();

    }

    public void StageClear()
    {
        //スコア計算をここですると最後のコインがスコアに入らない可能性があるので修正

        //リザルトの表示
        ShowResultAsync().Forget();
    }


    //
    public void IncreaseDamageCount()
    {
        DamageCount += 1;
    }

    public void IncreaseKillCount()
    {
        KillEnemyCount++;
    }

    public int GetCoinCount()
    {
        return playerInteraction.CoinNum.CurrentValue;
    }

    public int GetScore(ScoreKind key)
    {
        return key switch
        {
            ScoreKind.Coin      => _scoreDic["coin"],
            ScoreKind.Time      => _scoreDic["time"],
            ScoreKind.Damage    => _scoreDic["damage"],
            ScoreKind.Kill      => _scoreDic["kill"],
            ScoreKind.All       => _scoreDic["all"],
            _                   => 0
        };
    }

    public bool IsFinalStage()
    {
        return GameManager.Instance.IsFinalStage();
    }

    private int CulculateCoinScore()
    {
        return GetCoinCount() * 100;
    }

    private int CulculateTimeScore()
    {
        int intTimeCount = (int)_timeCount;

        int score = 8000 - 200 * intTimeCount;
        return Mathf.Max(score, 0);
    }

    private int CulculateDamageCountScore()
    {
        int score = 2000 - 500 * DamageCount;
        return Mathf.Max(score, 0);
    }

    private int CulculateKillCountScore()
    {
        return KillEnemyCount * 200;
    }

    //スコア計算と保持、ハイスコア更新をする
    private void CulcAndSetScore()
    {
        //スコアの計算
        _scoreDic["coin"] = CulculateCoinScore();
        _scoreDic["time"] = CulculateTimeScore();
        _scoreDic["damage"] = CulculateDamageCountScore();
        _scoreDic["kill"] = CulculateKillCountScore();
        _scoreDic["all"] = _scoreDic["coin"] + _scoreDic["time"] + _scoreDic["damage"] + _scoreDic["kill"];

        //スコアの保存(リザルトで"ハイスコア更新!!とか表示するかも")
        _updateHighScore = GameManager.Instance.SaveScore(_scoreDic["all"], CurrentStage);
        if (_updateHighScore)
        {
            Debug.Log("スコア更新");
        }
    }

    private async UniTask ShowResultAsync()
    {
        UIManager.Instance.HideAllUI();

        await blackOutScreen.FadeInOrOutAsync(targetAlpha: 0.75f);

        //ここのタイミングでスコア計算する(最後のコインがスコアに入らないことがあるから)
        CulcAndSetScore();

        //getFireBallはFireBallを取得しているかどうか
        UIManager.Instance.ShowResultPanel(_getAttackAction);
    }
}
