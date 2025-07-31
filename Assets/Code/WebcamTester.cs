using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WebcamTester : MonoBehaviour
{
    public RawImage targetImage;
    private WebCamTexture webcamTexture;

    void Start()
    {
        Debug.Log("🟡 Avvio WebcamTester...");

        if (WebCamTexture.devices.Length > 0)
        {
            string camName = WebCamTexture.devices[0].name;
            Debug.Log("✅ Trovata webcam: " + camName);

            webcamTexture = new WebCamTexture(camName, 1280, 720, 30); // forza risoluzione

            targetImage.texture = webcamTexture;
            targetImage.material.mainTexture = webcamTexture;

            webcamTexture.Play();

            Debug.Log("🟢 webcamTexture.Play() chiamato");

            StartCoroutine(CheckWebcamRunning());
        }
        else
        {
            Debug.LogError("❌ Nessuna webcam trovata!");
        }
    }

    private IEnumerator CheckWebcamRunning()
    {
        yield return new WaitForSeconds(1f);

        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            Debug.Log("✅ webcamTexture.isPlaying = TRUE");

            if (webcamTexture.width <= 16)
            {
                Debug.LogWarning("⚠️ La webcam è in play ma la risoluzione è sospetta: " + webcamTexture.width);
            }
            else
            {
                Debug.Log("📷 Webcam risoluzione: " + webcamTexture.width + "x" + webcamTexture.height);
            }
        }
        else
        {
            Debug.LogError("❌ webcamTexture non è in play!");
        }
    }

    void OnDisable()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
            Debug.Log("🛑 webcamTexture fermata");
        }
    }
}