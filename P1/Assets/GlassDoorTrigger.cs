using UnityEngine;

public class GlassDoorTrigger : MonoBehaviour
{
    [SerializeField] private GlassDoorLift door;
    [SerializeField] private string notReadyMsg = "still missing a few ingredients.";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (door == null) return;

        if (door.AllIngredientsCollected())
        {
            PromptUI.Instance?.Hide();
            door.OpenDoor();
        }
        else
        {
            PromptUI.Instance?.Show(notReadyMsg);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        PromptUI.Instance?.Hide();
    }
}
