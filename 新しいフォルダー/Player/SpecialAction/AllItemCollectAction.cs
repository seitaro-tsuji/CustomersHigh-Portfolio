using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AllItemCollectAction:ISpecialAction
{
    private PlayerInteraction _playerInteraction;
    private Camera _targetCamera;
    private LayerMask _itemLayer;
    private SpecialActionButton _specialActionButton;

    public AllItemCollectAction(PlayerInteraction playerInteraction,
         Camera targetCamera,
         LayerMask itemLayer,
         SpecialActionButton specialActionButton)
    {
        _playerInteraction = playerInteraction;
        _targetCamera = targetCamera;
        _itemLayer = itemLayer;
        _specialActionButton = specialActionButton;
    }

    public void OnPressed()
    {
        StartAllItemCollectAsync().Forget();
    }

    private async UniTask StartAllItemCollectAsync()
    {
        float collectDirectionDuration = 0.5f;//アイテムがプレイヤーまで飛んでくる時間

        //画面全体のアイテムを取得
        Vector2 bottomLeft = _targetCamera.ViewportToWorldPoint(Vector2.zero);
        Vector2 topLeft = _targetCamera.ViewportToWorldPoint(Vector2.one);
        Collider2D[] hits = Physics2D.OverlapAreaAll(bottomLeft, topLeft, _itemLayer);

        int coinAmount = 0; //ゲットしたコイン枚数

        //アイテム全部を取得
        foreach (Collider2D hit in hits)
        {
            CollectableObject collectableObject = hit.GetComponent<CollectableObject>();
            if (collectableObject != null)
            {
                //コインの場合
                if (collectableObject is Coin coin)
                {
                    coinAmount += coin.Amount;  //ゲットした枚数を足す
                    coin.SpecialCollect();      //特殊アクションでゲットしたことを通知
                }

                UIManager.Instance.StartAllCollectDirection(collectableObject, collectDirectionDuration);
            }
        }

        //1回しか押せない
        _specialActionButton.Disable();

        //0.5秒(エフェクト終了時間)待ってゲットしたコイン枚数の表示
        await UniTask.Delay(TimeSpan.FromSeconds(collectDirectionDuration));
        UIManager.Instance.PlayGetCoinsDirection(coinAmount);
    }

    public void OnReleased() { }

    public void OnHeld() { }

    public void Tick(float deltaTime) { }
}
