using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public class EnemyHurtBox : MonoBehaviour
{
    Enemy _enemy;

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
        if(_enemy == null)
        {
            Debug.LogError("親オブジェクトにenemyがありません。");
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //プレイヤーの踏みつけに死亡判定はない
        if (collision.CompareTag("PlayerStomp"))
        {
            return;
        }

        //死亡判定
        _enemy.Die();
        Debug.Log("敵に攻撃を当てた");
    }
}
