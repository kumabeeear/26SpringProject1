using System.Collections;
using UnityEngine;

public class CookingInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject cookedFood;      // 做出来的食物（开局关掉）
    [SerializeField] private GameObject doorCube;        // 吃完后要消失的门（Cube）
    [SerializeField] private AudioSource audioSource;    // 播放所有音效的 AudioSource（建议挂在trigger上）
    [SerializeField] private AudioClip cookSfx;          // 做饭音效
    [SerializeField] private AudioClip eatSfx;           // 吃饭音效
    [SerializeField] private AudioClip doorDisappearSfx; // 门消失音效

    [Header("Text")]
    [SerializeField] private string cookHint = "press C to make some food";
    [SerializeField] private string eatHint  = "E to eat";

    [Header("Eat result lines (left click to continue)")]
    [TextArea(2, 4)]
    [SerializeField] private string[] afterEatLines;

    [Header("Options")]
    [SerializeField] private bool oneTimeOnly = true; // 吃完后是否禁用整套交互

    [Header("Keys (for saving state)")]
    [SerializeField] private string doorGoneKey = "CookDoorGone"; // 记录门是否已消失

    [Header("Flip page mouse button: 0=left")]
    [SerializeField] private int mouseButton = 0;

    private bool playerInside;

    private enum State { WaitingToCook, Cooking, ReadyToEat, Eating, AfterEatDialogue, Done }
    private State state = State.WaitingToCook;

    private int dialogueIndex = 0;

    private void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        // 开局让食物隐藏
        if (cookedFood != null) cookedFood.SetActive(false);

        // 如果门之前被消失过，就保持消失（除非你开局 reset）
        if (doorCube != null)
        {
            bool gone = PlayerPrefs.GetInt(doorGoneKey, 0) == 1;
            doorCube.SetActive(!gone);
        }

        RefreshPrompt();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = true;
        RefreshPrompt();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;

        // 关键：如果正在字幕阶段，别把字幕关掉（玩家跑远也能继续看/翻页）
        if (state != State.AfterEatDialogue)
            PromptUI.Instance?.Hide();
    }

    private void Update()
    {
        // 关键：字幕阶段“全局翻页”，不依赖 playerInside
        if (state == State.AfterEatDialogue)
        {
            if (Input.GetMouseButtonDown(mouseButton))
                NextDialoguePage();
            return;
        }

        // 非字幕阶段：仍然需要在范围内才可交互
        if (!playerInside) return;

        if (state == State.WaitingToCook && Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(CookRoutine());
        }
        else if (state == State.ReadyToEat && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(EatRoutine());
        }
    }

    private void RefreshPrompt()
    {
        if (!playerInside) return;

        switch (state)
        {
            case State.WaitingToCook:
                PromptUI.Instance?.Show(cookHint);
                break;
            case State.ReadyToEat:
                PromptUI.Instance?.Show(eatHint);
                break;
            default:
                PromptUI.Instance?.Hide();
                break;
        }
    }

    private IEnumerator CookRoutine()
    {
        state = State.Cooking;
        PromptUI.Instance?.Hide();

        float wait = PlayAndGetLength(cookSfx);
        if (wait > 0f) yield return new WaitForSeconds(wait);

        if (cookedFood != null) cookedFood.SetActive(true);

        state = State.ReadyToEat;
        RefreshPrompt();
    }

    private IEnumerator EatRoutine()
    {
        state = State.Eating;
        PromptUI.Instance?.Hide();

        float waitEat = PlayAndGetLength(eatSfx);
        if (waitEat > 0f) yield return new WaitForSeconds(waitEat);

        if (cookedFood != null) cookedFood.SetActive(false);

        // 1) 门消失 + 门消失音效
        float waitDoor = PlayAndGetLength(doorDisappearSfx);
        if (doorCube != null) doorCube.SetActive(false);

        PlayerPrefs.SetInt(doorGoneKey, 1);
        PlayerPrefs.Save();

        if (waitDoor > 0f) yield return new WaitForSeconds(waitDoor);

        // 2) 吃完后的字幕翻页（复用 PromptUI）
        if (afterEatLines != null && afterEatLines.Length > 0)
        {
            state = State.AfterEatDialogue;
            dialogueIndex = 0;
            PromptUI.Instance?.Show(afterEatLines[dialogueIndex]);

            // 解锁鼠标方便点击翻页（如果你是FPS）
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            FinishInteraction();
        }
    }

    private void NextDialoguePage()
    {
        dialogueIndex++;

        if (afterEatLines == null || dialogueIndex >= afterEatLines.Length)
        {
            // 结束字幕
            PromptUI.Instance?.Hide();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            FinishInteraction();
        }
        else
        {
            PromptUI.Instance?.Show(afterEatLines[dialogueIndex]);
        }
    }

    private void FinishInteraction()
    {
        state = State.Done;

        if (oneTimeOnly)
        {
            // 一次性：关掉触发器
            gameObject.SetActive(false);
        }
        else
        {
            // 允许重复：回到开头
            state = State.WaitingToCook;
            RefreshPrompt();
        }
    }

    private float PlayAndGetLength(AudioClip clip)
    {
        if (audioSource == null || clip == null) return 0f;
        audioSource.PlayOneShot(clip, 1f);
        return clip.length;
    }
}
