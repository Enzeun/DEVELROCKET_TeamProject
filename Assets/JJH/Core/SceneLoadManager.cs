using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
        // progress 값을 0과 1 사이로 안전하게 제한 (오버플로우 방지)
        progress = Mathf.Clamp01(progress);

        // startColor에서 endColor까지 progress(0~1) 비율로 보간
        Color blendedColor = Color.Lerp(startColor, endColor, progress);

        // 대상 컴포넌트에 색상 적용
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

        float elapsed = 0f;
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
            elapsed += Time.deltaTime;
            if (loadingBar != null)
            {
                Debug.Log(asyncLoad.progress);
                float value = Mathf.Clamp(asyncLoad.progress, 0 , 0.9f);
                SetColorByProgress(value);
                loadingValueText.text = Mathf.Round(value * 100) + "%";
                loadingBar.fillAmount = value;
                loadingBar2.fillAmount = value;
            }
        }

        // 씬이 준비된 후에도 진행 바가 멈추지 않도록 보완
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
        }

        // 로딩바 100% 채우기
        if (loadingBar != null) loadingBar.fillAmount = 1;

        // 로딩 완료 후 페이드 아웃 (투명 → 검정)
        fade.gameObject.SetActive(true);

        yield return fade.DOFade(1, fadeDuration).WaitForCompletion();

        // 씬 활성화
        asyncLoad.allowSceneActivation = true;
    }
}
