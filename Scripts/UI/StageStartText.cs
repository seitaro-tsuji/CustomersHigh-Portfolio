using DG.Tweening;
using TMPro;
using UnityEngine;

public class StageStartText : MonoBehaviour
{
    [SerializeField] private float _textShowInterval = 0.5f;    //文字を読ませる時間
    [SerializeField] private float _textJumpInChange = 100f;    //文字入れ替えの時にどれだけ跳ねるか

    private TextMeshProUGUI _text;
    private RectTransform _rectTransform;
    private Sequence _startStageSequence;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void PlayTextAnimation(string text1, string text2)
    {
        Vector2 targetAnchoredPos = new Vector2(0, 0);
        _startStageSequence = DOTween.Sequence();

        //テキストを変更
        _text.text = text1;

        //画面中央まで落下
        _startStageSequence.Append(_rectTransform
            .DOAnchorPos(targetAnchoredPos, duration: 0.5f)
            .SetEase(Ease.OutBounce)
            );

        //0.5秒待つ
        _startStageSequence.AppendInterval(_textShowInterval);

        //少し上に上がってテキストを変更する
        _startStageSequence.Append(_rectTransform
            .DOAnchorPosY(_textJumpInChange, duration: 0.2f)
            );
        _startStageSequence.AppendCallback(() =>
        {
            _text.text = text2;
        });
        _startStageSequence.Append(_rectTransform
            .DOAnchorPosY(0f, duration: 0.2f)
            );

        //0.5秒待つ
        _startStageSequence.AppendInterval(_textShowInterval);

        //拡大しながらフェードアウトする
        Vector3 targetScale = new Vector3(10f, 10f, 10f);
        _startStageSequence.Append(_rectTransform
            .DOScale(targetScale, 0.5f)
            );
        _startStageSequence.Join(_text
            .DOFade(0f, duration: 0.5f)
            );

        //削除する
        _startStageSequence.AppendCallback(() =>
        {
            Destroy(gameObject);
        });
    }

    private void OnDestroy()
    {
        _startStageSequence?.Kill();
    }
}
