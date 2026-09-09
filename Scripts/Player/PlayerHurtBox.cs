using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerHurtBox : MonoBehaviour
{
    [SerializeField] private PlayerEnemyInteraction playerEnemyInteraction;
    [SerializeField] private PlayerStatus playerStatus;  

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerEnemyInteraction == null)
            return;
        if (playerStatus == null)
            return;

        //即死判定に触れた時
        if (collision.CompareTag("InstantDeath"))
        {
            playerStatus.InstantDeath();
            return;
        }

        //トゲならトゲダメージリクエスト
        if (collision.CompareTag("Needle"))
        {
            playerStatus.RequestNeedleDamage();
            return;
        }

        //どちらでもなければ普通のダメージリクエスト
        playerEnemyInteraction.RequestDamage();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //トゲに触れている間トゲダメージリクエスト
        if (collision.CompareTag("Needle"))
        {
            playerStatus.RequestNeedleDamage();
            return;
        }
    }
}
