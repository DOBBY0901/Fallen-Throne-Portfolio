using UnityEngine.SceneManagement;

public static class LoadingSceneController
{
    private const string LoadingSceneName = "Loading";

    public static string NextSceneName { get; private set; }

    public static void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return;

        NextSceneName = sceneName;
        SceneManager.LoadScene(LoadingSceneName);
    }
}
