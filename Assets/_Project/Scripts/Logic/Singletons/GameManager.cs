using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace ARMarker
{
    public class GameManager : BaseSingleton<GameManager>
    {

        [SerializeField]
        private Button button2D;

        [SerializeField]
        private Button button3D;

        [Space]

        [SerializeField]
        private Button buttonPreviewToAR;

        [SerializeField]
        private Button buttonCustomize;

        [Space]

        [SerializeField]
        private GameObject[] elementsOnCustomization;

        [Space]

        [SerializeField]
        private GameObject[] elementsOnARWorld;

        private CameraPerspectiveSwitcher cachedCamSwitcher;

        protected override void Awake()
        {
            base.Awake();

            buttonPreviewToAR.onClick.AddListener(LoadARWorld);
            buttonCustomize.onClick.AddListener(LoadWorkArea);

            button2D.onClick.AddListener(() => SwitchCamera(true));
            button3D.onClick.AddListener(() => SwitchCamera(false));
        }

        private void Start()
        {
            ARSessionSingleton.Instance
                .RegisterOnTrackedMarker(OnTrackedMarker);
            ARSessionSingleton.Instance
                .RegisterOnSessionOriginAvailable(OnSessionOriginAvailable);
            LoadWorkArea();
        }

        private void SwitchCamera(bool is2D)
        {
            if (cachedCamSwitcher == null)
            {
                return;
            }

            cachedCamSwitcher.SwitchPerspective(is2D);
        }

        private void OnSessionOriginAvailable()
        {
            Debug.Log($"{GetType().Name}.OnSessionOriginAvailable(): " +
                $"Session Origin ready!", gameObject);
            ARSessionSingleton.Instance.StartTracking(
                ARMarkerChooserSingleton.Instance.GetChosenMarker());
        }

        private void LoadWorkArea()
        {
            SetWorkAreaEnabled(true);
            WorkSpaceSingleton.Instance.SetIsEnabled(true);
            WorkSpaceSingleton.Instance.DeleteClone();

            ARSessionSingleton.Instance.DisableActiveTracking();
            SceneManagerSingleton.Instance.TransitionToWorkspace();
        }

        private void LoadARWorld()
        {
            if (ARMarkerChooserSingleton.Instance.GetChosenMarker() 
                == null)
            {
                Debug.LogError($"{GetType().Name}.LoadARWorld(): " +
                    $"No marker chosen yet.", gameObject);
                return;
            }

            WorkSpaceSingleton.Instance.SetIsEnabled(false);
            WorkSpaceSingleton.Instance.DeleteClone();

            SetWorkAreaEnabled(false);

            SceneManagerSingleton.Instance.TransitionToAR();
        }

        private void OnTrackedMarker(ARTrackedImage trackedImage)
        {
            Debug.LogWarning($"{GetType().Name} TRACKED MARKER! " +
                $"is null?  {trackedImage == null}", gameObject);

            WorkSpaceSingleton.Instance.CloneToARWorld(trackedImage);
        }

        private void SetWorkAreaEnabled(bool isOnWorkArea)
        {
            foreach (var element in elementsOnCustomization)
            {
                if (element == null)
                {
                    continue;
                }

                element.SetActive(isOnWorkArea);
            }

            foreach (var element in elementsOnARWorld)
            {
                if (element == null)
                {
                    continue;
                }

                element.SetActive(!isOnWorkArea);
            }
        }

        public void RegisterCameraPerspectiveSwitcher(
            CameraPerspectiveSwitcher switcher)
        { 
            cachedCamSwitcher = switcher;
        }

    }

}