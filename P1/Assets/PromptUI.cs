using TMPro;
using UnityEngine;

public class PromptUI : MonoBehaviour
{
    public static PromptUI Instance;

    [SerializeField] private TMP_Text promptText;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(string msg)
    {
        promptText.text = msg;
        promptText.gameObject.SetActive(true);
    }

    public void Hide()
    {
        promptText.gameObject.SetActive(false);
    }
}
