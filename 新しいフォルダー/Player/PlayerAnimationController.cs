using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

[RequireComponent (typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerStatus playerStatus;

    private Animator animator;

    private void Awake()
    {
        if(playerMovement == null)
        {
            Debug.LogError("PlayerMovementが渡されていません。");
            return;
        }
        if (playerStatus == null)
        {
            Debug.LogError("PlayerStatusが渡されていません。");
            return;
        }

        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        //非ジャンプ→ジャンプ状態になったらトリガー
        playerMovement.IsJumping
            .Pairwise()
            .Subscribe(pair =>
            {
                bool prev = pair.Previous;
                bool cur = pair.Current;
                if (!prev && cur)
                {
                    //ジャンプ開始
                    animator.SetTrigger("JumpTrigger");
                }
            })
            .AddTo(this);

        playerMovement.IsDoubleJumping
            .Pairwise()
            .Subscribe(pair =>
            {
                bool cur = pair.Current;
                bool prev = pair.Previous;
                if (!prev && cur)
                {
                    //ダブルジャンプ開始
                    animator.SetTrigger("DoubleJumpTrigger");
                }
            })
            .AddTo(this);

        //死亡状態で死亡アニメーションを流す
        playerStatus.IsDead
            .Subscribe(value =>
            {
                if (value)
                {
                    animator.SetTrigger("DieTrigger");
                }
            })
            .AddTo(this);
    }

    private void Update()
    {
        animator.SetFloat("SpeedY", playerMovement.SpeedY);
    }

    //Animationから死亡アニメーション終了時に呼ぶ
    public void OnDeathAnimationFinished()
    {
        UIManager.Instance.ShowFailureUI();
    }
}
