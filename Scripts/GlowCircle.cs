using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GlowCircle : MonoBehaviour
{
    private SpriteRenderer glowRenderer;

    private Tween glowTween;

    private void Awake()
    {
        glowRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        //ìßâﬂìxÇÃèâä˙ílÇ0.25Ç…Ç∑ÇÈ
        Color color = glowRenderer.color;
        color.a = 0.25f;
        glowRenderer.color = color;

        glowTween = glowRenderer
            .DOFade(endValue: 1.0f, duration: 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void OnDisable()
    {
        glowTween?.Kill();
        glowTween = null;
    }
}
