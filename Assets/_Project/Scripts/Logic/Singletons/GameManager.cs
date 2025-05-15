using UnityEngine;
using UnityEngine.SceneManagement;

namespace ARMarker
{

    public class GameManager : BaseSingleton<GameManager>
    {

        private Sprite cachedMarker;

        private void Start()
        {
            ARSessionSingleton.Instance
                .RegisterOnStatusChange(OnARStatusChange);

            LoadWorkSpace();
        }

        public Sprite GetMarker() => cachedMarker;

        public void RegisterMarkerChooser(ARMarkerChooser chooser)
        {
            if (chooser == null)
            {
                return;
            }

            chooser.RegisterOnChooseMarker(OnMarkerChosen);
        }

        private void OnMarkerChosen(Sprite marker)
        { 
            cachedMarker = marker;
            WorkSpaceSingleton.Instance.AddLayer(cachedMarker, true);
        }

        public void LoadScene(Scene scene)
        {
            switch (scene)
            {
                case Scene.ARPreview:
                    {
                        LoadARWorld();
                        break;
                    }
                case Scene.WorkSpace:
                    {
                        LoadWorkSpace();
                        break;
                    }
            }
        }

        private void LoadWorkSpace()
        {
            Debug.Log($"{GetType().Name}.LoadWorkSpace(): " +
                $"Loading Workspace...", gameObject);

            WorkSpaceSingleton.Instance.SetIsEnabled(true);
            WorkSpaceSingleton.Instance.DeleteClone();
            WorkSpaceSingleton.Instance.SetTempLayerIsEnabled(true);
            ARSessionSingleton.Instance.DisableActiveTracking();

            SceneManager.LoadScene((int)Scene.WorkSpace);
        }

        private void LoadARWorld()
        {
            if (cachedMarker == null)
            {
                Debug.LogError($"{GetType().Name}.LoadARWorld(): " +
                    $"No marker chosen yet.", gameObject);
                return;
            }
            if (WorkSpaceSingleton.Instance.GetLayers().Count == 0)
            {
                Debug.LogError($"{GetType().Name}.LoadARWorld(): " +
                    $"No WorkSpace layers at all!", gameObject);
                return;
            }

            WorkSpaceSingleton.Instance.SetIsEnabled(false);
            WorkSpaceSingleton.Instance.SetTempLayerIsEnabled(false);
            WorkSpaceSingleton.Instance.DeleteClone();

            SceneManager.LoadScene((int)Scene.ARPreview);
        }

        private void OnARStatusChange(ARStatus status)
        {
            switch (status)
            {
                case ARStatus.UNSET:
                    {
                        WorkSpaceSingleton.Instance.DeleteClone();
                        break;
                    }
                case ARStatus.MarkerDetected:
                    {
                        var trackedImage = ARSessionSingleton.Instance.GetTrackedImage();
                        Debug.LogWarning($"{GetType().Name} TRACKED MARKER! " +
                            $"is null?  {trackedImage == null}", gameObject);

#if PLATFORM_IOS || UNITY_IOS
                        UpdateClonePositionRotation();
#else
                        WorkSpaceSingleton.Instance.CloneToARWorld(trackedImage);
#endif
                        break;
                    }
#if PLATFORM_IOS || UNITY_IOS
                case ARStatus.ActivelyTrackingMarker:
                    {
                        UpdateClonePositionRotation();
                        break;
                    }
#endif
                case ARStatus.SessionOriginCreated:
                    {
                        Debug.Log($"{GetType().Name}.OnSessionOriginAvailable(): " +
                            $"Session Origin ready!", gameObject);
                        ARSessionSingleton.Instance.StartTracking(cachedMarker);
                        break;
                    }
            }
        }

        private void UpdateClonePositionRotation()
        {
            var trackedImage = ARSessionSingleton.Instance.GetTrackedImage();
            var clone = WorkSpaceSingleton.Instance.GetClone();

            Debug.Log($"{GetType().Name}.UpdateClonePositionRotation(): " +
                           $"trackedImage is null? {trackedImage == null}", gameObject);

            Debug.Log($"{GetType().Name}.UpdateClonePositionRotation(): " +
                    $"clone is null? {clone == null}", gameObject);

            if (clone == null) //safety check
            {
                WorkSpaceSingleton.Instance.CloneToARWorld(trackedImage, false);
                clone = WorkSpaceSingleton.Instance.GetClone();
            }

            clone.transform.SetParent(trackedImage.transform);

            //clone.transform.position = trackedImage.transform.position;
            //clone.transform.localPosition = trackedImage.transform.position;

            clone.transform.position = Vector3.zero;
            clone.transform.localPosition = Vector3.zero;
            clone.transform.rotation = WorkSpaceSingleton.Instance.GetDesiredCloneRotation();
            clone.transform.localRotation = WorkSpaceSingleton.Instance.GetDesiredCloneRotation();
        }

    }

}