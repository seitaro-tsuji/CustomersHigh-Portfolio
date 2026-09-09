using DG.Tweening;
using UnityEngine;

//
//注意：これをアタッチする場合、オブジェクト自体に付けるのではなく、子オブジェクトvisualを作ってそっちに付ける方が安全
//
public class FloatMotion : MonoBehaviour
{
    [SerializeField] private float period = 1f;
    [SerializeField] private float amplitude = 0.3f;

    //todo 汎用化するときは1フレーム待つ場合の処理を追加し、インスペクタウィンドウで選べるようにするかも

    private void Awake()
    {
        //初期値を振動の中心にするために振幅分だけ振動開始地点を下げる
        transform.localPosition += Vector3.down * amplitude;

        this.transform.DOLocalMoveY(transform.localPosition.y + amplitude * 2, period / 2)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }
}
