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
        private Action<WorkLayer> onAddNewLayer;
        private Action<int> onLayerCountChange;
        private Action<WorkLayer> onUpdateTempLayer;

        public void DeleteClone()
        {
            if (cachedClone != null)
            { 
                Destroy(cachedClone);
                cachedClone = null;
            }
        }

        public WorkLayer GetTempLayer() => cachedTempLayerForMarker;

        public GameObject GetClone() => cachedClone;

        public void ToggleTempLayer()
        { 
            if(cachedTempLayerForMarker == null)
            {
                return;
            }

            cachedTempLayerForMarker.gameObject.SetActive(
                !cachedTempLayerForMarker.gameObject.activeInHierarchy);
        }

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
            DeactivateCollidersInClone();
            SetIsEnabled(false);

            Debug.Log($"{GetType().Name} WorkSpace cloned!", gameObject);
        }

        private void DeactivateCollidersInClone()
        {
            foreach (Transform child in cachedClone.transform)
            {
                if(child.TryGetComponent<Collider>(out var collider))
                {
                    collider.enabled = false;
                }
            }
        }

        public List<WorkLayer> GetLayers() => cachedLayers;
        public LayerEditMode GetLayerEditMode() => cachedLayerEditMode;
        public WorkLayer GetActiveLayer() => cachedLayer;

        public void ChangeActiveLayer(WorkLayer layer)
        {
            cachedLayer = layer;
            onChangeActiveLayer(layer);
        }

        public void RegisterOnUpdatetempLayer(
            Action<WorkLayer> listener, bool deregisterInstead = false)
        {
            if (listener == null)
            {
                return;
            }

            if (deregisterInstead)
            {
                onUpdateTempLayer -= listener;
            } 
            else
            {
                onUpdateTempLayer += listener;
            }
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

        public void RegisterOnLayerCountChange(
            Action<int> listener, bool deregisterInstead = false)
        {
            if (listener == null)
            {
                return;
            }

            if (deregisterInstead)
            {
                onLayerCountChange -= listener;
            }
            else
            {
                onLayerCountChange += listener;
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

        public void RegisterOnNewLayerAdded(
            Action<WorkLayer> listener, bool deregisterInstead = false)
        {
            if (listener == null)
            {
                return;
            }

            if (deregisterInstead)
            {
                onAddNewLayer -= listener;
            }
            else
            {
                onAddNewLayer += listener;
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

        public WorkLayer AddVideoLayer(VideoLayerData data)
        {
            if (data == null || data.Sprite == null)
            {
                Debug.LogError($"{GetType().Name} Video data is null", gameObject);
                return null;
            }

            var layer = AddLayer(data.Sprite, false);

            if (layer == null)
            {
                Debug.LogError($"{GetType().Name} Layer creation error", gameObject);
                return null;
            }

            layer.SetUpVideoController(data.Clip);
            return layer;
        }

        public WorkLayer AddAnimatedLayer(AnimatorLayerData data)
        {
            if (data == null || data.Sprite == null)
            {
                Debug.LogError($"{GetType().Name} Animator data is null", gameObject);
                return null;
            }

            var layer = AddLayer(data.Sprite, false);

            if (layer == null)
            {
                Debug.LogError($"{GetType().Name} Layer creation error", gameObject);
                return null;
            }

            layer.SetUpAnimator(data.Controller);
            return layer;
        }

        public WorkLayer AddLayer(
            Sprite sprite, 
            bool isTemporary = false)
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
                onUpdateTempLayer?.Invoke(cachedTempLayerForMarker);
            }
            else
            {
                cachedLayers.Add(layer);
                onAddNewLayer?.Invoke(layer);
                onLayerCountChange?.Invoke(cachedLayers.Count);
            }

            return layer;
        }

        public void DuplicateLayer(WorkLayer layerToDuplicate)
        {
            if (layerToDuplicate == null 
                || layerToDuplicate.Data == null)
            {
                return;
            }

            layerToDuplicate.Deselect();
            var layer = AddLayer(layerToDuplicate.Data.sprite, false);

            layer.transform.localPosition = layerToDuplicate.transform.localPosition;
            layer.transform.rotation = layerToDuplicate.transform.rotation;
            layer.transform.localScale = layerToDuplicate.transform.localScale;

            if (layerToDuplicate.Data.controller != null)
            {
                layer.SetUpAnimator(layerToDuplicate.Data.controller);
            }
            else if (layerToDuplicate.Data.clip != null)
            { 
                layer.SetUpVideoController(layerToDuplicate.Data.clip);
            }
        }

        public void DeleteLayer(WorkLayer layerToDelete)
        {
            if (layerToDelete == null)
            {
                return;
            }

            for(int x=0; x<cachedLayers.Count; x++)
            {
                var layer = cachedLayers[x];

                if (layer == null)
                {
                    continue;
                }

                if (layer.GetHashCode() == layerToDelete.GetHashCode())
                {
                    cachedLayers.RemoveAt(x);
                    DestroyImmediate(layer.gameObject);
                    onLayerCountChange?.Invoke(cachedLayers.Count);
                    return;
                }
            }
        }

    }

}