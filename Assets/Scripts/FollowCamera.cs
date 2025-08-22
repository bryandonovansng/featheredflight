using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;  // Your phoenix
    public Vector3 offset = new Vector3(0, 3, -6);  // Adjust as needed
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target);
    }
}
