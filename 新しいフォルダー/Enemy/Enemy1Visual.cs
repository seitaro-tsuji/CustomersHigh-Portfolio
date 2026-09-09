using UnityEngine;
using R3;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy1Visual : MonoBehaviour
{
    [SerializeField] private Sprite movingSprite;
    [SerializeField] private Sprite stoppingSprite;

    [SerializeField] private Enemy enemy;

    private SpriteRenderer spriteRenderer;
    private Animator _animator;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        //ismoving‚É‚æ‚Á‚Ä‰æ‘œ‚ð•Ï‚¦‚é
        enemy.IsMoving
            .Subscribe(value =>
            {
                _animator.SetBool("IsMoving", value);
            })
            .AddTo(this);
    }

    public void DieAnimationStart()
    {
        _animator.SetTrigger("Die");
    }

    public void DieAnimationEnd()
    {
        enemy.OnDieAnimationEnd();
    }
}
