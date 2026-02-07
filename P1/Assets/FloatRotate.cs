using UnityEngine;

public class FloatRotate : MonoBehaviour
{
    [Header("Rotate")]
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 30f, 0f); // 每秒转多少度

    [Header("Float")]
    [SerializeField] private float floatAmplitude = 0.08f; // 上下幅度（米）
    [SerializeField] private float floatFrequency = 1.2f;  // 频率（每秒几次）

    private Vector3 startLocalPos;

    private void Start()
    {
        startLocalPos = transform.localPosition;
    }

    private void Update()
    {
        // 1) 缓慢转圈
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);

        // 2) 上下漂浮
        float yOffset = Mathf.Sin(Time.time * Mathf.PI * 2f * floatFrequency) * floatAmplitude;
        transform.localPosition = startLocalPos + new Vector3(0f, yOffset, 0f);
    }
}
