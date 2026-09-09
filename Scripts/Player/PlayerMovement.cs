using UnityEngine;
using NaughtyAttributes;
using R3;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
public class PlayerMovement : MonoBehaviour
{
    public enum JumpState
    {
        Grounded,   //
        Boosting,   //ジャンプ長押し受付時間
        Rising,     //
        Falling,    //入力を話した後、上向きの速度が残っている間の含むので注意
        StompJumpWindow     //踏みつけジャンプ受付時間
    }

    public enum DoubleJumpState
    {
        Unavailable,        //使用不可(特殊アクションを取得していない)
        Available,          
        Consumed            //消費済み(特殊アクションは取得した)
    }

    [BoxGroup("Parameta"), SerializeField] private float initialSpeed = 3f;
    [BoxGroup("Parameta"), SerializeField] private float initialJumpPower = 8f;
    [BoxGroup("Parameta"), SerializeField] private float jumpBoostAmount = 2f;
    [BoxGroup("Parameta"), SerializeField] private float maxJumpSec = 0.25f;
    [BoxGroup("Parameta"), SerializeField] private float maxFallSpeed = 20f;

    [BoxGroup("DoubleJump"), SerializeField] private float doubleJumpPower = 6f;

    [BoxGroup("Dash"), SerializeField] private float dashPower = 2.5f;

    [BoxGroup("JumpSystem"), SerializeField] private Transform groundCheck;
    [BoxGroup("JumpSystem"), SerializeField] private float checkRadius = 0.1f;
    [BoxGroup("JumpSystem"), SerializeField] private LayerMask jumpableLayer;
    [BoxGroup("JumpSystem"), SerializeField] private GameObject stompHitBox;

    [SerializeField] private PlayerStatus playerStatus;

    private JumpState _jumpState= JumpState.Grounded;

    [SerializeField,ReadOnly]private float jumpPower;
    private float jumpBoostingSec;
    private DoubleJumpState doubleJumpState = DoubleJumpState.Unavailable;

    private bool isDashing = false;

    private ReactiveProperty<bool> _isJumping = new(false);
    public ReadOnlyReactiveProperty<bool> IsJumping => _isJumping;

    private ReactiveProperty<bool> _isDoubleJumping = new(false);
    public ReadOnlyReactiveProperty<bool> IsDoubleJumping => _isDoubleJumping;

    public Rigidbody2D Rb {  get; private set; }
    [SerializeField, ReadOnly] private float _currentSpeedX;

    //public float SpeedX => _currentSpeedX;
    public float SpeedY => Rb.linearVelocityY;

    public ReactiveProperty<bool> IsGrounded { get; } = new(false);


    private void Awake()
    {
        //初期化
        Rb = GetComponent<Rigidbody2D>();
        _currentSpeedX = initialSpeed;
        _isJumping.Value = false;
        _isDoubleJumping.Value = false;
        jumpPower = initialJumpPower;
        jumpBoostingSec = 0f;
        doubleJumpState = DoubleJumpState.Unavailable;
        isDashing = false;

        //空中→地上になったらジャンプフラグは消す
        IsGrounded
            .Pairwise()
            .Subscribe(pair =>
            {
                bool previous = pair.Previous;
                bool current = pair.Current;

                if (!previous && current)
                {
                    //着地
                    _isJumping.Value = false;
                    _isDoubleJumping.Value = false;
                }
            })
            .AddTo(this);

        //踏み判定
        if(stompHitBox == null)
        {
            Debug.LogError("StompHitBoxが渡されていません。");
        }
        stompHitBox.SetActive(false);
    }

    private void Start()
    {
        //死亡時全ての操作と動きを止める
        playerStatus.IsDead
            .Subscribe(value =>
            {
                if (value)
                {
                    StopMovement();
                }
            })
            .AddTo(this);
    }

    private void Update()
    {
        if (playerStatus.IsDead.CurrentValue)
        {
            return;
        }

        
    }

    private void FixedUpdate()
    {
        if (playerStatus.IsDead.CurrentValue)
        {
            return;
        }

        //jumpStateの更新(ここ以外の更新は条件で呼び出す関数で行う)
        IsGrounded.Value = CheckGrounded();
        UpdateJumpState(IsGrounded.Value);

        //常に一定のスピードで進み続ける
        ApplyVelocity();

        //fixme 速度計算の順番大丈夫？
        if (_jumpState == JumpState.Boosting)
            ApplyJumpVelocity();
    }

    //---------------------------------------------------------------------------------
    //              以下、外から呼ぶ関数
    //---------------------------------------------------------------------------------

