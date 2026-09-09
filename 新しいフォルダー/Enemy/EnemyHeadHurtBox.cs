using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyHeadHurtBox : MonoBehaviour
{
    Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerEnemyInteraction playerEnemyInteraction = collision.GetComponentInParent<PlayerEnemyInteraction>();
        if (playerEnemyInteraction == null)
        {
            Debug.Log("プレイヤーではないものにhead hurt boxが接触");
            return;
        }
        if(enemy == null)
        {
            Debug.Log("親オブジェクトからenemyスクリプトが見つかりません");
            return;
        }

        playerEnemyInteraction.RequestStomp();
        enemy.OnStompedTask().Forget();
    }

}
