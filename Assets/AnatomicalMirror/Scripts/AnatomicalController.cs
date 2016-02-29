namespace AnatomicalMirror {
    using UnityEngine;
    using System.Collections.Generic;

    public class AnatomicalController : MonoBehaviour {
        public SimpleGestureListener gestureListener = null;

        public int currentModelIdx;
        public GameObject[] models;
        public List<SkinnedMeshRenderer>[] smRenderers;

        void Awake() {
            smRenderers = new List<SkinnedMeshRenderer>[models.Length];
            for(int i = 0; i < models.Length; ++i) {
                var renderers = models[i].GetComponentsInChildren<SkinnedMeshRenderer>();
                smRenderers[i] = new List<SkinnedMeshRenderer>(renderers);
            }

        }

        void Start() {
            if(gestureListener == null) {
                Debug.LogError("Gesture listener isn't initialized!");
                this.enabled = false;
                return;
            }
            if (models.Length == 0) {
                Debug.LogError("No models to control");
                return;
            }
            if(currentModelIdx < 0 || currentModelIdx >= models.Length) {
                Debug.LogWarning("Start model index is out of range. First model used by default");
                currentModelIdx = 0;
            }
            for(int i = 0; i < models.Length; ++i) {
                models[i].SetActive(currentModelIdx == i);
            }
            gestureListener.SetGestureDetectCallback(GestureDetected);
        }

        protected void NextModel() {
            models[currentModelIdx].SetActive(false);
            if(++currentModelIdx == models.Length) {
                currentModelIdx = 0;
            }
            models[currentModelIdx].SetActive(true);
        }

        protected void SquatDetected() {
            Debug.Log("Squat detected!");
            NextModel();
        }

        public void GestureDetected(KinectGestures.Gestures gesture) {
            switch(gesture) {
                case KinectGestures.Gestures.Squat:
                    SquatDetected();
                    break;
                default:
                    Debug.Log("Gesture handler is missing: " + gesture);
                    break;
            }
        }
    }
}
