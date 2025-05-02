using UnityEngine.SceneManagement;

namespace ARMarker
{ 
    public class SceneManagerSingleton : BaseSingleton<SceneManagerSingleton>
    {

        public void TransitionToAR()
        {
            SceneManager.LoadScene((int)Scene.ARPreview, LoadSceneMode.Single);
        }

        public void TransitionToWorkspace()
        {
            SceneManager.LoadScene((int)Scene.WorkSpace, LoadSceneMode.Single);
        }

    }

}
