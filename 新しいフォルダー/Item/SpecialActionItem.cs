using UnityEngine;

public enum SpecialActionType
{
    DoubleJump,
    Dash,
    FireBall,
    AllItemCollect,
    None
}

public class SpecialActionItem : CollectableObject
{
    

    [SerializeField] private SpecialActionType actionType;

    //‰ñû‚µ‚½‚Æ‚«‚Ìˆ—
    protected override void OnCollected(PlayerInteraction player)
    {
        player.GetSpecialActionItem(actionType);
        AudioManager.Instance.PlayMultipleShotSe("GetGem", loopNum: 3, intervalRate: 0.25f);
    }
}
