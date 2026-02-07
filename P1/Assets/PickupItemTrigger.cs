using UnityEngine;

public class PickupItemTrigger : MonoBehaviour
{
    [Header("拖入真正要消失的物体（你的 List 模型）")]
    [SerializeField] private GameObject itemToHide;

    private bool inside;
void Start()
{
    PlayerPrefs.DeleteKey("HasPickedItem");
}

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        inside = true;
        PromptUI.Instance?.Show("Press F to pickup");
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

        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayerPrefs.SetInt("HasPickedItem", 1);
            PlayerPrefs.Save();

            PromptUI.Instance?.Hide();

            if (itemToHide != null) itemToHide.SetActive(false);
        }
    }
}
