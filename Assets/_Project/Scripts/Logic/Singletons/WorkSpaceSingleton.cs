using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMarker
{

    public class WorkSpaceSingleton : BaseSingleton<WorkSpaceSingleton>
    {

        [SerializeField]
        private WorkLayer prefabLayer;

        [Header("Clone Settings")]

        [SerializeField]
        private Vector3 clonePosition;

        [Space]

        [SerializeField]
        private Quaternion cloneRotationAndroid;

        [SerializeField]
        private Quaternion cloneRotationiOS;

        [Space]

        [SerializeField]
        private Vector3 cloneScale;

        private readonly List<WorkLayer> cachedLayers = new();
        private GameObject cachedClone;
        private LayerEditMode cachedLayerEditMode = LayerEditMode.UNSET;

        private WorkLayer cachedLayer;
        private WorkLayer cachedTempLayerForMarker;

        private Action<WorkLayer> onChangeActiveLayer;
        private Action<LayerEditMode> onUpdateLayerEditMode;
        private Action<int> onChangeLayerCount;

        public void DeleteClone()
        {
            if (cachedClone != null)
            { 
                Destroy(cachedClone);
                cachedClone = null;
            }
        }

        public GameObject GetClone() => cachedClone;

        public Quaternion GetDesiredCloneRotation() 
        {
#if PLATFORM_IOS || UNITY_IOS
            return cloneRotationiOS;
#endif
            return cloneRotationAndroid;
        }

        public void CloneToARWorld(ARTrackedImage trackedImage, bool shouldReparent = true)
        {
            DeleteClone();
            SetIsEnabled(true);

            if (cachedLayer != null)
            {
                cachedLayer.Deselect();
            }

            foreach (var layer in cachedLayers)
            {
                layer.Deselect();
            }

            cachedClone = Instantiate(gameObject);

#if UNITY_EDITOR
            cachedClone.transform.SetParent(ARSessionSingleton.Instance
                .InstantiateARObjectToSessionOrigin());
            cachedClone.transform.localScale = cloneScale;
#else
            if (shouldReparent)
            {
                cachedClone.transform.SetParent(trackedImage.transform);

#if PLATFORM_ANDROID || UNITY_ANDROID
                trackedImage.transform.localScale = Vector3.one;
#endif
            }

            var imageSize = trackedImage.size;
            var width = imageSize.x / ConstantInts.AR_OBJECT_SIZE_DIVISOR;
            var height = imageSize.y / ConstantInts.AR_OBJECT_SIZE_DIVISOR;

            var baseScale = Mathf.Min(width, height);
            cachedClone.transform.localScale =
                new Vector3(baseScale, baseScale, baseScale);
#endif

            cachedClone.transform.localPosition = clonePosition;
            cachedClone.transform.position = clonePosition;
            cachedClone.transform.rotation = GetDesiredCloneRotation();
            
            cachedClone.SetActive(true);
            SetIsEnabled(false);

            Debug.Log($"{GetType().Name} WorkSpace cloned!", gameObject);
        }

        public List<WorkLayer> GetLayers() => cachedLayers;
        public LayerEditMode GetLayerEditMode() => cachedLayerEditMode;
        public WorkLayer GetActiveLayer() => cachedLayer;

        public void ChangeActiveLayer(WorkLayer layer)
        {
            cachedLayer = layer;
            onChangeActiveLayer(layer);
        }

        public void RegisterOnChangeLayer(
            Action<WorkLayer> listener, bool deregisterInstead = false)
        {
            if (listener == null)
            {
                return;
            }

            if (deregisterInstead)
            {
                onChangeActiveLayer -= listener;
            }
            else
            {
                onChangeActiveLayer += listener;
            }
        }

        public void RegisterOnChangeLayerEditMode(
            Action<LayerEditMode> listener, bool deregisterInstead = false)
        {
            if (listener == null)
            {
                return;
            }

            if (deregisterInstead)
            {
                onUpdateLayerEditMode -= listener;
            }
            else
            {
                onUpdateLayerEditMode += listener;
            }
        }

        public void RegisterOnChangeLayerCount(
            Action<int> listener, bool deregisterInstead = false)
        {
            if (listener == null)
            {
                return;
            }

            if (deregisterInstead)
            {
                onChangeLayerCount -= listener;
            }
            else
            {
                onChangeLayerCount += listener;
            }
        }

        public void SetLayerEditMode(LayerEditMode mode)
        {
            cachedLayerEditMode = mode;
            onUpdateLayerEditMode?.Invoke(cachedLayerEditMode);
        }

        public void SetTempLayerIsEnabled(bool isEnabled)
        {
            if (cachedTempLayerForMarker != null)
            {
                cachedTempLayerForMarker.SetEnabledIfTemporary(isEnabled);
            }
        }

        public void SetIsEnabled(bool isEnabled)
        { 
            gameObject.SetActive(isEnabled);
        }

        public void AddLayer(Sprite sprite, bool isTemporary = false)
        {
            var data = new WorkLayerData();
            data.ResetTransform();

            data.sprite = sprite;
            data.isTemporary = isTemporary;

            if (isTemporary && (cachedTempLayerForMarker != null))
            {
                DestroyImmediate(cachedTempLayerForMarker.gameObject);
                cachedTempLayerForMarker = null;
            }

            var layer = Instantiate(prefabLayer, transform);
            layer.enabled = true;
            layer.SetUp(data);

            if (isTemporary)
            {
                cachedTempLayerForMarker = layer;
            }
            else
            {
                cachedLayers.Add(layer);
            }

            onChangeLayerCount?.Invoke(cachedLayers.Count);
        }

    }

}