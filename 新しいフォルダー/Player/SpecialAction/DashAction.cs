using R3;
using UnityEngine;

public class DashAction : ISpecialAction
{
    private readonly PlayerMovement _playerMovement;

    private bool _isDashing = false;
    public bool IsDashing => _isDashing;

    //以下ゲージ関係
    private float _dashGaugeMax;
    public float DashGaugeMax => _dashGaugeMax;
    private ReactiveProperty<float> _dashGauge;
    public ReadOnlyReactiveProperty<float> DashGauge => _dashGauge;

    public DashAction(PlayerMovement playerMovement, float dashGaugeMax)
    {
        this._playerMovement = playerMovement;
        this._dashGaugeMax = dashGaugeMax;
        this._dashGauge = new ReactiveProperty<float>(dashGaugeMax);
    }

    public void OnPressed()
    {
        Debug.Log("ダッシュ開始");
        _playerMovement.StartDash();
        _isDashing = true;

    }

    public void OnHeld()
    {
        
    }

    public void OnReleased()
    {
        StopDash();
    }

    //外のUpdate関数で呼び出す
    public void Tick(float deltaTime)
    {
        //ゲージ更新
        if (_isDashing)
        {
            _dashGauge.Value -= deltaTime;
        }
        else
        {
            _dashGauge.Value = Mathf.Min(_dashGauge.Value + deltaTime, _dashGaugeMax);
        }

        //ゲージがなくなれば勝手に解除する
        if(_dashGauge.Value <= 0f)
        {
            _dashGauge.Value = 0f;

            Debug.Log("ダッシュゲージ枯渇");
            StopDash();
        }
    }

    private void StopDash()
    {
        Debug.Log("ダッシュ終了");
        _playerMovement.EndDash();
        _isDashing = false;
    }
}
