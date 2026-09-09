using UnityEngine;

public class StageLeftCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerController>(out PlayerController _))
            return;

        StageManager.Instance.MainStageStart();
    }
}
