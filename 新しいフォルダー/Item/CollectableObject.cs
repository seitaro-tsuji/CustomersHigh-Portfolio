//コインやアイテムなど、回収できるアイテム共通の基底クラス

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class CollectableObject : MonoBehaviour
{
    private bool isCollected;

    protected virtual void Awake()
    {
        isCollected = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCollected)
            return;

        if (!collision.TryGetComponent<PlayerInteraction>(out var player))
            return;

        OnPlayerTouch(player);
    }

    public void OnPlayerTouch(PlayerInteraction player)
    {
        //プレイヤーが触れた時の処理
        isCollected = true;
        OnCollected(player);
        Destroy(gameObject);
    }

    //回収したときの処理
    protected abstract void OnCollected(PlayerInteraction player);
}
