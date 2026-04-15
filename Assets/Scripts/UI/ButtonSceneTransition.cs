using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSceneTransition : AbstractActionButton
{
    [SerializeField] protected string scenename = default;

    protected override void OnClicked()
    {
        SceneManager.LoadSceneAsync(scenename);
    }
}
