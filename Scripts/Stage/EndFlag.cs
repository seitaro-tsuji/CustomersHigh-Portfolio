using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EndFlag : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerController>(out PlayerController playerController))
            return;

        StageManager.Instance.StageClear();
    }
}
