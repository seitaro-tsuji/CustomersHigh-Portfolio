using NaughtyAttributes;
using R3;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent (typeof(PlayerController))]
public class PlayerInteraction : MonoBehaviour
{
    [BoxGroup("Player"), SerializeField] private PlayerSpecialAction playerSpecialAction;
    [BoxGroup("Player"), SerializeField] private PlayerMovement playerMovement;

    [BoxGroup("SpecialAction"), SerializeField] private SpecialActionButton specialActionButton;
    [BoxGroup("SpecialAction"), SerializeField] private GameObject dashGauge;

    private PlayerMovement _movement;
    private PlayerController _controller;

    //コイン、スピードアップ、ジャンプアップの取得数
    [SerializeField, ReadOnly] private ReactiveProperty<int> _coinCount;
    public ReadOnlyReactiveProperty<int> CoinNum => _coinCount;

    private ReactiveProperty<int> _speedBoostCount;
    public ReadOnlyReactiveProperty<int> SpeedBoostCount => _speedBoostCount;

    private ReactiveProperty<int> _jumpBoostCount;
    public ReadOnlyReactiveProperty<int> JumpBoostCount => _jumpBoostCount;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _controller = GetComponent<PlayerController>();
        _coinCount = new(0);
        _speedBoostCount = new(0);
        _jumpBoostCount = new(0);
    }

    //boost itemを取得したとき
    public void GetBoostItem(BoostItem.BoostType type)
    {
        switch (type)
        {
            case BoostItem.BoostType.Speed:
                _movement.BoostSpeed();
                _speedBoostCount.Value += 1;
                break;

            case BoostItem.BoostType.JumpPower:
                _movement.BoostJumpPower();
                _jumpBoostCount.Value += 1;
                break;

            case BoostItem.BoostType.MaxHp:
                if (!TryGetComponent<PlayerStatus>(out PlayerStatus status))
                {
                    Debug.Log("player statusスクリプトがアタッチされていません。");
                    return;
                }
                status.IncreaseMaxHp();
                break;

            default:
                Debug.LogWarning($"BoostItemの値が不正です。 BoostType:{type}");
                break;
        }
    }

    //Coinを取得したとき
    [Button]
    public void GetCoin(int num=1)
    {
        _coinCount.Value += num;
    }

    //回復アイテムを取得したとき
    public void GetHealItem(int amount = 1)
    {
        if(!TryGetComponent<PlayerStatus>(out  PlayerStatus status))
        {
            Debug.Log("player statusスクリプトがアタッチされていません。");
            return;
        }

        status.Heal(amount);
    }

    public void GetSpecialActionItem(SpecialActionType type)
    {
        switch (type)
        {
            case SpecialActionType.DoubleJump:
                playerMovement.SetSpecialActionDoubleJump();
                break;

            case SpecialActionType.Dash:
                playerSpecialAction.SetDashAction();
                specialActionButton.gameObject.SetActive(true);
                dashGauge.SetActive(true);
                break;

            case SpecialActionType.FireBall:
                playerSpecialAction.SetFireBallAction();
                specialActionButton.gameObject.SetActive(true);
                StageManager.Instance.GetAttackAction();    //攻撃可能アクションをゲットしたことを通知
                break;

            case SpecialActionType.AllItemCollect:
                playerSpecialAction.SetAllCollectAction();
                specialActionButton.gameObject.SetActive(true);
                break;
        }

        UIManager.Instance.SetSpecialActionText(type);
    }
}
