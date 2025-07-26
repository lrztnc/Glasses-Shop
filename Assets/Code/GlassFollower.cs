using UnityEngine;

public class GlassesFollower : MonoBehaviour
{
    public Transform leftEyeLandmark;
    public Transform rightEyeLandmark;
    public Transform glasses;

    void Update()
    {
        Vector3 mid = (leftEyeLandmark.position + rightEyeLandmark.position) / 2f;
        glasses.position = mid;

        Vector3 dir = rightEyeLandmark.position - leftEyeLandmark.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        glasses.rotation = Quaternion.Euler(0, 0, angle);

        float scale = dir.magnitude;
        glasses.localScale = new Vector3(scale, scale, 1);
    }
}