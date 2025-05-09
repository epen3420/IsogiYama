using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class SceneLoader : SceneSingleton<SceneLoader>
{
    public void LoadNextScene(string nextScene)
    {
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
