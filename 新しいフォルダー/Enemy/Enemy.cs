using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    [SerializeField] private GameObject attackHitBox;
    
    private Vector3 moveDirection;
    private ReactiveProperty<bool> isMoving = new(true);
    public ReadOnlyReactiveProperty<bool> IsMoving => isMoving;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        moveDirection = Vector3.left;
        isMoving.Value = true;

        if(attackHitBox == null)
        {
            Debug.LogError("attack hit boxを設定してください。");
        }
    }

    private void FixedUpdate()
    {
        //ずっと移動し続けるように
        if (isMoving.Value)
        {
            float speed = data.Speed;
            _rigidbody.linearVelocityX = speed * moveDirection.x;
        }
        else
        {
            _rigidbody.linearVelocityX = 0f;
        }
    }

    //踏まれたときの処理 1秒止まって、その間攻撃判定を消す
    public async UniTask OnStompedTask()
    {
        var cancellationToken = this.GetCancellationTokenOnDestroy();

        isMoving.Value = false;
        attackHitBox.SetActive(false);

        await UniTask.Delay(System.TimeSpan.FromSeconds(1), cancellationToken:cancellationToken);

        isMoving.Value = true;
        attackHitBox.SetActive(true);
    }

    //死亡処理
    public void Die()
    {
        Enemy1Visual visual = GetComponentInChildren<Enemy1Visual>();
        if (visual == null)
        {
            Debug.LogError("Enemy1Visualがありません");
            return;
        }

        StageManager.Instance.IncreaseKillCount();

        //死亡アニメーションスタートと同時に全てのコライダーを消す
        visual.DieAnimationStart();
        DisableAllColliders();
        AudioManager.Instance.PlayOneShotSe("Die"); //SE
    }

    public void OnDieAnimationEnd()
    {
        Destroy(gameObject);
    }

    //子オブジェクトを含めすべてのコライダーを消す
    private void DisableAllColliders()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }
}
