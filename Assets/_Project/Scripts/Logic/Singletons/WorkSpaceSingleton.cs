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

        [SerializeField]
        private Quaternion cloneRotation;

        [SerializeField]
        private Vector3 cloneScale;

        private List<WorkLayer> cachedLayers = new();
        private GameObject cachedClone;

        public void DeleteClone()
        {
            if (cachedClone != null)
            { 
                Destroy(cachedClone);
                cachedClone = null;
            }
        }

        public void CloneToARWorld(ARTrackedImage trackedImage)
        {
            DeleteClone();
            SetIsEnabled(true);

            cachedClone = Instantiate(gameObject);

#if UNITY_EDITOR
            cachedClone.transform.SetParent(ARSessionSingleton.Instance
                .InstantiateARObjectToSessionOrigin());
            cachedClone.transform.localScale = cloneScale;

#else
            cachedClone.transform.SetParent(trackedImage.transform);

            var imageSize = trackedImage.size;
            var width = imageSize.x / ConstantInts.AR_OBJECT_SIZE_DIVISOR;
            var height = imageSize.y / ConstantInts.AR_OBJECT_SIZE_DIVISOR;

            var baseScale = Mathf.Min(width, height);
            cachedClone.transform.localScale = 
                new Vector3(baseScale, baseScale, baseScale);
#endif

            cachedClone.transform.localPosition = clonePosition;
            cachedClone.transform.position = clonePosition;
            cachedClone.transform.rotation = cloneRotation;
            
            cachedClone.SetActive(true);
            SetIsEnabled(false);

            Debug.Log($"{GetType().Name} WorkSpace cloned!", gameObject);
        }

        public List<WorkLayer> GetLayers() => cachedLayers;

        public void SetIsEnabled(bool isEnabled)
        { 
            gameObject.SetActive(isEnabled);
        }

        public void AddLayer(Sprite sprite)
        {
            var data = new WorkLayerData();
            data.ResetTransform();

            data.sprite = sprite;

            var layer = Instantiate(prefabLayer, transform);
            cachedLayers.Add(layer);
            layer.SetUp(data);
        }

    }

}