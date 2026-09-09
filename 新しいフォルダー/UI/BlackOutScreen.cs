using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BlackOutScreen : MonoBehaviour
{
    private Image image;

    private float Alpha
    {
        get => image.color.a;
        set
        {
            Color c = image.color;
            c.a = Mathf.Clamp01(value);
            image.color = c;
        }
    }

    private void Awake()
    {
        image = GetComponent<Image>();
    }


    //fixme  キャンセル対応を後で付け足す
    public async UniTask FadeInOrOutAsync(float targetAlpha, float duration=1f)
    {
        float initialAlpha = Alpha;
        float elapsedSec = 0f;

        //毎フレーム一回繰り返し　目標値まで少しずつ透過度を変化させる
        while (elapsedSec < duration)
        {
            elapsedSec += Time.deltaTime;
            Alpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsedSec/duration);

            await UniTask.Yield();
        }

        Alpha = targetAlpha;
    }

    public UniTask FadeOutAsync(float duration = 1f) => FadeInOrOutAsync(1f, duration);
    public UniTask FadeInAsync(float duration = 1f) => FadeInOrOutAsync(0f, duration);

    public void SetAlpha(float alpha)
    {
        Alpha = alpha;
    }
}