    //ジャンプキーを押した瞬間にcontrollerから呼ばれる
    public void StartJumpRequest()
    {
        //地上ならジャンプ
        if (_jumpState == JumpState.Grounded)
            ChangeJumpState(JumpState.Boosting);
        //ダブルジャンプ可能なら使う
        else if(doubleJumpState == DoubleJumpState.Available)
            StartDoubleJump();
    }

    //ジャンプキーを離すとcontrollerから呼ばれる
    public void JumpReleaseRequest()
    {
        if (_jumpState == JumpState.Boosting)
            ChangeJumpState(JumpState.Rising);
    }

    [Button]
    public void BoostSpeed(float speed = 1f)
    {
        _currentSpeedX += speed;
    }

    [Button]
    public void BoostJumpPower()
    {
        jumpPower += jumpBoostAmount;
    }
    public void BoostJumpPower(float amount)
    {
        jumpPower += amount;
    }

    public void SetSpecialActionDoubleJump()
    {
        ChangeDoubleJumpState(DoubleJumpState.Available);
    }

    public void StartDash()
    {
        isDashing = true;
    }

    public void EndDash()
    {
        isDashing = false;
    }

    //---------------------------------------------------------------------------------
    //                               以下、private関数
    //---------------------------------------------------------------------------------

    //速度設定　自動で走るために毎フレーム呼ぶ
    private void ApplyVelocity()
    {
        //x方向の速度設定
        Rb.linearVelocityX = _currentSpeedX;

        if (isDashing)
        {
            Rb.linearVelocityX *= dashPower;
        }

        //y方向の速度の下限設定
        Rb.linearVelocityY = Mathf.Max(Rb.linearVelocityY, -maxFallSpeed);
    }

    private void ApplyJumpVelocity()
    {
        Rb.linearVelocityY = jumpPower;
        _isJumping.Value = true;
    }

    //FixedUpdateから呼ぶ
    private void UpdateJumpState(bool isGrounded)
    {
        switch (_jumpState)
        {
            case JumpState.Grounded:
                //落ちたらfallingへ
                if (!isGrounded)
                {
                    ChangeJumpState(JumpState.Falling);
                }
                break;

            case JumpState.Boosting:
                //時間経過でRisingへ
                jumpBoostingSec += Time.fixedDeltaTime;
                if (jumpBoostingSec > maxJumpSec)
                {
                    ChangeJumpState(JumpState.Rising);
                }
                break;

            case JumpState.Rising:
                //Y速度がマイナスになったらfallingへ
                if (SpeedY < 0f)
                {
                    ChangeJumpState(JumpState.Falling);
                }
                break;

            case JumpState.Falling:
                //着地したらgroundedへ
                if (isGrounded)
                {
                    ChangeJumpState(JumpState.Grounded);
                }
                break;
        }
    }

    private void ChangeJumpState(JumpState nextState)
    {
        _jumpState = nextState;

        //boosting以外へ移行するときはjumpBoostingSecをリセット
        if (nextState != JumpState.Boosting)
            jumpBoostingSec = 0f;

        //fallingヘするときは踏み判定をオン、それ以外はオフ
        stompHitBox.SetActive(nextState == JumpState.Falling);

        //ダブルジャンプは着地すると復活
        if(doubleJumpState == DoubleJumpState.Consumed && nextState == JumpState.Grounded)
        {
            ChangeDoubleJumpState(DoubleJumpState.Available);
        }

        //Boostingに移行するときにSE
        if(nextState == JumpState.Boosting)
        {
            AudioManager.Instance.PlayOneShotSe("Jump");
        }

        //ダブルジャンプするときのSEはここではなくStartDoubleJump()で
    }

    private void ChangeDoubleJumpState(DoubleJumpState nextState)
    {
        doubleJumpState = nextState;
    }

    private bool CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            origin: groundCheck.transform.position,
            size: new Vector2(1f, 0.1f),
            angle: 0,
            direction: Vector2.down,
            distance: checkRadius,
            layerMask: jumpableLayer);

        return hit.collider != null;
    }

    private void StartDoubleJump()
    {
        ChangeDoubleJumpState(DoubleJumpState.Consumed);
        Rb.linearVelocityY = doubleJumpPower;
        _isDoubleJumping.Value = true;

        ChangeJumpState(JumpState.Rising);

        AudioManager.Instance.PlayOneShotSe("DoubleJump");
    }

    private void StopMovement()
    {
        Rb.linearVelocity = Vector2.zero;
        Rb.angularVelocity = 0f;
        Rb.simulated = false;
    }
}
