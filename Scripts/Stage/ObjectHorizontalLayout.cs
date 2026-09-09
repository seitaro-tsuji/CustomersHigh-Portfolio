using UnityEngine;

//子オブジェクトを横向きに自動レイアウトして並べるクラス　親オブジェクトに持たせる
public class ObjectHorizontalLayout : MonoBehaviour 
{
    [SerializeField] private float width = 5f;

    //子オブジェクトが生成された後に呼ぶ
    private void Start()
    {
        int childCount = transform.childCount;
        if (childCount <= 0)
            return;

        //必要な間隔の計算  数と大きさ
        int spaceCount = childCount + 1; 
        float space = width / spaceCount;

        //左端の座標を取得
        Vector3 leftEdge = transform.position - new Vector3(1, 0, 0) * width / 2;

        for(int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            //左端から間隔i+1個分の位置に配置する
            child.transform.position = leftEdge + new Vector3(space, 0, 0) * (i + 1);
        }
    }
}
