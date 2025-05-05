using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyPackages.UIFramework.Runtime
{
    public enum UIType
    {
        Normal,
        Fixed,
        PopUp,
        None, 
    }

    public enum UIMode
    {
        DoNothing,
        HideOther, 
        NeedBack, 
        NoNeedBack, 
    }

    public enum UICollider
    {
        None,
        Normal, 
        WithBg, 
    }

    public abstract class UIPage
    {
        public static List<UIPage> currentPageNodes => m_currentPageNodes;
        public static Dictionary<string, UIPage> allPages => m_allPages;
        public static Func<string, Object> delegateSyncLoadUI = null;

        private static Dictionary<string, UIPage> m_allPages;
        private static List<UIPage> m_currentPageNodes;
        private static Action<string, Action<Object>> delegateAsyncLoadUI = null;

        protected object data => m_data;
        protected GameObject gameObject;
        protected Transform transform;
        protected string uiPath = string.Empty;

        private UIType type = UIType.Normal;
        private UIMode mode = UIMode.DoNothing;
        private UICollider collider = UICollider.None;
        private int id = -1;
        private string name = string.Empty;
        private bool isAsyncUI = false;
        private bool isActived = false;
        private object m_data = null;

        #region UI_API

        protected virtual void Awake(GameObject go)
        {
        }

        protected virtual void Refresh()
        {
        }

        protected virtual void Active()
        {
            gameObject.SetActive(true);
            isActived = true;
        }

        protected virtual void Hide()
        {
            gameObject.SetActive(false);
            isActived = false;
            m_data = null;
        }

        #endregion

        #region internal api

        private UIPage()
        {
        }

        protected UIPage(UIType type, UIMode mod, UICollider col)
        {
            this.type = type;
            mode = mod;
            collider = col;
            name = GetType().ToString();
            UIBind.Bind();
        }

        /// <summary>
        /// Sync Show UI 
        /// </summary>
        private void Show()
        {
            if (gameObject == null && string.IsNullOrEmpty(uiPath) == false)
            {
                GameObject go = null;
                if (delegateSyncLoadUI != null)
                {
                    Object o = delegateSyncLoadUI(uiPath);
                    go = o != null ? GameObject.Instantiate(o) as GameObject : null;
                }
                else
                {
                    go = GameObject.Instantiate(Resources.Load(uiPath)) as GameObject;
                }

                if (go == null)
                {
                    Debug.LogError("[UI] Cant sync load your ui prefab.");
                    return;
                }

                AnchorUIGameObject(go);
                Awake(go);
                isAsyncUI = false;
            }

            Active();
            Refresh();
            PopNode(this);
        }

        /// <summary>
        /// Async Show UI Logic
        /// </summary>
        private void Show(Action callback)
        {
            UIRoot.Instance.StartCoroutine(AsyncShow(callback));
        }

        private IEnumerator AsyncShow(Action callback)
        {
            //Instance UI
            if (gameObject == null && string.IsNullOrEmpty(uiPath) == false)
            {
                GameObject go = null;
                bool _loading = true;
                delegateAsyncLoadUI(uiPath, (o) =>
                {
                    go = o != null ? GameObject.Instantiate(o) as GameObject : null;
                    AnchorUIGameObject(go);
                    Awake(go);
                    isAsyncUI = true;
                    _loading = false;

                    Active();
                    Refresh();
                    PopNode(this);
                    callback?.Invoke();
                });

                float _t0 = Time.realtimeSinceStartup;
                while (_loading)
                {
                    if (Time.realtimeSinceStartup - _t0 >= 10.0f)
                    {
                        Debug.LogError("[UI] WTF async load your ui prefab timeout!");
                        yield break;
                    }

                    yield return null;
                }
            }
            else
            {
                Active();
                Refresh();
                PopNode(this);
                if (callback != null) callback();
            }
        }

        internal bool CheckIfNeedBack()
        {
            if (type == UIType.Fixed || type == UIType.PopUp || type == UIType.None) return false;
            else if (mode == UIMode.NoNeedBack || mode == UIMode.DoNothing) return false;
            return true;
        }

        protected void AnchorUIGameObject(GameObject ui)
        {
            if (UIRoot.Instance == null || ui == null) return;

            gameObject = ui;
            transform = ui.transform;

            Vector3 anchorPos = Vector3.zero;
            Vector2 sizeDel = Vector2.zero;
            Vector3 scale = Vector3.one;
            if (ui.GetComponent<RectTransform>() != null)
            {
                anchorPos = ui.GetComponent<RectTransform>().anchoredPosition;
                sizeDel = ui.GetComponent<RectTransform>().sizeDelta;
                scale = ui.GetComponent<RectTransform>().localScale;
            }
            else
            {
                anchorPos = ui.transform.localPosition;
                scale = ui.transform.localScale;
            }

            if (type == UIType.Fixed)
            {
                ui.transform.SetParent(UIRoot.Instance.fixedRoot);
            }
            else if (type == UIType.Normal)
            {
                ui.transform.SetParent(UIRoot.Instance.normalRoot);
            }
            else if (type == UIType.PopUp)
            {
                ui.transform.SetParent(UIRoot.Instance.popupRoot);
            }


            if (ui.GetComponent<RectTransform>() != null)
            {
                ui.GetComponent<RectTransform>().anchoredPosition = anchorPos;
                ui.GetComponent<RectTransform>().sizeDelta = sizeDel;
                ui.GetComponent<RectTransform>().localScale = scale;
            }
            else
            {
                ui.transform.localPosition = anchorPos;
                ui.transform.localScale = scale;
            }
        }

        public override string ToString()
        {
            return ">Name:" + name + ",ID:" + id + ",Type:" + type.ToString() + ",ShowMode:" + mode.ToString() +
                   ",Collider:" + collider.ToString();
        }

        private bool DOIsActive()
        {
            bool ret = gameObject != null && gameObject.activeSelf;
            return ret || isActived;
        }

        #endregion

        #region static api

        private static bool CheckIfNeedBack(UIPage page)
        {
            return page != null && page.CheckIfNeedBack();
        }

        private static void PopNode(UIPage page)
        {
            if (m_currentPageNodes == null)
            {
                m_currentPageNodes = new List<UIPage>();
            }

            if (page == null)
            {
                Debug.LogError("[UI] page popup is null.");
                return;
            }

            if (CheckIfNeedBack(page) == false)
            {
                return;
            }

            bool _isFound = false;
            for (int i = 0; i < m_currentPageNodes.Count; i++)
            {
                if (m_currentPageNodes[i].Equals(page))
                {
                    m_currentPageNodes.RemoveAt(i);
                    m_currentPageNodes.Add(page);
                    _isFound = true;
                    break;
                }
            }

            if (!_isFound)
            {
                m_currentPageNodes.Add(page);
            }

            HideOldNodes();
        }

        private static void HideOldNodes()
        {
            if (m_currentPageNodes.Count < 0) return;
            UIPage topPage = m_currentPageNodes[m_currentPageNodes.Count - 1];
            if (topPage.mode == UIMode.HideOther)
            {
                //form bottom to top.
                for (int i = m_currentPageNodes.Count - 2; i >= 0; i--)
                {
                    if (m_currentPageNodes[i].DOIsActive())
                        m_currentPageNodes[i].Hide();
                }
            }
        }

        public static void ClearNodes()
        {
            m_currentPageNodes.Clear();
        }

        /// <summary>
        /// Show Page
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="pageInstance"></param>
        public static void ShowPage(string pageName, UIPage pageInstance)
        {
            ShowPage(pageName, pageInstance, null, null, false);
        }

        /// <summary>
        /// Show Page With Page Data Input.
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="pageInstance"></param>
        /// <param name="pageData"></param>
        public static void ShowPage(string pageName, UIPage pageInstance, object pageData)
        {
            ShowPage(pageName, pageInstance, null, pageData, false);
        }
        
        /// <summary>
        /// Async Show Page
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="pageInstance"></param>
        /// <param name="callback"></param>
        public static void ShowPage(string pageName, UIPage pageInstance, Action callback)
        {
            ShowPage(pageName, pageInstance, callback, null, true);
        }
        
        /// <summary>
        /// Async Show Page With Page Data Input.
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="pageInstance"></param>
        /// <param name="callback"></param>
        /// <param name="pageData"></param>
        public static void ShowPage(string pageName, UIPage pageInstance, Action callback, object pageData)
        {
            ShowPage(pageName, pageInstance, callback, pageData, true);
        }

        /// <summary>
        /// Async Show Page
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="T"></typeparam>
        public static void ShowPage<T>(Action callback) where T : UIPage, new()
        {
            ShowPage<T>(callback, null, true);
        }

        /// <summary>
        /// Async Show Page With Page Data Input.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="pageData"></param>
        /// <typeparam name="T"></typeparam>
        public static void ShowPage<T>(Action callback, object pageData) where T : UIPage, new()
        {
            ShowPage<T>(callback, pageData, true);
        }

        /// <summary>
        /// Sync Show Page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void ShowPage<T>() where T : UIPage, new()
        {
            ShowPage<T>(null, null, false);
        }

        /// <summary>
        /// Sync Show Page With Page Data Input.
        /// </summary>
        /// <param name="pageData"></param>
        /// <typeparam name="T"></typeparam>
        public static void ShowPage<T>(object pageData) where T : UIPage, new()
        {
            ShowPage<T>(null, pageData, false);
        }
        
        private static void ShowPage<T>(Action callback, object pageData, bool isAsync) where T : UIPage, new()
        {
            Type t = typeof(T);
            string pageName = t.ToString();

            if (m_allPages != null && m_allPages.ContainsKey(pageName))
            {
                ShowPage(pageName, m_allPages[pageName], callback, pageData, isAsync);
            }
            else
            {
                T instance = new T();
                ShowPage(pageName, instance, callback, pageData, isAsync);
            }
        }
        
        private static void ShowPage(string pageName, UIPage pageInstance, Action callback, object pageData,
            bool isAsync)
        {
            if (string.IsNullOrEmpty(pageName) || pageInstance == null)
            {
                Debug.LogError("[UI] show page error with :" + pageName + " maybe null instance.");
                return;
            }

            if (m_allPages == null)
            {
                m_allPages = new Dictionary<string, UIPage>();
            }

            UIPage page = null;
            if (m_allPages.TryGetValue(pageName, out var allPage))
            {
                page = allPage;
            }
            else
            {
                m_allPages.Add(pageName, pageInstance);
                page = pageInstance;
            }

            page.m_data = pageData;

            if (isAsync)
                page.Show(callback);
            else
                page.Show();
        }

        /// <summary>
        /// close current page in the "top" node.
        /// </summary>
        public static void ClosePage()
        {
            if (m_currentPageNodes == null || m_currentPageNodes.Count <= 1) return;

            UIPage closePage = m_currentPageNodes[m_currentPageNodes.Count - 1];
            m_currentPageNodes.RemoveAt(m_currentPageNodes.Count - 1);

            if (m_currentPageNodes.Count > 0)
            {
                UIPage page = m_currentPageNodes[m_currentPageNodes.Count - 1];
                if (page.isAsyncUI)
                    ShowPage(page.name, page, () => { closePage.Hide(); });
                else
                {
                    ShowPage(page.name, page);
                    closePage.Hide();
                }
            }
        }
    
        public static void ClosePage<T>() where T : UIPage
        {
            Type t = typeof(T);
            string pageName = t.ToString();

            if (m_allPages != null && m_allPages.ContainsKey(pageName))
            {
                ClosePage(m_allPages[pageName]);
            }
            else
            {
                Debug.LogError(pageName + "havnt show yet!");
            }
        }

        public static void ClosePage(string pageName)
        {
            if (m_allPages != null && m_allPages.ContainsKey(pageName))
            {
                ClosePage(m_allPages[pageName]);
            }
            else
            {
                Debug.LogError(pageName + " havnt show yet!");
            }
        }
        
        private static void ClosePage(UIPage target)
        {
            if (target == null) return;
            if (target.DOIsActive() == false)
            {
                if (m_currentPageNodes != null)
                {
                    for (int i = 0; i < m_currentPageNodes.Count; i++)
                    {
                        if (m_currentPageNodes[i] == target)
                        {
                            m_currentPageNodes.RemoveAt(i);
                            break;
                        }
                    }

                    return;
                }
            }

            if (m_currentPageNodes != null && m_currentPageNodes.Count >= 1 &&
                m_currentPageNodes[m_currentPageNodes.Count - 1] == target)
            {
                m_currentPageNodes.RemoveAt(m_currentPageNodes.Count - 1);

                //show older page.
                //TODO:Sub pages.belong to root node.
                if (m_currentPageNodes.Count > 0)
                {
                    UIPage page = m_currentPageNodes[m_currentPageNodes.Count - 1];
                    if (page.isAsyncUI)
                        ShowPage(page.name, page, () => { target.Hide(); });
                    else
                    {
                        ShowPage(page.name, page);
                        target.Hide();
                    }

                    return;
                }
            }
            else if (target.CheckIfNeedBack())
            {
                for (int i = 0; i < m_currentPageNodes.Count; i++)
                {
                    if (m_currentPageNodes[i] == target)
                    {
                        m_currentPageNodes.RemoveAt(i);
                        target.Hide();
                        break;
                    }
                }
            }

            target.Hide();
        }


        #endregion
    }
}