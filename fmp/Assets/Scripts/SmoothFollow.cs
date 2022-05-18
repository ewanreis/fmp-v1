using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime;
    private void FixedUpdate() => this.transform.position = Vector3.Lerp(this.transform.position, target.position, smoothTime);
}
