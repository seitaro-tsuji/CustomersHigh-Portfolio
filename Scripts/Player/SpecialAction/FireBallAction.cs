using UnityEngine;

public class FireBallAction:ISpecialAction
{
    PlayerSpecialAction _playerSpecialAction;

    public FireBallAction(PlayerSpecialAction playerSpecialAction)
    {
        _playerSpecialAction = playerSpecialAction;
    }

    public void Tick(float deltaTime)
    {

    }

    public void OnPressed()
    {
        if (FireBall.GetCount() < 2)
            _playerSpecialAction.CreateFireBall();
        else
        {
            Debug.Log("Šù‚É‰æ–Ê“à‚ÉFireBall‚ª2ŒÂ‚ ‚è‚Ü‚·B");
        }
    }

    public void OnHeld()
    {

    }

    public void OnReleased()
    {

    }
}
