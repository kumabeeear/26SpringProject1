using System.Collections;
using UnityEngine;

public class GlassDoorLift : MonoBehaviour
{
    [Header("需要满足的三个Key（和拾取脚本里保持一致）")]
    [SerializeField] private string key1 = "Lettuce";
    [SerializeField] private string key2 = "pepper";
    [SerializeField] private string key3 = "romaine";

    [Header("上升参数")]
    [SerializeField] private float liftHeight = 3.0f;
    [SerializeField] private float liftDuration = 1.2f;

    private bool opened;
    private Vector3 closedPos;

    private void Start()
    {
        closedPos = transform.position;
    }

    public bool AllIngredientsCollected()
    {
        return PlayerPrefs.GetInt(key1, 0) == 1
            && PlayerPrefs.GetInt(key2, 0) == 1
            && PlayerPrefs.GetInt(key3, 0) == 1;
    }

    public void OpenDoor()
    {
        if (opened) return;
        opened = true;

        StartCoroutine(LiftRoutine());
    }

    private IEnumerator LiftRoutine()
    {
        Vector3 start = closedPos;
        Vector3 end = closedPos + Vector3.up * liftHeight;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, liftDuration);

            // smoothstep 更顺滑
            float s = t * t * (3f - 2f * t);

            transform.position = Vector3.Lerp(start, end, s);
            yield return null;
        }

        transform.position = end;
    }
}
