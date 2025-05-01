using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMarker
{

    public class WorkCanvasSingleton : BaseSingleton<WorkCanvasSingleton>
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

        [Header("Runtime Data")]

        [SerializeField]
        private List<WorkLayer> layers = new();

        [SerializeField]
        private GameObject clone;

        public void DeleteClone()
        {
            if (clone != null)
            { 
                Destroy(clone);
                clone = null;
            }
        }

        public void CloneToARWorld(ARTrackedImage trackedImage)
        {
            DeleteClone();

            SetIsEnabled(true);

            clone = Instantiate(gameObject);

#if UNITY_EDITOR
            clone.transform.SetParent(ARSessionSingleton.Instance
                .InstantiateARObjectToSessionOrigin());
#else
            clone.transform.SetParent(trackedImage.transform);
#endif

            clone.transform.localPosition = clonePosition;
            clone.transform.position = clonePosition;
            clone.transform.rotation = cloneRotation;
            clone.transform.localScale = cloneScale;

            clone.SetActive(true);

            SetIsEnabled(false);
        }

        public void SetIsEnabled(bool isEnabled)
        { 
            gameObject.SetActive(isEnabled);
        }

        public void AddLayer(Sprite sprite)
        {
            //TODO remove this:
            if (layers.Count > 0)
            {
                layers[layers.Count - 1].gameObject.SetActive(false);
            }
            //TODO remove this: [end]

            var data = new WorkLayerData();
            data.ResetTransform();

            data.sprite = sprite;

            var layer = Instantiate(prefabLayer, transform);
            layers.Add(layer);
            layer.SetUp(data);
        }

    }

}