using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpecialAction : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private SpecialActionButton specialActionButton;

    [BoxGroup("Dash"), SerializeField] private float dashGaugeMax = 1f;
    [BoxGroup("Dash"), SerializeField, ReadOnly] private float debugDashGauge = 0f;

    [BoxGroup("FireBall"), SerializeField] private FireBall _fireBallPrefab;
    [BoxGroup("FireBall"), SerializeField] private int _fireBallMaxCount=2;

    [BoxGroup("AllCollect"), SerializeField] private Camera _camera;
    [BoxGroup("AllCollect"), SerializeField] private LayerMask _itemLayer;

    private ISpecialAction specialAction;

    //Dashアクションを取得したとき
    public void SetDashAction()
    {
        specialAction = new DashAction(playerMovement, dashGaugeMax);
        specialActionButton.Pressed += specialAction.OnPressed;
        specialActionButton.Held += specialAction.OnHeld;
        specialActionButton.Released += specialAction.OnReleased;

        //ボタンの文字変更
        TextMeshProUGUI buttonText = specialActionButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = "DASH";
        buttonText.color = Color.cyan;
    }

    //FireBallアクションを取得したとき
    public void SetFireBallAction()
    {
        specialAction = new FireBallAction(this);
        specialActionButton.Pressed += specialAction.OnPressed;

        //FireBallが生成できない時はボタン押せなく
        FireBall.Count
            .Select(count=>count<_fireBallMaxCount)
            .Subscribe(canShoot =>
            {
                Button button = specialActionButton.gameObject.GetComponent<Button>();
                button.interactable = canShoot;
            })
            .AddTo(this);

        //ボタンの文字変更
        TextMeshProUGUI buttonText = specialActionButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = "FIRE";
        buttonText.color = Color.orangeRed;
    }

    public void SetAllCollectAction()
    {
        PlayerInteraction interaction = GetComponent<PlayerInteraction>();
        specialAction = new AllItemCollectAction(interaction, _camera, _itemLayer, specialActionButton);
        specialActionButton.Pressed += specialAction.OnPressed;

        //ボタンの文字変更
        TextMeshProUGUI buttonText = specialActionButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = "ALL\nCOLLECT";
        buttonText.color = Color.yellow;
    }

    private void Update()
    {
        specialAction?.Tick(Time.deltaTime);

        //デバッグ用
        if (specialAction is DashAction dashAction)
        {
            debugDashGauge = dashAction.DashGauge.CurrentValue;
        }
    }

    public DashAction GetDashAction()
    {
        if( specialAction is DashAction dashAction)
        {
            return dashAction;
        }
        else
        {
            Debug.LogWarning("特殊アクションがDashでない状態でGetDashActionが呼ばれています。");
            return null;
        }
    }

    public void CreateFireBall()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        //ファイアボール生成
        Debug.Log("ファイアボール生成");
        var fireBall = Instantiate(_fireBallPrefab, transform.position + Vector3.right, Quaternion.identity);
        fireBall.Initialize(rb.linearVelocityX);
    }
}
