using UnityEngine;
using UnityEngine.SceneManagement;

namespace ARMarker
{ 
    public class SceneManagerSingleton : BaseSingleton<SceneManagerSingleton>
    {

        public void TransitionToAR()
        {
            SceneManager.LoadScene((int)Scene.ARPreview, LoadSceneMode.Single);
        }

        public void TransitionToWorkCanvas()
        {
            Debug.Log($"{GetType().Name}.TransitionToWorkCanvas(): Reloaded Work Area...", gameObject);
            SceneManager.LoadScene((int)Scene.Customization, LoadSceneMode.Single);
        }

    }

}
