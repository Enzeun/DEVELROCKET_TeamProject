using DG.Tweening;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

public class TitleManager : MonoBehaviour
{
    [Header("타이틀")]
    [SF] private Image titleImage;
    [SF] private Image titleImageBackground;
    [SF] private TextMeshProUGUI titleText;

    [Header("버튼 그룹")]
    [SF] private VerticalLayoutGroup btnGroup;

    [Header("측면 바")]
    [SF] private Image sideBar;
    [SF] private CanvasGroup sideBarCanvasGroup;

    [Header("시작 버튼")]
    [SF] private Button startBtn;
    [SF] private RectTransform startBtnRect;
    [SF] private CanvasGroup startBtnCanvasGroup;
    [SF] private TextMeshProUGUI startBtntext;

    [Header("종료 버튼")]
    [SF] private Button exitBtn;
    [SF] private RectTransform exitBtnRect;
    [SF] private CanvasGroup exitBtnCanvasGroup;
    [SF] private TextMeshProUGUI exitBtntext;

    [Header("기타")]
    [SF] private AnimationCurve barEase;
    public void Start()
    {
        StartCoroutine(nameof(StartEvent));
    }

    /*    public void Update()
        {
            if(Input.GetKeyDown(KeyCode.I)) StartCoroutine(nameof(StartEvent));
        }*/

    public IEnumerator StartEvent()
    {
        startBtn.interactable = false;
        exitBtn.interactable = false;

        // 상태 초기화
        startBtnCanvasGroup.gameObject.SetActive(false);
        exitBtnCanvasGroup.gameObject.SetActive(false);
        btnGroup.gameObject.SetActive(false);
        sideBar.gameObject.SetActive(false);

        // 초기 설정
        startBtnCanvasGroup.gameObject.SetActive(true);
        exitBtnCanvasGroup.gameObject.SetActive(true);
        sideBar.gameObject.SetActive(true);
        sideBarCanvasGroup.alpha = 0;
        startBtnCanvasGroup.alpha = 0;
        exitBtnCanvasGroup.alpha = 0;
        btnGroup.gameObject.SetActive(true);

        yield return null;
        btnGroup.enabled = false;

        Vector2 titleVertorSize = titleImage.rectTransform.sizeDelta;
        Vector2 titleBackgroundVertorSize = titleImageBackground.rectTransform.anchoredPosition;
        titleImage.rectTransform.sizeDelta = new(0, 2);

        Vector2 startVectorSave = startBtnRect.anchoredPosition;
        Vector2 exitVectorSave = exitBtnRect.anchoredPosition;
        Vector2 sideBarSize = sideBar.rectTransform.sizeDelta;

        sideBar.rectTransform.sizeDelta = new(sideBarSize.x, 0);
        startBtnRect.anchoredPosition = new(startVectorSave.x + 400, startVectorSave.y);
        exitBtnRect.anchoredPosition = new(exitVectorSave.x + 400, exitVectorSave.y);

        startBtntext.maxVisibleCharacters = 0;
        exitBtntext.maxVisibleCharacters = 0;
        titleText.maxVisibleCharacters = 0;

        float duration = 0.025f;
        float startTextDuration = duration * startBtntext.textInfo.characterCount;
        float exitTextDuration = duration * exitBtntext.textInfo.characterCount;
        float titleTextDuration = duration * titleText.textInfo.characterCount;

        float btnAnimationTime = 0.27f;
        float btnDelayTime = 0.08f;
        float btnCount = 2;

        float sideBarTime = btnAnimationTime + ((btnAnimationTime - btnDelayTime) * btnCount);

        var title = DOTween.Sequence();

        title.Join(titleImage.rectTransform.DOSizeDelta(new(titleVertorSize.x, 5), 0.2f));
        title.Append(titleImage.rectTransform.DOSizeDelta(titleVertorSize, 0.25f));
        title.Append(DOTween.To(
            () => titleText.maxVisibleCharacters,
            x => titleText.maxVisibleCharacters = x,
            titleText.textInfo.characterCount,
            titleTextDuration
        ).SetEase(Ease.Linear));

        yield return title.WaitForCompletion();

        var seq = DOTween.Sequence();

        seq.Join(sideBar.rectTransform.DOSizeDelta(sideBarSize, sideBarTime).SetEase(barEase));
        seq.Join(sideBarCanvasGroup.DOFade(1, sideBarTime).SetEase(barEase));

        float startBtnStartTime = 0f;
        seq.Insert(startBtnStartTime, startBtnRect.DOAnchorPosX(startVectorSave.x, btnAnimationTime).SetEase(Ease.Linear));
        seq.Insert(startBtnStartTime, startBtnCanvasGroup.DOFade(1, btnAnimationTime).SetEase(Ease.Linear));
        seq.Insert(btnAnimationTime, DOTween.To(
            () => startBtntext.maxVisibleCharacters,
            x => startBtntext.maxVisibleCharacters = x,
            startBtntext.textInfo.characterCount,
            startTextDuration
        ).SetEase(Ease.Linear));

        float exitBtnStartTime = btnDelayTime;
        seq.Insert(exitBtnStartTime, exitBtnRect.DOAnchorPosX(exitVectorSave.x, btnAnimationTime).SetEase(Ease.Linear));
        seq.Insert(exitBtnStartTime, exitBtnCanvasGroup.DOFade(1, btnAnimationTime).SetEase(Ease.Linear));
        seq.Insert(btnAnimationTime + exitBtnStartTime, DOTween.To(
            () => exitBtntext.maxVisibleCharacters,
            x => exitBtntext.maxVisibleCharacters = x,
            exitBtntext.textInfo.characterCount,
            exitTextDuration)
            .SetEase(Ease.Linear));

        seq.OnComplete(() => { 
            btnGroup.enabled = true;
            startBtn.interactable = true;
            exitBtn.interactable = true;
        });





    }

    public void StartGame()
    {
        GameStateManager.Instance.LoadSceneName = "JJH_TestScene";
        SceneManager.LoadScene("JJH_LoadingScene");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
