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
    [SF] private Color middleColor;
    [SF] private Color endColor;

    private readonly float fadeDuration = 1f; // 페이드 인/아웃 시간
    private readonly float minLoadingTime = 2.0f; // 최소 로딩 시간 보장

    void Start()
    {
        StartCoroutine(nameof(LoadSceneAsyncWithDelay)); // 최소 2초 로딩 보장
    }

    public void SetColorByProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);
        Color blendedColor = Color.white;
        if (progress < 0.5f)
            blendedColor = Color.Lerp(startColor, middleColor, progress * 2);
        else
            blendedColor = Color.Lerp(middleColor, endColor, Mathf.Max(progress - 0.5f, 0) * 2);

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
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync((int)GameStateManager.Instance.LoadSceneName);
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
