using UnityEngine;

public class IntroController : MonoBehaviour
{
    [Header("导语")]
    [TextArea(2, 4)]
    [SerializeField] private string[] introLines;

    private int index = 0;
    private bool playing = true;

    private void Start()
    {
        if (introLines == null || introLines.Length == 0)
        {
            playing = false;
            return;
        }

        // 显示第一句
        PromptUI.Instance.Show(introLines[0]);

        // Intro 期间解锁鼠标，方便点击
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (!playing) return;

        // 只允许鼠标左键翻页
        if (Input.GetMouseButtonDown(0))
        {
            index++;

            if (index >= introLines.Length)
            {
                EndIntro();
            }
            else
            {
                PromptUI.Instance.Show(introLines[index]);
            }
        }
    }

    private void EndIntro()
    {
        playing = false;

        PromptUI.Instance.Hide();

        // Intro 结束后锁回鼠标（如果你是 FPS）
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
