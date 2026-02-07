using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private DoorHingeRotate door;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (PlayerPrefs.GetInt("HasPickedItem", 0) == 1)
        {
            PromptUI.Instance.Hide();
            door.SetOpen(true);
        }
        else
        {
            PromptUI.Instance.Show("Maybe I forget something...");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        PromptUI.Instance.Hide();
    }
}
