using R3;
using UnityEngine;
using UnityEngine.UI;


//注意：Fillオブジェクトにアタッチする
[RequireComponent(typeof(Image))]
public class DashGaugeView : MonoBehaviour
{
    [SerializeField] private PlayerSpecialAction _playerSpecialAction;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private float _maxHeight = 300f;

    private Image _image;
    private CompositeDisposable _disposables;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        _disposables = new CompositeDisposable();

        var dashAction = _playerSpecialAction.GetDashAction();
        if(dashAction == null)
        {
            Debug.LogWarning("DashActionがnullです。");
            return;
        }

        //ゲージのfill amountを更新
        dashAction.DashGauge
            .Select(value => Mathf.Clamp01(value / dashAction.DashGaugeMax) )
            .Subscribe(normalizedValue =>
            {
                SetHeight(normalizedValue * _maxHeight);
            })
            .AddTo(_disposables);
    }

    private void OnDisable()
    {
        _disposables?.Dispose();
        _disposables = null;
    }

    private void SetHeight(float height)
    {
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}
