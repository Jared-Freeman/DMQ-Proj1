using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace ActorSystem.StatusEffect.UI
{    
    public class SE_Debug_StatusRenderer : MonoBehaviour
    {
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
            foreach(var prox in _List_ProxyRecords)
            {
                foreach(var r in prox.Records)
                {
                    Vector3 pos = r.Proxy.GetCameraSpacePosition(_Camera);
                    pos.z = 0f;
                    r._RectTransform.anchoredPosition = pos;
                    var p = r._RectTransform.localPosition;

                    //this is dumb...
                    r._RectTransform.localPosition = new Vector3(p.x, p.y, 0);
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
