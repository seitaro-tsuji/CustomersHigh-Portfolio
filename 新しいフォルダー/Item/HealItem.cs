using UnityEngine;

public class HealItem : CollectableObject
{
    public enum ItemType
    {
        Heal,
        BoostMaxHp
    }

    [SerializeField] private int healAmount = 1;
    [SerializeField] private ItemType type = ItemType.Heal;
    protected override void OnCollected(PlayerInteraction player)
    {
        if (type == ItemType.Heal)
        {
            player.GetHealItem(healAmount);
        }
        else if (type == ItemType.BoostMaxHp)
        {
            //player.GetMaxHpBoost(healAmount);
            Debug.LogWarning("MaxHPBoostÇÕBoostItemÇ≈èàóùÇ∑ÇÈÇÊÇ§Ç…ïœçXÇµÇ‹ÇµÇΩÅB");
        }
        AudioManager.Instance.PlayOneShotSe("GetGem");
    }
}
