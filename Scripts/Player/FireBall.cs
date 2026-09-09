using NaughtyAttributes;
using R3;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private bool setSpeedOnManual = false;
    [SerializeField, HideIf(nameof(setSpeedOnManual))] private float speedOffset;
    [SerializeField, ShowIf(nameof(setSpeedOnManual))] private float speed;

    [SerializeField, TextArea(3, 10)] private string memo;

    //今何個画面内にfire ballがあるか
    static private ReactiveProperty<int> _count = new(0);
    static public ReadOnlyReactiveProperty<int> Count => _count;

    private float _speedX;

    //生成時に呼ぶ
    public void Initialize(float playerSpeed)
    {
        //各種設定
        _speedX = setSpeedOnManual ? speed : playerSpeed + speedOffset;
        AudioManager.Instance.PlayOneShotSe("FireBall");
        _count.Value++;

        Debug.Log($"FireBall{_count.Value}個目生成");
    }

    void FixedUpdate()
    {
        transform.position += Vector3.right * _speedX * Time.fixedDeltaTime;
    }


    //プレイヤー以外の何かに当たったら消滅する
    private void OnTriggerExit2D(Collider2D collision)
    {
        //プレイヤーは無視
        if (collision.CompareTag("Player"))
        {
            return;
        }
        //敵の踏みつけ判定も無視する
        if (collision.CompareTag("EnemyHead"))
        {
            return;
        }

        Debug.Log($"fire ballが{collision.gameObject.name}に当たって消滅");
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _count.Value--;
    }

    static public int GetCount()
    {
        return _count.Value;
    }
}
