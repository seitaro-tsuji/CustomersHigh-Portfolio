using NaughtyAttributes;
using R3;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private ReactiveProperty<int> _currentHp = new();
    public ReadOnlyReactiveProperty<int> CurrentHp => _currentHp;

    [SerializeField] private int _initialMaxHp = 3;
    private ReactiveProperty<int> _maxHp = new();
    public ReadOnlyReactiveProperty<int> MaxHp => _maxHp;

    private ReactiveProperty<bool> _isDead = new(false);
    public ReadOnlyReactiveProperty<bool> IsDead => _isDead;

    [SerializeField] private float _needleDamageInterval = 1f;
    private float _needleDamageTimer;


    private void Awake()
    {
        _maxHp.Value = _initialMaxHp;
        _currentHp.Value = _maxHp.Value;
        _needleDamageTimer = 0f;
    }

    private void Update()
    {
        if (_needleDamageTimer > 0f)
        {
            _needleDamageTimer -= Time.deltaTime;
        }
    }

    [Button]
    public void Heal(int amount = 1) => SetCurrentHp(_currentHp.Value + amount);
    [Button]
    public void FullRestore() => SetCurrentHp(_maxHp.Value);
    [Button]
    public void Damage(int amount = 1)
    {
        SetCurrentHp(_currentHp.Value - amount);
        AudioManager.Instance.PlayOneShotSe("Damage");
        UIManager.Instance.PlayDamageEffect(transform.position, amount);
        StageManager.Instance.IncreaseDamageCount();
    }
    public void InstantDeath() => SetCurrentHp(0);
    [Button]
    public void IncreaseMaxHp(int amount = 1)
    {
        SetMaxHp(_maxHp.Value + amount);
        Heal(amount);//増えた分回復
    }
    [Button]
    public void DecreaseMaxHp(int amount = 1) => SetMaxHp(_maxHp.Value - amount);

    public void RequestNeedleDamage()
    {
        //タイマーが0以下ならダメージを受けて、タイマー初期化
        if(_needleDamageTimer <= 0f)
        {
            Damage(1);
            _needleDamageTimer = _needleDamageInterval;
        }
    }

    private void SetCurrentHp(int value)
    {
        _currentHp.Value = Mathf.Clamp(value, 0, _maxHp.Value);

        if (_currentHp.Value == 0)
        {
            OnDie();
        }
    }

    private void SetMaxHp(int value)
    {
        //最大Hpは0以下にならない
        _maxHp.Value = Mathf.Max(value, 1);

        //現在HPが超過すると補正
        if (_currentHp.Value > _maxHp.Value)
            _currentHp.Value = _maxHp.Value;
    }

    //死亡時の処理
    private void OnDie()
    {
        if (_isDead.Value)
            return;
        _isDead.Value = true;
        AudioManager.Instance.StopBgm();
        AudioManager.Instance.PlayOneShotSe("Die");
    }
}
