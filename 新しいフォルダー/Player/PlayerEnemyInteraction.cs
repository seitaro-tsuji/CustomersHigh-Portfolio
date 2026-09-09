using UnityEngine;

[RequireComponent (typeof(PlayerController))]
[RequireComponent(typeof(PlayerStatus))]
public class PlayerEnemyInteraction : MonoBehaviour
{
    [SerializeField] private float enemyStompJumpPower = 10f;

    private PlayerController controller;
    private PlayerStatus status;

    public bool HasStompRequest {  get; private set; }
    public bool HasDamageRequest { get; private set; }

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        status = GetComponent<PlayerStatus>();

        HasStompRequest = false;
        HasDamageRequest = false;
    }

    private void FixedUpdate()
    {
        //踏みつけ成功しているときの処理　ダメージ判定は消す
        if (HasStompRequest)
        {
            //todo  controllerクラスにbounceメソッドを追加して処理をそっちに投げる
            controller.Velocity = Vector2.up * enemyStompJumpPower;
            HasStompRequest = false;
            HasDamageRequest = false;
        }
        //ダメージを受けるときの処理
        else if (HasDamageRequest)
        {
            status.Damage();
            //StageManager.Instance.IncreaseDamageCount();
            HasDamageRequest= false;
        }
    }

    public void RequestStomp()
    {
        HasStompRequest = true;
    }

    public void RequestDamage()
    {
        HasDamageRequest = true;
    }
}
