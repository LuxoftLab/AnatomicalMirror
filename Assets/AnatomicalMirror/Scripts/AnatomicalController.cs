namespace AnatomicalMirror {
    using UnityEngine;
    using System.Collections.Generic;
    using System.Collections;

    [System.Serializable]
    public class ModelDescription {
        public GameObject goModel;
        public List<Material> materials;
        public Transform motionRoot;
    }

    public enum BlendMode {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    public class AnatomicalController : MonoBehaviour {
        public SimpleGestureListener gestureListener = null;
        public ModelDescription[] models;
        public int currentModelIdx;
        protected int? lastModelidx = null;

        void Awake() {
            for(int i = 0; i < models.Length; ++i) {
                if(i == currentModelIdx) {
                    continue;
                }
                for(int j = 0; j < models[i].materials.Count; ++j) {
                    Color c = models[i].materials[j].color;
                    c.a = 0;
                    models[i].materials[j].color = c;
                }
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
                if(currentModelIdx == i) {
                    StartCoroutine(ShowModel(models[i]));
                } else {
                    StartCoroutine(FadeModel(models[i]));
                }
            }
            gestureListener.SetGestureDetectCallback(GestureDetected);
        }

        void Update() {
            Vector3 curPos = models[currentModelIdx].motionRoot.localPosition;
            if (curPos.z >= 0.5f && currentModelIdx != 1) {
                StartCoroutine(FadeModel(models[currentModelIdx]));
                currentModelIdx = 1;
                StartCoroutine(ShowModel(models[currentModelIdx]));
            } else if (curPos.z < 0.5f && currentModelIdx != 0) {
                StartCoroutine(FadeModel(models[currentModelIdx]));
                currentModelIdx = 0;
                StartCoroutine(ShowModel(models[currentModelIdx]));
            }
        }

        protected void NextModel() {
            StartCoroutine(FadeModel(models[currentModelIdx]));
            if(++currentModelIdx == models.Length) {
                currentModelIdx = 0;
            }
            StartCoroutine(ShowModel(models[currentModelIdx]));
        }

        public static void SetShaderBlendMode(BlendMode blendMode, ref Material material) {
            switch (blendMode) {
                case BlendMode.Opaque:
                    material.SetFloat("_Mode", (float)BlendMode.Opaque);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case BlendMode.Cutout:
                    material.SetFloat("_Mode", (float)BlendMode.Cutout);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 2450;
                    break;
                case BlendMode.Fade:
                    material.SetFloat("_Mode", (float)BlendMode.Fade);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
                case BlendMode.Transparent:
                    material.SetFloat("_Mode", (float)BlendMode.Transparent);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
            }

        }

        public IEnumerator FadeModel(ModelDescription model) {
            for (int i = 0; i < model.materials.Count; ++i) {
                var mat = model.materials[i];
                SetShaderBlendMode(BlendMode.Fade, ref mat);
                model.materials[i] = mat;
            }
            for (float f = 1f; f > 0f; f -= 0.2f) {
                for(int j = 0; j < model.materials.Count; ++j) {
                    Color c = model.materials[j].color;
                    c.a = f;
                    model.materials[j].SetColor("_Color", c);
                }
                yield return new WaitForSeconds(.1f);
            }
            for (int j = 0; j < model.materials.Count; ++j) {
                Color c = model.materials[j].color;
                c.a = 0f;
                model.materials[j].SetColor("_Color", c);
            }
        }

        public IEnumerator ShowModel(ModelDescription model) {
            for (int i = 0; i < model.materials.Count; ++i) {
                var mat = model.materials[i];
                SetShaderBlendMode(BlendMode.Fade, ref mat);
                model.materials[i] = mat;
                Color c = model.materials[i].color;
                c.a = 0f;
                model.materials[i].SetColor("_Color", c);
            }
            for (float f = 0f; f < 1f; f += 0.2f) {
                for (int j = 0; j < model.materials.Count; ++j) {
                    Color c = model.materials[j].color;
                    c.a = f;
                    model.materials[j].SetColor("_Color", c);
                }
                yield return new WaitForSeconds(.1f);
            }
            for (int i = 0; i < model.materials.Count; ++i) {              
                Color c = model.materials[i].color;
                c.a = 1f;
                model.materials[i].SetColor("_Color", c);
                var mat = model.materials[i];
                SetShaderBlendMode(BlendMode.Opaque, ref mat);
                model.materials[i] = mat;
            }
        }

        protected void SquatDetected() {
            Debug.Log("Squat detected!");
            NextModel();
        }

        public void GestureDetected(KinectGestures.Gestures gesture) {
            switch(gesture) {
                case KinectGestures.Gestures.Squat:
                    //SquatDetected();
                    break;
                default:
                    Debug.Log("Gesture handler is missing: " + gesture);
                    break;
            }
        }
    }
}
