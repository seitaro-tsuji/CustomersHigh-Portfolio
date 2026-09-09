using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{
    static public UIManager Instance;

    [SerializeField] private Transform playerTransform;

    [BoxGroup("Result"), SerializeField] private ResultPanel resultPanel;
    [BoxGroup("Result"), SerializeField] private ResultPanel resultPanelWithFireBall;

    [BoxGroup("UI"), SerializeField] private UISpace uiSpace;
    [BoxGroup("UI"), SerializeField] private Button specialActionButton;
    [BoxGroup("UI"), SerializeField] private GameObject dashGauge;

    [BoxGroup("Effect"), SerializeField] private Image dustParticlePrefab;
    [BoxGroup("Effect"), SerializeField] private TextMeshProUGUI plusTextPrefab;
    [BoxGroup("Effect"), SerializeField] private GetCoinText getCoinTextPrefab;    
    [BoxGroup("Effect"), SerializeField] private TextMeshProUGUI damageTextPrefab;
    [BoxGroup("Effect"), SerializeField] private StageStartText stageStartTextPrefab;
    [BoxGroup("Effect"), SerializeField] private Transform effectRoot;

    PlayerInteraction playerInteraction;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        playerInteraction = playerTransform.GetComponent<PlayerInteraction>();
    }

    void Start()
    {
        if (resultPanel == null)
        {
            Debug.LogError("result panelが設定されていません。");
            return;
        }

        resultPanel.gameObject.SetActive(false);
        resultPanelWithFireBall?.gameObject.SetActive(false);
    }

    //準備エリアスタートの演出
    public void StartPreparationTextAnimation()
    {
        var text = Instantiate(stageStartTextPrefab, effectRoot);
        text.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 700f);
        text.PlayTextAnimation("準備エリア","START!!");
    }

    //メインステージ開始時に呼ぶ
    public void OnMainStageStart()
    {
        //HPが更新されたら表示も更新するように
        uiSpace.OnMainStageStart();

        DisplayTimeCoin();

        //テキストの演出
        var text = Instantiate(stageStartTextPrefab, effectRoot);
        text.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 700f);
        text.PlayTextAnimation($"STAGE {StageManager.Instance.CurrentStage}", "START!!");
    }

    public void ShowResultPanel(bool hasAttackAction)
    {
        if(!hasAttackAction)
        {
            resultPanel.gameObject.SetActive(true);
            resultPanel.Initialize();
        }
        else
        {
            resultPanelWithFireBall.gameObject.SetActive(true);
            resultPanelWithFireBall.Initialize();
        }
    }

    public void ShowFailureUI()
    {
        HideAllUI();
        resultPanel.gameObject.SetActive(true);
        resultPanel.Initialize(clearFlag: false);
    }

    public void HideAllUI()
    {
        uiSpace.HideAllUI();
        specialActionButton.gameObject.SetActive(false);
        dashGauge.SetActive(false);
    }

    //ブーストアイテムを取得したときの演出
    public void StartGetBoostItemEffect(BoostItem.BoostType type, Vector3 worldPos)
    {
        //目標地点の設定
        Vector3 effectTargetPos = type switch
        {
            BoostItem.BoostType.Speed => uiSpace.GetSpeedUiTransform().position,
            BoostItem.BoostType.JumpPower => uiSpace.GetJumpPowerUiTransform().position,
            BoostItem.BoostType.MaxHp => uiSpace.GetHpUiTransform().position,
            _ => throw new ArgumentOutOfRangeException()
        };

        //生成地点の設定
        Vector3 effectStartPos = Camera.main.WorldToScreenPoint(worldPos);

        //生成
        Image effectImage = Instantiate(dustParticlePrefab, effectStartPos, Quaternion.identity, effectRoot);

        //色を変更する
        effectImage.color = type switch
        {
            BoostItem.BoostType.Speed => Color.blue,
            BoostItem.BoostType.JumpPower => Color.orange,
            BoostItem.BoostType.MaxHp => Color.red,
            _ => throw new ArgumentOutOfRangeException()
        };
        

        //UIまで移動させてから消す
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(effectImage.transform.DOMove(effectTargetPos, duration: 1f).SetEase(Ease.InQuad))
            .AppendCallback(() =>
            {
                uiSpace.IncreaseBoostStatus(type);//ステータス表示の更新
                Destroy(effectImage.gameObject);//エフェクトを消す

                //+1テキストを出して演出する
                TextMeshProUGUI effectText = GeneratePlusTextEffect(type);
                PlayStatusChangeTextEffect(effectText);
            });
    }

    //特殊アクションでアイテムゲットしたときの演出スタート
    public void StartAllCollectDirection(CollectableObject collectableObject, float duration=0.5f)
    {
        //目標位置の設定
        Vector3 targetPos = Camera.main.WorldToScreenPoint(playerTransform.position);

        //生成位置の設定
        Vector3 startPos = Camera.main.WorldToScreenPoint(collectableObject.transform.position);

        //エフェクトの生成
        var effect = Instantiate(dustParticlePrefab, startPos, Quaternion.identity, effectRoot);
        
        //色の変更
        if(collectableObject is Coin coin)
        {
            //通常のコインの時
            if(coin.Amount == 1)
            {
                effect.color = Color.gray;
            }
            //大コインの時
            else
            {
                effect.color = Color.yellow;
            }
        }

        Sequence sequence = DOTween.Sequence();
        sequence
            .AppendCallback(() =>
            {
                //アイテムを消す
                if(!collectableObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer renderer))
                {
                    Debug.Log($"{collectableObject}にsprite rendererがありません。");
                }

                renderer.enabled = false;
            })
            .Append(effect.transform.DOMove(targetPos, duration).SetEase(Ease.OutQuad))//エフェクトの移動
            .AppendCallback(() =>
            {
                //エフェクトを消して、アイテム回収処理
                Destroy(effect.gameObject);
                collectableObject.OnPlayerTouch(playerInteraction);
            });
    }

    public void PlayGetCoinsDirection(int amount=1)
    {
        //生成位置の設定
        Vector3 startPos = Camera.main.WorldToScreenPoint(playerTransform.position);

        var text = Instantiate(getCoinTextPrefab, startPos, Quaternion.identity, effectRoot);
        text.Initialize(amount);
    }

    public void PlayDamageEffect(Vector3 playerPos, int damageAmount=1)
    {
        //生成位置
        Vector3 effectStartPos = Camera.main.WorldToScreenPoint(playerPos) + new Vector3(0, 20f);

        //生成
        TextMeshProUGUI damageText = Instantiate(damageTextPrefab, effectStartPos, Quaternion.identity, effectRoot);
        damageText.text = $"HP -{damageAmount}";

        //演出スタート
        PlayStatusChangeTextEffect(damageText);
    }

    //UI Spaceの操作をそのまま行う関数
    public void SetSpecialActionText(SpecialActionType type) => uiSpace.SetSpecialActionText(type);
    public void ShowMessage(string message, float showSec = 3f) => uiSpace.ShowMessage(message, showSec).Forget();
    public void DisplayTimeCoin() => uiSpace.DisplayTimeCoin();

    private TextMeshProUGUI GeneratePlusTextEffect(BoostItem.BoostType type, int amount=1, int size=65)
    {
        //位置の決定
        Vector2 screenPos = type switch
        {
            BoostItem.BoostType.Speed => uiSpace.GetSpeedUiTransform().position + new Vector3(150f,0),
            BoostItem.BoostType.JumpPower => uiSpace.GetJumpPowerUiTransform().position + new Vector3(125f, 0),
            BoostItem.BoostType.MaxHp => uiSpace.GetHpUiTransform().position + new Vector3(110f, 0),
            _ => throw new ArgumentOutOfRangeException()
        };

        //生成
        TextMeshProUGUI text = Instantiate(plusTextPrefab, screenPos, Quaternion.identity, effectRoot);

        //色の変更
        text.color = type switch
        {
            BoostItem.BoostType.Speed => Color.cyan,
            BoostItem.BoostType.JumpPower => Color.orange,
            BoostItem.BoostType.MaxHp => Color.red,
            _ => throw new ArgumentOutOfRangeException()
        };

        return text;
    }

    //テキストが上に移動しながらフェードアウトする
    private void PlayStatusChangeTextEffect(TextMeshProUGUI textEffect)
    {
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(textEffect.transform.DOMoveY(textEffect.transform.position.y + 60f, duration: 0.5f))
            .Join(textEffect.DOFade(endValue: 0f, duration: 0.5f))
            .OnComplete(() => Destroy(textEffect.gameObject));
    }
}
