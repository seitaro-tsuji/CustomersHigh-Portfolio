using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] private bool verticalTrack = false;
    [SerializeField] private bool horizontalTrack = true;

    void Start()
    {
        if(player == null)
        {
            Debug.LogWarning("playerが渡されていません。");
        }
    }

    void Update()
    {
        if (player == null)
            return;

        Vector3 pos = transform.position;   //現在位置

        //縦横それぞれ更新する
        if (verticalTrack)
        {
            pos.y = player.position.y;
        }
        if (horizontalTrack)
        {
            pos.x = player.position.x;
        }

        transform.position = pos;
    }
}
