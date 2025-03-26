using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyPackages.UIFramework.Runtime
{
    /// <summary>
    /// UIRoot
    /// -Canvas
    /// --FixedRoot
    /// --NormalRoot
    /// --PopupRoot
    /// -UICamera
    /// </summary>
    public class UIRoot : MonoBehaviour
    {
        private static UIRoot m_Instance = null;

        public static UIRoot Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    InitRoot();
                }

                return m_Instance;
            }
        }

        public Transform root;
        public Transform fixedRoot;
        public Transform normalRoot;
        public Transform popupRoot;
        public Camera uiCamera;

        private static void InitRoot()
        {
            var go = new GameObject("UIRoot");
            go.layer = LayerMask.NameToLayer("UI");
            m_Instance = go.AddComponent<UIRoot>();
            go.AddComponent<RectTransform>();

            var can = go.AddComponent<Canvas>();
            can.renderMode = RenderMode.ScreenSpaceCamera;
            can.pixelPerfect = true;

            go.AddComponent<GraphicRaycaster>();

            m_Instance.root = go.transform;

            GameObject camObj = new GameObject("UICamera");
            camObj.layer = LayerMask.NameToLayer("UI");
            camObj.transform.parent = go.transform;
            camObj.transform.localPosition = new Vector3(0, 0, -100f);
            var cam = camObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Depth;
            cam.orthographic = true;
            cam.farClipPlane = 200f;
            can.worldCamera = cam;
            cam.cullingMask = 1 << 5;
            cam.nearClipPlane = -50f;
            cam.farClipPlane = 50f;

            m_Instance.uiCamera = cam;
            
            camObj.AddComponent<AudioListener>();
            
            CanvasScaler cs = go.AddComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(640f,1136f);
            cs.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            var subRoot = CreateSubCanvasForRoot(go.transform, 0);
            subRoot.name = "NormalRoot";
            m_Instance.normalRoot = subRoot.transform;
            m_Instance.normalRoot.transform.localScale = Vector3.one;

            subRoot = CreateSubCanvasForRoot(go.transform, 250);
            subRoot.name = "FixedRoot";
            m_Instance.fixedRoot = subRoot.transform;
            m_Instance.fixedRoot.transform.localScale = Vector3.one;

            subRoot = CreateSubCanvasForRoot(go.transform, 500);
            subRoot.name = "PopupRoot";
            m_Instance.popupRoot = subRoot.transform;
            m_Instance.popupRoot.transform.localScale = Vector3.one;

            var esObj = GameObject.Find("EventSystem");
            if (esObj != null)
            {
                DestroyImmediate(esObj);
            }

            GameObject eventObj = new GameObject("EventSystem");
            eventObj.layer = LayerMask.NameToLayer("UI");
            eventObj.transform.SetParent(go.transform);
            eventObj.AddComponent<EventSystem>();
            eventObj.AddComponent<StandaloneInputModule>();
        }

        private static GameObject CreateSubCanvasForRoot(Transform root, int sort)
        {
            var go = new GameObject("canvas");
            go.transform.parent = root;
            go.layer = LayerMask.NameToLayer("UI");
            var rect = go.AddComponent<RectTransform>();
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;

            return go;
        }

        private void OnDestroy()
        {
            m_Instance = null;
        }
    }
}