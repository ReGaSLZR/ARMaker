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

        public void RegisterMarkerChooser(ARMarkerChooser chooser)
        {
            if (chooser == null)
            {
                return;
            }

            chooser.RegisterOnChooseMarker(
                marker => cachedMarker = marker);
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
            WorkSpaceSingleton.Instance.DeleteClone();

            SceneManager.LoadScene((int)Scene.ARPreview);
        }

        private void OnARStatusChange(ARStatus status)
        {
            switch (status)
            {
                case ARStatus.MarkerDetected:
                    {
                        var trackedImage = ARSessionSingleton.Instance.GetTrackedImage();
                        Debug.LogWarning($"{GetType().Name} TRACKED MARKER! " +
                            $"is null?  {trackedImage == null}", gameObject);

                        WorkSpaceSingleton.Instance.CloneToARWorld(trackedImage);
                        break;
                    }
                case ARStatus.SessionOriginCreated:
                    {
                        Debug.Log($"{GetType().Name}.OnSessionOriginAvailable(): " +
                            $"Session Origin ready!", gameObject);
                        ARSessionSingleton.Instance.StartTracking(cachedMarker);
                        break;
                    }
            }
        }

    }

}