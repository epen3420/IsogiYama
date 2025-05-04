using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private GameObject loadPanel;


    public void LoadNextScene(string nextScene)
    {
        loadPanel.SetActive(true);

        LoadScene(nextScene).Forget();
    }

    private async UniTask LoadScene(string scene)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);
        while (!async.isDone)
        {
            await UniTask.Yield();
        }
    }
}
