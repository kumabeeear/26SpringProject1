using UnityEngine;

public class SpamQTrigger : MonoBehaviour
{
    [Header("提示文字")]
    [SerializeField] private string hint = "Q";

    [Header("音效")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sfx;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    [Header("鬼畜参数")]
    [SerializeField] private bool randomPitch = true;
    [SerializeField] private float pitchMin = 0.85f;
    [SerializeField] private float pitchMax = 1.15f;

    private bool inside;

    private void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        inside = true;
        PromptUI.Instance?.Show(hint);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        inside = false;
        PromptUI.Instance?.Hide();
    }

    private void Update()
    {
        if (!inside) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlaySfx();
        }
    }

    private void PlaySfx()
    {
        if (audioSource == null || sfx == null) return;

        if (randomPitch)
            audioSource.pitch = Random.Range(pitchMin, pitchMax);
        else
            audioSource.pitch = 1f;

        audioSource.PlayOneShot(sfx, volume);
    }
}
