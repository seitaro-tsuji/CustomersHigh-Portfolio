using Cysharp.Threading.Tasks;
using R3;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISpace : MonoBehaviour
{
    [BoxGroup("Player")]
    [SerializeField] private PlayerInteraction playerInteraction;
    [BoxGroup("Player")]
    [SerializeField] private PlayerStatus playerStatus;

    [BoxGroup("UI")]
    [SerializeField] private TMP_Text coinCountText;
    [BoxGroup("UI")]
    [SerializeField] private TMP_Text timeCountText;
    [BoxGroup("UI")]
    [SerializeField] private TMP_Text messageText;
    [BoxGroup("UI")]
    [SerializeField] private TMP_Text hpText;
    [BoxGroup("UI")]
    [SerializeField] private TMP_Text speedText;
    [BoxGroup("UI")]
    [SerializeField] private TMP_Text jumpText;
    [BoxGroup("UI")]
    [SerializeField] private TMP_Text specialActionText;
    [BoxGroup("UI")]
    [SerializeField] private Button goToTitleButton;
    [BoxGroup("UI")]
    [SerializeField] private Button replayButton;

    private int _speedBoostCount = 0;
    private int _jumpBoostCount = 0;
    private int _hpBoostCount = 0;



    private void Start()
    {
        if (coinCountText == null)
            Debug.LogWarning("coinCountTextがnullです。");
        if (hpText == null)
            Debug.LogWarning("hpTextがnullです。");

        //コイン取得数の更新
        playerInteraction.CoinNum
            .Subscribe(x =>
            {
               coinCountText.text = $"コイン：{x}";
            })
            .AddTo(this);
    }

    private void Update()
    {
        //時間の更新
        if (timeCountText.gameObject.activeInHierarchy)
            timeCountText.text = $"Time：{(int)StageManager.Instance.TimeCount}";
    }

    public void OnMainStageStart()
    {
        //HPテキストの更新開始
        playerStatus.CurrentHp
            .Subscribe(x =>
            {
                hpText.text = $"HP : {x} / {playerStatus.MaxHp}";

                hpText.color = x switch
                {
                    0 => Color.red,
                    1 => Color.red,
                    2 => Color.yellow,
                    _ => Color.green
                };
            })
            .AddTo(this);
    }

    public void DisplayTimeCoin()
    {
        coinCountText.gameObject.SetActive(true);
        timeCountText.gameObject.SetActive(true);
    }

    public void HideAllUI()
    {
        coinCountText.gameObject.SetActive(false);
        timeCountText.gameObject.SetActive(false);
        messageText.gameObject.SetActive(false);
        hpText.gameObject.SetActive(false);
        speedText.gameObject.SetActive(false);
        jumpText.gameObject.SetActive(false);
        specialActionText.gameObject.SetActive(false);
        goToTitleButton.gameObject.SetActive(false);
        replayButton.gameObject.SetActive(false);
    }

    public async UniTask ShowMessage(string message, float showSec = 3f)
    {
        var cancellationToken = this.GetCancellationTokenOnDestroy();

        messageText.text = message;
        messageText.gameObject.SetActive(true);

        await UniTask.Delay(System.TimeSpan.FromSeconds(showSec), cancellationToken:cancellationToken);
        //fixme　このままだと1個目が消える前に2個目のメッセージを表示するとバグる

        messageText.gameObject.SetActive(false);
    }

    //Boostのステータスを+1する
    public void IncreaseBoostStatus(BoostItem.BoostType type)
    {
        switch (type)
        {
            case BoostItem.BoostType.Speed:
                _speedBoostCount++;
                speedText.text = $"Speed：+{_speedBoostCount}";
                break;

            case BoostItem.BoostType.JumpPower:
                _jumpBoostCount++;
                jumpText.text = $"Jump：+{_jumpBoostCount}";
                break;

            case BoostItem.BoostType.MaxHp:
                _hpBoostCount++;
                hpText.text = $"HP：{_hpBoostCount + 1} / {_hpBoostCount + 1}";  //HPは最初から1だから
                hpText.color = (_hpBoostCount+1) switch
                {
                    2 => Color.yellow,
                    1 => Color.red,
                    0 => Color.red,
                    _ => Color.green
                };
                break;
            default:
                Debug.LogWarning("typeが不正です。");
                break;
        }
    }

    //special action textの更新
    public void SetSpecialActionText(SpecialActionType type)
    {
        switch (type)
        {
            case SpecialActionType.None:
                specialActionText.text = "Action : ";
                break;

            case SpecialActionType.Dash:
                specialActionText.text = "Action : Dash";
                specialActionText.color = Color.cyan;
                break;

            case SpecialActionType.DoubleJump:
                specialActionText.text = "Action : Double Jump";
                specialActionText.color = Color.orange;
                break;

            case SpecialActionType.FireBall:
                specialActionText.text = "Action : Fire Ball";
                specialActionText.color = Color.red;
                break;

            case SpecialActionType.AllItemCollect:
                specialActionText.text = "Action : All Collect";
                specialActionText.color = Color.yellow;
                break;

            default:
                break;
        }
    }

    public Transform GetHpUiTransform() => hpText.transform;

    public Transform GetSpeedUiTransform() => speedText.transform;

    public Transform GetJumpPowerUiTransform() => jumpText.transform;
}
