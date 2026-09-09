using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private AudioClip _bgmClip;

    static public TitleManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        AudioManager.Instance.StartPlayingBgm(_bgmClip);
    }
}
