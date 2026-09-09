using UnityEngine;

public class Coin : CollectableObject
{
    [SerializeField] private int amount = 1;
    public int Amount => amount;

    private bool _isSpecialCollect = false;     //特殊アクションAllCollectで入手したか？

    protected override void OnCollected(PlayerInteraction player)
    {
        player.GetCoin(amount);
        AudioManager.Instance.PlayOneShotSe("GetCoin");

        //大コインをゲットしたら枚数を演出で見せる
        if(amount >= 2 && !_isSpecialCollect)
        {
            UIManager.Instance.PlayGetCoinsDirection(amount);
        }
    }

    public void SpecialCollect()
    {
        _isSpecialCollect = true;
    }
}
