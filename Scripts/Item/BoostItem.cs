using UnityEngine;

public class BoostItem : CollectableObject
{
    public enum BoostType
    {
        Speed,
        JumpPower,
        MaxHp
    }

    [SerializeField] private BoostType type;

    //‰ñû‚µ‚½‚Æ‚«‚Ìˆ—
    protected override void OnCollected(PlayerInteraction player)
    {
        player.GetBoostItem(type);
        AudioManager.Instance.PlayOneShotSe("GetGem");
        UIManager.Instance.StartGetBoostItemEffect(type, transform.position);
    }
}
