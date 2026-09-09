using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Title,
    DebugStage,
    Stage1,
    Stage2,
    Stage3,
    Count
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private ScoreRankData _scoreRankData;
    [SerializeField] private ScoreRankViewTable _scoreRankViewTable;
    [SerializeField] private BlackOutScreen _blackOutScreen;
    [SerializeField] private float _fadeDuration = 0.5f;    //フェードイン/アウトの時間
    [SerializeField] private float _blackDuration = 0.1f;   //フェードアウト終了後、シーン遷移を見せないように暗くする時間
    [SerializeField] private GameObject _blockInputScreen;
    [SerializeField, BoxGroup("デバッグ用")] private SceneType _initialScene;   //開始scene

    public static GameManager Instance;

    private ReactiveProperty<bool> _isFadingInOut = new(false);
    public ReadOnlyReactiveProperty<bool> IsFadingInOut => _isFadingInOut;


    private SceneType _currentScene;
    public int CurrentStage
    {
        get
        {
            return (int)_currentScene - (int)SceneType.Stage1 + 1;
        }
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        //最初のシーンを読み込む
        _currentScene = _initialScene;
        SceneManager.LoadScene(GetSceneName(_initialScene));

        //操作可能にする
        EnableClickTap();
    }

    public void ChangeScene(SceneType nextScene)
    {
        _currentScene = nextScene;

        //タイトル画面への移行
        ChangeSceneAsync(GetSceneName(nextScene)).Forget();
    }

    public void ReloadCurrentScene()
    {
        ChangeScene(_currentScene);
    }

    public void LoadNextStage()
    {
        if (IsFinalStage())
        {
            Debug.LogError("今いるステージが最終ステージです。");
        }

        SceneType nextScene = (SceneType)((int)_currentScene + 1);
        ChangeScene(nextScene);
    }

    public Rank GetRank(int score)
    {
        Rank rank = _scoreRankData.GetRank(score);
        Debug.Log($"Rank:{rank.ToString()}, Score:{score}");
        return rank;
    }

    public TextMeshProUGUI GetRankText(int score)
    {
        Rank rank = GetRank(score);
        return _scoreRankViewTable.GetTMP(rank);
    }

    public bool SaveScore(int score, int stage)
    {
        int highScore = GetHighScore(stage);

        if(score <= highScore)
        {
            return false;
        }

        PlayerPrefs.SetInt($"HighScore{stage}", score);
        return true;
    }

    public int GetHighScore(int stage)
    {
        return PlayerPrefs.GetInt($"HighScore{stage}", 0);
    }

    //今いるステージが最終ステージかどうか
    public bool IsFinalStage()
    {
        return ((int)_currentScene == (int)SceneType.Count - 1);
    }

    //フェードイン終了までの時間を返す
    public float GetFadeInSec()
    {
        return _fadeDuration + _blackDuration;
    }

    private async UniTask ChangeSceneAsync(string nextSceneName)
    {
        //操作不可能、フェードアウト
        DisableClickTap();
        _isFadingInOut.Value = true;
        await _blackOutScreen.FadeOutAsync(_fadeDuration);
        _blackOutScreen.SetAlpha(1f);

        //シーン変更
        SceneManager.LoadScene(nextSceneName);
        await UniTask.Delay(System.TimeSpan.FromSeconds(_blackDuration));

        //フェードイン開始、その後操作可能
        await _blackOutScreen.FadeInAsync(_fadeDuration);
        _isFadingInOut.Value = false;
        EnableClickTap();
    }

    private string GetSceneName(SceneType type)
    {
        if(type == SceneType.Title)
        {
            return "TitleScene";
        }
        else
        {
            return "Stage1Scene";
        }
    }

    private void EnableClickTap()
    {
        _blockInputScreen.SetActive(false);
    }

    private void DisableClickTap()
    {
        _blockInputScreen.SetActive(true);
    }
}
