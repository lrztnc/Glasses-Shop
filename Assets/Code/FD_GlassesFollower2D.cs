using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MP = Mediapipe;
using UIImage = UnityEngine.UI.Image;

public class FD_GlassesFollower2D : MonoBehaviour
{
    [Header("UI / Video")]
    public RawImage screen;
    public RectTransform glassesRect;
    public bool mirrorHorizontally = true;

    [Header("Fit")]
    [Range(0.8f, 2.5f)] public float widthMultiplier = 1.35f;
    [Range(-1f, 1f)]    public float verticalOffset  = 0.05f;
    [Range(0f, 1f)]     public float smooth          = 0.25f;

    [Header("Debug")]
    public bool debugLog = false;
    float _lastLog; RectTransform _screenRect;

    void Awake() {
        if (!glassesRect) glassesRect = (RectTransform)transform;
        _screenRect = screen.rectTransform;
    }

    // --- TUTTE LE FIRME CHE POTRESTI VEDERE NELL'INSPECTOR ---
    public void OnDetection(MP.Detection d)              { if (d!=null) OnDetections(new List<MP.Detection>{ d }); }
    public void OnDetectionsList(List<MP.Detection> ds)  { OnDetections(ds); }     // per UnityEvent<List<>>
    public void OnDetectionsIList(IList<MP.Detection> ds){ OnDetections(ds); }     // per eventi IList<>
    // ----------------------------------------------------------

    void OnDetections(IList<MP.Detection> dets)
    {
        var uiImg = glassesRect.GetComponent<UIImage>();
        if (uiImg == null || uiImg.sprite == null) { uiImg.enabled = false; return; }

        if (dets == null || dets.Count == 0) { uiImg.enabled = false; return; }
        uiImg.enabled = true;
        if (debugLog && Time.time - _lastLog > 1f) { Debug.Log($"[FD] detections: {dets.Count}"); _lastLog = Time.time; }

        var d = dets[0];
        var loc = d.LocationData;
        Vector2? leftEye = null, rightEye = null;

        if (loc != null && loc.RelativeKeypoints != null && loc.RelativeKeypoints.Count >= 2) {
            var kpR = loc.RelativeKeypoints[0]; // right eye
            var kpL = loc.RelativeKeypoints[1]; // left eye
            rightEye = ToLocal(kpR.X, kpR.Y);
            leftEye  = ToLocal(kpL.X, kpL.Y);
        }

        Vector2 mid; float eyeDist; float angleDeg;
        if (leftEye.HasValue && rightEye.HasValue) {
            var pL = leftEye.Value; var pR = rightEye.Value;
            if (mirrorHorizontally) { pL.x = -pL.x; pR.x = -pR.x; }
            mid = (pL + pR) * 0.5f;
            eyeDist = Vector2.Distance(pL, pR);
            angleDeg = Mathf.Atan2(pR.y - pL.y, pR.x - pL.x) * Mathf.Rad2Deg;
        } else {
            var rb = loc.RelativeBoundingBox;
            var center = ToLocal(rb.Xmin + rb.Width*0.5f, rb.Ymin + rb.Height*0.40f);
            if (mirrorHorizontally) center.x = -center.x;
            mid = center;
            eyeDist = _screenRect.rect.width * rb.Width * 0.45f;
            angleDeg = 0f;
        }

        Vector2 targetPos = mid + new Vector2(0f, verticalOffset * eyeDist);
        float targetWidth = eyeDist * widthMultiplier;

        float aspect = (float)uiImg.sprite.texture.width / uiImg.sprite.texture.height;
        Vector2 targetSize = new Vector2(targetWidth, targetWidth / aspect);

        float t = 1f - Mathf.Clamp01(smooth);
        glassesRect.anchoredPosition = Vector2.Lerp(glassesRect.anchoredPosition, targetPos, t);
        var curSize = glassesRect.rect.size;
        var newSize = Vector2.Lerp(curSize, targetSize, t);
        glassesRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize.x);
        glassesRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize.y);
        glassesRect.localRotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(glassesRect.localEulerAngles.z, angleDeg, t));
    }

    Vector2 ToLocal(float nx, float ny)
    {
        var r = _screenRect.rect;
        float x = nx * r.width  - r.width  * 0.5f;
        float y = (1f - ny) * r.height - r.height * 0.5f;
        return new Vector2(x, y);
    }
}