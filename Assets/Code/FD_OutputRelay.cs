using System;
using System.Collections.Generic;
using UnityEngine;
using MP = Mediapipe;

[Serializable] public class DetectionListEvent : UnityEngine.Events.UnityEvent<List<MP.Detection>> {}

public class FD_OutputRelay : MonoBehaviour
{
  public Component faceDetectionGraph;
  public DetectionListEvent OnDetections;

  void OnEnable() {
    if (!faceDetectionGraph) return;
    TrySub("OnFaceDetectionsOutput");
    TrySub("OnDetectionsOutput");
    TrySub("OnOutput");
    TrySub("OnNext");
    // single detection (alcune versioni)
    TrySubSingle("OnDetectionOutput");
  }

  void TrySub(string eventName) {
    var evt = faceDetectionGraph.GetType().GetEvent(eventName);
    if (evt != null) {
      Action<IList<MP.Detection>> h = Handle;
      evt.AddEventHandler(faceDetectionGraph, h);
      Debug.Log($"[Relay] Subscribed to {eventName}");
    }
  }

  void TrySubSingle(string eventName) {
    var evt = faceDetectionGraph.GetType().GetEvent(eventName);
    if (evt != null) {
      Action<MP.Detection> h = d => { if (d!=null) Handle(new List<MP.Detection>{ d }); };
      evt.AddEventHandler(faceDetectionGraph, h);
      Debug.Log($"[Relay] Subscribed to {eventName} (single)");
    }
  }

  void Handle(IList<MP.Detection> dets) {
    if (dets==null) return;
    OnDetections?.Invoke(new List<MP.Detection>(dets));
  }
}