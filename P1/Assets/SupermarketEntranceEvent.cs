using UnityEngine;

public class SupermarketEntranceEvent : MonoBehaviour
{
    [Header("一次性Key（每个入口都要不同）")]
    [SerializeField] private string uniqueKey = "Entrance_Supermarket";

    [Header("是否进入时重置第二关食材状态")]
    [SerializeField] private bool resetIngredientsOnEnter = true;

    [Header("第二关食材Keys（要和拾取脚本一致）")]
    [SerializeField] private string key1 = "Lettuce";
    [SerializeField] private string key2 = "pepper";
    [SerializeField] private string key3 = "romaine";

    [Header("字幕内容（每一行一页，左键翻页）")]
    [TextArea(2, 4)]
    [SerializeField] private string[] lines;

    [Header("进门音效（可选）")]
    [SerializeField] private AudioSource audioSource; // 推荐用这个物体上的AudioSource
    [SerializeField] private AudioClip enterClip;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    [Header("翻页按键：0=左键 1=右键 2=中键")]
    [SerializeField] private int mouseButton = 0;

    private bool talking;
    private int index;

    private void Start()
    {
        // 触发过就关掉，保证一次性
        if (PlayerPrefs.GetInt(uniqueKey, 0) == 1)
            gameObject.SetActive(false);

        // 如果没指定AudioSource，就尝试自动找
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (PlayerPrefs.GetInt(uniqueKey, 0) == 1) return;

        // 标记为已触发（防止重复触发）
        PlayerPrefs.SetInt(uniqueKey, 1);
        PlayerPrefs.Save();

        // 1) 重置第二关食材
        if (resetIngredientsOnEnter)
        {
            PlayerPrefs.DeleteKey(key1);
            PlayerPrefs.DeleteKey(key2);
            PlayerPrefs.DeleteKey(key3);
            PlayerPrefs.Save();
        }

        // 2) 播放进门音效
        if (audioSource != null && enterClip != null)
        {
            audioSource.PlayOneShot(enterClip, volume);
        }

        // 3) 弹字幕
        if (lines != null && lines.Length > 0)
        {
            talking = true;
            index = 0;

            PromptUI.Instance?.Show(lines[0]);

            // 如果你是 FPS，建议解锁鼠标方便点击翻页
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // 没字幕就直接结束
            EndEvent();
        }
    }

    private void Update()
    {
        if (!talking) return;

        if (Input.GetMouseButtonDown(mouseButton))
        {
            index++;
            if (index >= lines.Length)
            {
                EndEvent();
            }
            else
            {
                PromptUI.Instance?.Show(lines[index]);
            }
        }
    }

    private void EndEvent()
    {
        talking = false;

        PromptUI.Instance?.Hide();

        // 锁回鼠标（如果你是 FPS）
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 一次性：关掉触发器
        gameObject.SetActive(false);
    }
}
