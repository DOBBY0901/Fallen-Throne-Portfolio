using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image progressFillImage;

    [Header("Progress Display")]
    [SerializeField] private float fillSpeed = 0.5f;
    [SerializeField] private float minimumLoadingTime = 0.5f;
    [SerializeField] private float activationDelay = 0.2f;

    private void Start()
    {
        StartCoroutine(LoadSceneRoutine());
    }

    private IEnumerator LoadSceneRoutine()
    {
        string sceneName =
            LoadingSceneController.NextSceneName;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError(
                "[LoadingSceneUI] Next scene name is empty.",
                this
            );

            yield break;
        }

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(sceneName);

        if (operation == null)
            yield break;

        operation.allowSceneActivation = false;

        float displayProgress = 0f;
        float elapsed = 0f;

        while (!operation.isDone)
        {
            elapsed += Time.unscaledDeltaTime;

            // Unity AsyncOperation progress reaches 0.9 before scene activation.
            float targetProgress =
                Mathf.Clamp01(operation.progress / 0.9f);

            displayProgress = Mathf.MoveTowards(
                displayProgress,
                targetProgress,
                fillSpeed * Time.unscaledDeltaTime
            );

            if (progressFillImage != null)
                progressFillImage.fillAmount = displayProgress;

            bool loadingComplete =
                operation.progress >= 0.9f;

            bool progressDisplayComplete =
                displayProgress >= 1f;

            bool minimumTimePassed =
                elapsed >= minimumLoadingTime;

            if (loadingComplete &&
                progressDisplayComplete &&
                minimumTimePassed)
            {
                if (activationDelay > 0f)
                {
                    yield return new WaitForSecondsRealtime(
                        activationDelay
                    );
                }

                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
