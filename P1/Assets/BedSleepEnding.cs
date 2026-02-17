using System.Collections;
using TMPro;
using UnityEngine;

public class BedSleepEnding : MonoBehaviour
{
    [Header("Prompt (uses existing PromptUI)")]
    [SerializeField] private string prompt = "Press Q to sleep";

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeGroup; // 拖 FadePanel 的 CanvasGroup
    [SerializeField] private float fadeDuration = 1.2f;

    [Header("Ending text (NEW TMP text)")]
    [SerializeField] private GameObject endTextObject; // 拖 EndText (整个物体)
    [SerializeField] private TMP_Text endText;         // 拖 EndText 上的 TMP
    [TextArea(2, 4)]
    [SerializeField] private string endLine = "Good night.";

    [Header("Alarm SFX (played after fade to black)")]
    [SerializeField] private AudioSource audioSource;  // 可放在床trigger上或别处
    [SerializeField] private AudioClip alarmClip;
    [Range(0f, 1f)] [SerializeField] private float alarmVolume = 1f;

    [Header("Disable player control scripts (drag your movement/look scripts here)")]
    [SerializeField] private Behaviour[] controlScriptsToDisable;

    [Header("Optional: end game")]
    [SerializeField] private bool quitApplicationAfterEnding = false; // 打包后可退出
    [SerializeField] private float quitDelay = 2.0f;

    private bool inside;
    private bool endingStarted;

    private void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (fadeGroup != null) fadeGroup.alpha = 0f;

        if (endTextObject != null) endTextObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (endingStarted) return;
        if (!other.CompareTag("Player")) return;

        inside = true;
        PromptUI.Instance?.Show(prompt);
    }

    private void OnTriggerExit(Collider other)
    {
        if (endingStarted) return;
        if (!other.CompareTag("Player")) return;

        inside = false;
        PromptUI.Instance?.Hide();
    }

    private void Update()
    {
        if (endingStarted) return;
        if (!inside) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(EndingRoutine());
        }
    }

    private IEnumerator EndingRoutine()
    {
        endingStarted = true;
        PromptUI.Instance?.Hide();

        // 1) 禁用玩家操作
        SetControlsEnabled(false);

        // 2) 黑屏渐变
        if (fadeGroup != null)
        {
            yield return FadeTo(1f, fadeDuration);
        }

        // 3) 黑屏后闹钟音效
        if (audioSource != null && alarmClip != null)
        {
            audioSource.PlayOneShot(alarmClip, alarmVolume);
        }

        // 4) 显示结局文字（新的Text）
        if (endTextObject != null) endTextObject.SetActive(true);
        if (endText != null) endText.text = endLine;

        // 可选：鼠标显示出来（如果你是FPS）
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 5) 可选：退出游戏（Build里有效，编辑器里不会真的退出）
        if (quitApplicationAfterEnding)
        {
            yield return new WaitForSeconds(quitDelay);
            Application.Quit();
        }
    }

    private void SetControlsEnabled(bool enabled)
    {
        if (controlScriptsToDisable == null) return;
        foreach (var b in controlScriptsToDisable)
        {
            if (b != null) b.enabled = enabled;
        }
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float start = fadeGroup.alpha;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, duration);
            fadeGroup.alpha = Mathf.Lerp(start, targetAlpha, t);
            yield return null;
        }

        fadeGroup.alpha = targetAlpha;
    }
}
