using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DoorHingeRotate : MonoBehaviour
{
    [SerializeField] private Transform hingePoint;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float smooth = 6f;

    private bool open;
    private Quaternion closedRot;
    private Quaternion targetRot;

    private void Start()
    {
        closedRot = transform.rotation;
        targetRot = closedRot;
    }

    private void Update()
    {
        transform.rotation =
            Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * smooth);
    }

    public void SetOpen(bool value)
    {
        if (open == value) return;
        open = value;

        float angle = open ? openAngle : 0f;
        targetRot = Quaternion.AngleAxis(angle, Vector3.up) * closedRot;

        Vector3 pivot = hingePoint.position;
        Vector3 dir = transform.position - pivot;
        Quaternion delta = Quaternion.AngleAxis(open ? openAngle : -openAngle, Vector3.up);
        transform.position = pivot + delta * dir;
    }
}
