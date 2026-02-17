using System.Collections;
using UnityEngine;

public class PickupIngredient : MonoBehaviour
{
    [Header("要消失的食材模型（拖进来）")]
    [SerializeField] private GameObject itemToHide;

    [Header("这个食材的唯一Key（每个都不一样）")]
    [SerializeField] private string ingredientKey = "romaine";

    [Header("提示文字")]
    [SerializeField] private string hint = "Press F to pickup";

    [Header("拾取音效（可选）")]
    [SerializeField] private AudioSource audioSource; // 可以在本物体，也可以拖外部sound物体
    [SerializeField] private AudioClip pickupSfx;
    [Range(0f, 1f)] [SerializeField] private float pickupVolume = 1f;
    [SerializeField] private bool randomizePitch = true;

    [Header("拾取后是否禁用触发器（一次性）")]
    [SerializeField] private bool oneTimeOnly = true;

    private bool inside;
    private bool picking;

    private Collider myCol;

    private void Start()
    {
        myCol = GetComponent<Collider>();

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        // 已拾取：开局就隐藏，并禁用触发器
        if (PlayerPrefs.GetInt(ingredientKey, 0) == 1)
        {
            if (itemToHide != null) itemToHide.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        inside = true;

        if (!picking && PlayerPrefs.GetInt(ingredientKey, 0) == 0)
            PromptUI.Instance?.Show(hint);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        inside = false;
        if (!picking) PromptUI.Instance?.Hide();
    }

    private void Update()
    {
        if (!inside) return;
        if (picking) return;
        if (PlayerPrefs.GetInt(ingredientKey, 0) == 1) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(PickRoutine());
        }
    }

    private IEnumerator PickRoutine()
    {
        picking = true;
        PromptUI.Instance?.Hide();

        // 先让物品消失
        if (itemToHide != null) itemToHide.SetActive(false);

        // 立刻禁用Collider，防止重复触发（但不关掉整个物体）
        if (myCol != null) myCol.enabled = false;

        // 播放音效，并拿到时长
        float sfxLen = PlayPickupSfx();

        // 记录状态
        PlayerPrefs.SetInt(ingredientKey, 1);
        PlayerPrefs.Save();

        // 等音效播完再关掉触发器（避免声音被切断）
        if (sfxLen > 0f) yield return new WaitForSeconds(sfxLen);

        if (oneTimeOnly)
        {
            gameObject.SetActive(false);
        }
        else
        {
            // 如果允许重复，就把Collider打开并回到可拾取状态（一般不用）
            if (myCol != null) myCol.enabled = true;
            picking = false;
        }
    }

    private float PlayPickupSfx()
    {
        if (audioSource == null || pickupSfx == null) return 0f;

        audioSource.pitch = randomizePitch ? Random.Range(0.95f, 1.05f) : 1f;
        audioSource.PlayOneShot(pickupSfx, pickupVolume);

        return pickupSfx.length;
    }
}
