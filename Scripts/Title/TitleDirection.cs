using DG.Tweening;
using NaughtyAttributes;
using R3;
using TMPro;
using UnityEngine;

public class TitleDirection : MonoBehaviour
{
    [BoxGroup("Text"), SerializeField] private RectTransform _titleTexts;
    [BoxGroup("Text"), SerializeField] private TextMeshProUGUI _runnText;     // "Runn"
    [BoxGroup("Text"), SerializeField] private TextMeshProUGUI _ersText;      // "er's"
    [BoxGroup("Text"), SerializeField] private TextMeshProUGUI _highText;     // "High"
    [BoxGroup("Text"), SerializeField] private TextMeshProUGUI _customText;   // "Custom"
    [BoxGroup("Text"), SerializeField] private TextMeshProUGUI _japaneseText;   // "カスタマーズ・ハイ"

    [BoxGroup("Button"), SerializeField] private Transform _stageButtonsParent;

    [BoxGroup("Background"), SerializeField] private RectTransform _backGround;

    [BoxGroup("Direction Parameter"), SerializeField] private float _interval1 = 0.5f;
    [BoxGroup("Direction Parameter"), SerializeField] private float _interval2 = 0.5f;
    [BoxGroup("Direction Parameter"), SerializeField] private float _colorChangeInterval = 1f;
    [BoxGroup("Direction Parameter"), SerializeField] private float _cyanMoveX = 400f;
    [BoxGroup("Direction Parameter"), SerializeField] private int _cyanLoopTimes = 6;
    [BoxGroup("Direction Parameter"), SerializeField] private float _orangeMoveY = 200f;
    [BoxGroup("Direction Parameter"), SerializeField] private float _redScale = 1.3f;
    [BoxGroup("Direction Parameter"), SerializeField] private int _redLoopTimes = 5;

    Sequence _startSequence;
    Sequence _customTextSequence;

    private void Start()
    {
        StartAnimation();
    }

    private void StartAnimation()
    {
        _startSequence = DOTween.Sequence();
        _startSequence.AppendInterval(GameManager.Instance.GetFadeInSec());    //フェードインを待つ

        TextMeshProUGUI[] texts = { _runnText, _ersText, _highText };

        //Button非表示
        _stageButtonsParent.gameObject.SetActive(false);

        //日本語非表示(透過度0に)
        Color c = _japaneseText.color;
        c.a = 0f;
        _japaneseText.color = c;

        //はじめにテキスト"Runner's High"を画面よりも左に置いておく
        _startSequence.AppendCallback(() => { });
        foreach (var text in texts)
        {
            //目標値を置いて左に移動
            float targetLocalPosX = text.transform.localPosition.x;
            text.transform.localPosition += Vector3.left * 2400f;

            //画面内までの移動開始
            _startSequence.Join(text.transform.DOLocalMoveX(targetLocalPosX, duration: 0.5f).SetEase(Ease.OutBack));
        }

        //ちょっと間をおいて上から"Custom"が落ちてきて"Runn"と入れ替わる
        float customTargetLocalPosY = _runnText.transform.localPosition.y;

        _startSequence.AppendInterval(_interval1);

        //"Custom"が落ちてくる
        _startSequence.Append(_customText.transform
            .DOLocalMoveY(customTargetLocalPosY, duration: 0.5f)
            .SetEase(Ease.InCubic)
            );
        //"Runn"が入れ替わりで落ちる
        _startSequence.Append(_runnText.transform
            .DOLocalMoveY(customTargetLocalPosY - 1080f, duration: 0.5f)
            .SetEase(Ease.OutCubic)
            );
        //回転もする
        _startSequence.Join(_runnText.transform
            .DORotate(new Vector3(0,0,540f),duration:0.5f, RotateMode.FastBeyond360)
            );

        //少し待ってから、テキスト全体を上にずらす
        _startSequence.AppendInterval(_interval2);

        float targetY = _titleTexts.anchoredPosition.y + 200f;

        _startSequence.Append(_titleTexts
            .DOAnchorPosY(targetY, duration: 0.5f)
            .SetEase(Ease.Linear)
            );

        //ボタンを表示
        _startSequence.AppendCallback(() =>
        {
            _stageButtonsParent.gameObject.SetActive(true);
        });

        //0.5秒で日本語を表示
        _startSequence.Append(_japaneseText
            .DOFade(1f, duration: 0.5f)
            );

        //"Custom"部分のテキストを水色にしてから演出をループさせる
        _startSequence.Append(_customText
            .DOColor(Color.cyan, duration: 0.5f)
            .SetEase(Ease.Linear)
            );
        _startSequence.AppendCallback(() =>
        {
            CustomTextEffectStart();    //カスタムの文字色変更開始
            BackgroundMoveStart();      //背景も動かし始める
        });
    }

    private void CustomTextEffectStart()
    {
        _customTextSequence = DOTween.Sequence();
        RectTransform customRect = _customText.rectTransform;

        //水色状態でスタート
        //水色時の演出　左右に高速で動く
        float targetX = customRect.anchoredPosition.x + _cyanMoveX;
        _customTextSequence.Append(customRect
            .DOAnchorPosX(targetX, duration: _colorChangeInterval/(_cyanLoopTimes*2))
            .SetEase(Ease.InOutQuart)
            .SetLoops((_cyanLoopTimes * 2), LoopType.Yoyo)
            );

        //水色からオレンジ色に変更
        _customTextSequence.Append(_customText
            .DOColor(Color.orange, duration: 0.5f)
            .SetEase(Ease.Linear)
            );

        //オレンジ時の演出  上に跳ねる
        float targetY = customRect.anchoredPosition.y + _orangeMoveY;
        _customTextSequence.Append(customRect
            .DOAnchorPosY(targetY, duration: _colorChangeInterval)
            .SetEase(Ease.OutFlash, 6, 0)
            );

        //オレンジから赤色に変更
        _customTextSequence.Append(_customText
            .DOColor(Color.red, duration: 0.5f)
            .SetEase(Ease.Linear)
            );

        //赤色の演出　脈動する
        _customTextSequence.Append(_customText.transform
            .DOScale(_redScale, duration:_colorChangeInterval/(_redLoopTimes*2))
            .SetEase(Ease.InQuint)
            .SetLoops((_redLoopTimes * 2), LoopType.Yoyo)
            );


        //赤色から水色に変更
        _customTextSequence.Append(_customText
            .DOColor(Color.cyan, duration: 0.5f)
            .SetEase(Ease.Linear)
            );

        //ここまでを無限ループする
        _customTextSequence.SetLoops(-1, LoopType.Restart);
    }

    private void BackgroundMoveStart()
    {
        float targetX = _backGround.anchoredPosition.x - 1215f;  //1215は背景画像の横幅
        _backGround
            .DOAnchorPosX(targetX, duration: 1f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

    }

    private void OnDestroy()
    {
        _startSequence?.Kill();
        _customTextSequence?.Kill();
    }
}
