using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private bool isReplayButton = false;
    [SerializeField] private bool isNextStageButton = false;

    //ã2ŒÂ‚ª‚Ç‚Á‚¿‚©true‚È‚ç‰º‚ð•\Ž¦‚µ‚È‚¢
    private bool ShouldHideNextScene => (isReplayButton || isNextStageButton);

    [SerializeField, HideIf(nameof(ShouldHideNextScene))] private SceneType  nextScene;

    protected SceneType NextScene => nextScene;

    protected virtual void Awake()
    {
        if (isReplayButton)
            GetComponent<Button>().onClick.AddListener(ReloadCurrentScene);
        else if(isNextStageButton)
            GetComponent<Button>().onClick.AddListener(LoadNextStage);
        else
            GetComponent<Button>().onClick.AddListener(LoadScene);
    }

    private void LoadScene()
    {
        GameManager.Instance.ChangeScene(nextScene);
    }

    private void ReloadCurrentScene()
    {
        GameManager.Instance.ReloadCurrentScene();
    }

    private void LoadNextStage()
    {
        GameManager.Instance.LoadNextStage();
    }
}
