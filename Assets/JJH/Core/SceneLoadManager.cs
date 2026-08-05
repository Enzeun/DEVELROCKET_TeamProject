using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using SF = UnityEngine.SerializeField;
public class SceneLoadManager : MonoBehaviour
{
    [SF] private GameObject LoadingGroup;
    [SF] private Image loadingBar;
    [SF] private Image loadingBar2;
    [SF] private TextMeshProUGUI loadingValueText;
    [SF] private CanvasGroup fade;
    [SF] private Color startColor;
    [SF] private Color endColor;

    private readonly float fadeDuration = 1.5f; // 페이드 인/아웃 시간
    private readonly float minLoadingTime = 2.0f; // 최소 로딩 시간 보장

    void Start()
    {
        StartCoroutine(nameof(LoadSceneAsyncWithDelay)); // 최소 2초 로딩 보장
    }

    public void SetColorByProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);

        Color blendedColor = Color.Lerp(startColor, endColor, progress);

        if (loadingBar != null)
        {
            loadingBar.color = blendedColor;
        }
    }

    private IEnumerator LoadSceneAsyncWithDelay()
    {
        fade.alpha = 1;
        fade.gameObject.SetActive(true);
        // 페이드 인 (검정 → 투명)
        yield return fade.DOFade(0, fadeDuration).WaitForCompletion();
        fade.gameObject.SetActive(false);

        // 비동기 씬 로드 시작
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(GameStateManager.Instance.LoadSceneName);
        asyncLoad.allowSceneActivation = false;

        float targetProgress = 0;
        float currentDisplayProgress = 0;
        float elapsed = 0f;

        while (asyncLoad.progress < 0.9f || elapsed < minLoadingTime)
        {
            yield return null;
            elapsed += Time.deltaTime;

            float realProgress = asyncLoad.progress >= 0.9f ? 1f : asyncLoad.progress;
            float timeProgress = Mathf.Clamp01(elapsed / minLoadingTime);

            targetProgress = Mathf.Min(realProgress, timeProgress);
            currentDisplayProgress = 
                Mathf.MoveTowards(currentDisplayProgress,targetProgress, Time.deltaTime * 2f);

            if (loadingBar != null)
            {
                SetColorByProgress(targetProgress);
                loadingValueText.text = $"{Mathf.Round(targetProgress * 100)}%";
                loadingBar.fillAmount = targetProgress;
                loadingBar2.fillAmount = targetProgress;
            }
        }

        /*// 씬이 준비된 후에도 진행 바가 멈추지 않도록 보완
        while (elapsed < minLoadingTime)
        {
            yield return null;
            elapsed += Time.deltaTime;
            if (loadingBar != null)
            {
                Debug.Log(elapsed);
                float value = Mathf.Lerp(loadingBar.fillAmount, 1f, elapsed / minLoadingTime);
                SetColorByProgress(value);
                loadingValueText.text = Mathf.Round(value * 100) + "%";
                loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, 1f, elapsed / minLoadingTime);
                loadingBar2.fillAmount = Mathf.Lerp(loadingBar2.fillAmount, 1f, elapsed / minLoadingTime);
            }
        }*/

        // 로딩바 100% 채우기
        if (loadingBar != null)
        {
            SetColorByProgress(1);
            loadingValueText.text = "100%";
            loadingBar.fillAmount = 1;
            loadingBar2.fillAmount = 1;
        }


        // 로딩 완료 후 페이드 아웃 (투명 → 검정)
        fade.gameObject.SetActive(true);

        yield return fade.DOFade(1, fadeDuration).WaitForCompletion();

        // 씬 활성화
        asyncLoad.allowSceneActivation = true;
    }
}
