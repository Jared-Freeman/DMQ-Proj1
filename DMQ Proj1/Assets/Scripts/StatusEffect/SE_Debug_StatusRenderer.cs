using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace ActorSystem.StatusEffect.UI
{    
    public class SE_Debug_StatusRenderer : MonoBehaviour
    {
        #region Static Members

        /// <summary>
        /// Screen-space offset 
        /// </summary>
        protected static float s_IconOffset = 20f;
        /// <summary>
        /// Max icons to render before moving to a new row, per proxy
        /// </summary>
        protected static int s_MaxIconsPerProxyRow = 5;
        /// <summary>
        /// Max number of icons to render at once, per proxy
        /// </summary>
        protected static int s_MaxIconsPerProxy = 10;


        protected static bool s_CenterIcons = true;

        #endregion

        public GameObject _CanvasImagePrefab;
        public GameObject _Canvas;
        public Camera _Camera;

        List<Status_UI_Proxy_Record> _List_ProxyRecords = new List<Status_UI_Proxy_Record>();

        void Awake()
        {
            if (_CanvasImagePrefab == null)
            {
                Debug.LogError("Please set Canvas Image Prefab in inspector. Destroying.");
                Destroy(this);
            }
            if (_CanvasImagePrefab.GetComponent<CanvasRenderer>() == null)
            {
                Debug.LogError("Canvas Image Prefab does not contain CanvasRenderer component. Destroying.");
                Destroy(this);
            }
            if (_CanvasImagePrefab.GetComponent<Image>() == null)
            {
                Debug.LogError("Canvas Image Prefab does not contain Image component. Destroying.");
                Destroy(this);
            }
        }
        void Start()
        {
            SE_StatusEffect_Instance.OnStatusEffectCreate += SE_StatusEffect_Instance_OnStatusEffectCreate;
            SE_StatusEffect_Instance.OnStatusEffectDestroy += SE_StatusEffect_Instance_OnStatusEffectDestroy;

            SE_UI_Proxy.OnProxyDestroy += SE_UI_Proxy_OnProxyDestroy;
        }

        private void SE_UI_Proxy_OnProxyDestroy(object sender, EventArgs.UIProxyEventArgs e)
        {
            Status_UI_Proxy_Record removal = null;

            foreach(var r in _List_ProxyRecords)
            {
                if(e.Proxy == r.Proxy)
                {
                    removal = r;
                    break;
                }
            }

            if(removal != null)
            {
                //clean up gameobjects on canvas
                foreach(var r in removal.Records)
                {
                    Destroy(r.Renderer);
                }

                _List_ProxyRecords.Remove(removal);
            }
        }

        void OnDestroy()
        {
            SE_StatusEffect_Instance.OnStatusEffectCreate -= SE_StatusEffect_Instance_OnStatusEffectCreate;
            SE_StatusEffect_Instance.OnStatusEffectDestroy -= SE_StatusEffect_Instance_OnStatusEffectDestroy;

            SE_UI_Proxy.OnProxyDestroy -= SE_UI_Proxy_OnProxyDestroy;
        }

        void Update()
        {
            UpdateScreenSpaceElements();
        }

        void UpdateScreenSpaceElements()
        {
            if (s_CenterIcons) RenderIconsCentered();
            else RenderIconsLeftAligned();
        }

        void RenderIconsCentered()
        {
            //trackers to compute offset
            int totalIconsRemaining = 0; //per proxy
            int curIconIndex = 0;
            int curRow = 0;

            float startPosOffset = 0;

            foreach (var prox in _List_ProxyRecords)
            {
                totalIconsRemaining = 0;
                curIconIndex = 0;
                curRow = 0;
                startPosOffset = 0f;

                foreach (var r in prox.Records)
                {
                    totalIconsRemaining++;
                }

                //Debug.LogWarning(totalIconsRemaining);

                startPosOffset = Mathf.Clamp(totalIconsRemaining, 0, s_MaxIconsPerProxyRow-1) * -.5f * s_IconOffset;

                //there is definitely a better-readable version but uh, i want to do this quickly -J
                foreach (var r in prox.Records)
                {
                    Vector3 pos = r.Proxy.GetCameraSpacePosition(_Camera);
                    pos.z = 0f;
                    r._RectTransform.anchoredPosition = pos;
                    var p = r._RectTransform.localPosition;


                    r._RectTransform.localPosition = new Vector3(
                        p.x + startPosOffset + curIconIndex * s_IconOffset,
                        p.y - curRow * s_IconOffset,
                        0);

                    curIconIndex++;
                    totalIconsRemaining--;

                    if (curIconIndex + 1 > s_MaxIconsPerProxyRow) //careful of off-by-1
                    {
                        curIconIndex = 0;
                        curRow++;
                        startPosOffset = Mathf.Clamp(totalIconsRemaining, 0, s_MaxIconsPerProxyRow) * -.5f;
                    }
                }
            }
        }

        //older version. kept in just in case.
        void RenderIconsLeftAligned()
        {

            //trackers to compute offset
            int curIcons = 0;
            int curRow = 0;

            foreach (var prox in _List_ProxyRecords)
            {
                curIcons = 0;
                curRow = 0;

                foreach (var r in prox.Records)
                {

                    Vector3 pos = r.Proxy.GetCameraSpacePosition(_Camera);
                    pos.z = 0f;
                    r._RectTransform.anchoredPosition = pos;
                    var p = r._RectTransform.localPosition;

                    //this is dumb...
                    r._RectTransform.localPosition = new Vector3(
                        p.x + curIcons * s_IconOffset,
                        p.y - curRow * s_IconOffset,
                        0);

                    curIcons++;
                    if (curIcons + 1 > s_MaxIconsPerProxyRow) //careful of off-by-1
                    {
                        curIcons = 0;
                        curRow++;
                    }
                }
            }
        }

        private void SE_StatusEffect_Instance_OnStatusEffectCreate(object sender, CSEventArgs.StatusEffect_Actor_EventArgs e)
        {
            var root = e?._StatusEffect?.gameObject;

            //get UI proxy if one exists
            var Proxy = e?._StatusEffect?.gameObject.GetComponent<SE_UI_Proxy>();

            if (Proxy != null)
            {
                GameObject inst = Instantiate(_CanvasImagePrefab);

                inst.name = "Status Image (" + e._StatusEffect.Preset.name + ")";

                inst.transform.SetParent(_Canvas.transform);
                inst.transform.localScale = new Vector3(.25f, .25f, .25f);

                RectTransform RTransform = inst.GetComponent<RectTransform>();
                RTransform.anchoredPosition = Vector3.zero;
                RTransform.localRotation = Quaternion.Euler(Vector3.zero);


                Image img = inst.GetComponent<Image>();
                img.sprite = e._StatusEffect.Preset.Icon;


                //add to record list
                Status_UI_Record record = new Status_UI_Record();
                record.Ref = e._StatusEffect;
                record.Renderer = inst;
                record.Proxy = Proxy;
                record._RectTransform = RTransform;

                var listRecords = GetRecords(Proxy);

                listRecords.Add(record);
            }
        }

        private void SE_StatusEffect_Instance_OnStatusEffectDestroy(object sender, CSEventArgs.StatusEffect_Actor_EventArgs e)
        {
            Status_UI_Record removeRecord = null;

            foreach (var prox in _List_ProxyRecords)
            {
                foreach (var r in prox.Records)
                {
                    if (r.Ref = e._StatusEffect)
                    {
                        removeRecord = r;
                        break;
                    }
                }

                if (removeRecord != null)
                {
                    prox.Records.Remove(removeRecord);
                    break;
                }
            }
        }

        protected List<Status_UI_Record> GetRecords(SE_UI_Proxy proxy)
        {
            foreach(var pr in _List_ProxyRecords)
            {
                if (proxy = pr.Proxy) return pr.Records;
            }

            var r = new Status_UI_Proxy_Record();
            r.Proxy = proxy;
            r.Records = new List<Status_UI_Record>();
            _List_ProxyRecords.Add(r);

            return r.Records;
        }

        protected class Status_UI_Proxy_Record
        {
            public SE_UI_Proxy Proxy;
            public List<Status_UI_Record> Records;
        }

        protected class Status_UI_Record
        {
            public SE_StatusEffect_Instance Ref = null;
            public GameObject Renderer = null;
            public SE_UI_Proxy Proxy = null;
            public RectTransform _RectTransform = null;
        }
    }
}
