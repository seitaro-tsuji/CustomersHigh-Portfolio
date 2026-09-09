using DG.Tweening;
using TMPro;
using UnityEngine;

public class GetCoinText : MonoBehaviour
{

    public void Initialize(int amount)
    {
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = $"+{amount}";

        StartDirection();
    }

    //浮上するエフェクト開始
    private void StartDirection()
    {
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(transform.DOMove(transform.position + Vector3.up * 100f, duration: 0.5f))
            .AppendCallback(() =>
            {
                Destroy(gameObject);
            });
    }
}
