using UnityEngine;

public class GameResetOnStart : MonoBehaviour
{
    [Header("开局自动重置（演示/作业建议开）")]
    [SerializeField] private bool resetOnAwake = true;

    [Header("清空全部Prefs（核选项：只建议排错用）")]
    [SerializeField] private bool deleteAllPrefs = false;

    [Header("要在开局强制重新启用的物体（门/触发器/音源等）")]
    [SerializeField] private GameObject[] objectsToReactivate;

    private void Awake()
    {
        if (!resetOnAwake) return;

        if (deleteAllPrefs)
        {
            PlayerPrefs.DeleteAll();
        }
        else
        {
            // 第一关
            PlayerPrefs.DeleteKey("HasPickedItem");

            // 三个食材（你给我的 key）
            PlayerPrefs.DeleteKey("romaine");
            PlayerPrefs.DeleteKey("pepper");
            PlayerPrefs.DeleteKey("Lettuce");

            // 吃完饭后门消失的状态
            PlayerPrefs.DeleteKey("CookDoorGone");
        }

        PlayerPrefs.Save();

        // 把被 SetActive(false) 的东西复活
        if (objectsToReactivate != null)
        {
            foreach (var go in objectsToReactivate)
                if (go != null) go.SetActive(true);
        }

        Debug.Log("GameResetOnStart: reset done.");
    }
}
