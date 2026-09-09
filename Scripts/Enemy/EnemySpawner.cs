using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;

    private bool spawned;       //スポーン済みかどうか

    private void Awake()
    {
        //開始時に見た目を消す
        GetComponent<SpriteRenderer>().enabled = false;

        spawned = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //カメラ右側コライダーを検知したら1回だけ発動する
        if (spawned)
            return;
        if (!collision.CompareTag("CameraRight"))
            return;

        Spawn();
    }

    public void Spawn()
    {
        spawned = true;
        Instantiate(enemyPrefab, transform.position, transform.rotation);
    }
}
