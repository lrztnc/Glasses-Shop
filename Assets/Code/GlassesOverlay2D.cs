using UnityEngine;
using UnityEngine.UI;

public class GlassesOverlay2D : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform cameraRect;   
    public RectTransform glassesRect;  
    public Image glassesImage;         
    public Sprite[] models;            

    [Header("Tuning")]
    [Tooltip("Quanto la larghezza degli occhiali rispetto alla distanza tra gli angoli esterni degli occhi")]
    public float widthMultiplier = 2.0f;
    [Tooltip("Offset verso l’alto (in multipli della distanza tra occhi)")]
    public float yOffset = 0.05f;
    public bool mirrorX = true;       
    public bool clampToBounds = true;  
    public float smooth = 0.15f;      
    Vector2 _pos; float _angle; float _w;

    public void SetVisible(bool v) => glassesImage.enabled = v;

    public void SelectModel(int index)
    {
        if (models == null || models.Length == 0) return;
        index = Mathf.Clamp(index, 0, models.Length - 1);
        glassesImage.sprite = models[index];
        glassesImage.SetNativeSize();
        SetVisible(true);
    }

    public void UpdateFromLandmarks(Vector2 leftEye01, Vector2 rightEye01)
    {
        if (!glassesImage || !glassesImage.sprite) return;

        if (mirrorX)
        {
            leftEye01.x  = 1f - leftEye01.x;
            rightEye01.x = 1f - rightEye01.x;
        }

        Vector2 L = NormalizedToAnchored(leftEye01);
        Vector2 R = NormalizedToAnchored(rightEye01);
        Vector2 mid = (L + R) * 0.5f;
        Vector2 delta = R - L;

        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        float eyeDist = delta.magnitude;

        float targetWidth = eyeDist * widthMultiplier;
        float aspect = glassesImage.sprite.rect.height / glassesImage.sprite.rect.width;
        float targetHeight = targetWidth * aspect;

        Vector2 up = new Vector2(-delta.y, delta.x).normalized;
        mid += up * (eyeDist * yOffset);

        _pos   = Vector2.Lerp(_pos, mid, 1 - Mathf.Exp(-Time.deltaTime / smooth));
        _angle = Mathf.LerpAngle(_angle, angle, 1 - Mathf.Exp(-Time.deltaTime / smooth));
        _w     = Mathf.Lerp(_w, targetWidth, 1 - Mathf.Exp(-Time.deltaTime / smooth));

        float h = _w * aspect;
        glassesRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _w);
        glassesRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   h);

        Vector2 p = _pos;
        if (clampToBounds)
        {
            Vector2 half = cameraRect.rect.size * 0.5f;
            p.x = Mathf.Clamp(p.x, -half.x, half.x);
            p.y = Mathf.Clamp(p.y, -half.y, half.y);
        }

        glassesRect.anchoredPosition = p;
        glassesRect.localEulerAngles = new Vector3(0, 0, _angle);
    }

    Vector2 NormalizedToAnchored(Vector2 uv01)
    {
        Vector2 size = cameraRect.rect.size;
        return (uv01 - new Vector2(0.5f, 0.5f)) * size;
    }
}