using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMarker
{

    public class ARSessionOriginEntry : MonoBehaviour
    {

        [SerializeField]
        private ARSessionOrigin aRSessionOrigin;

        private void Awake()
        {
            ARSessionSingleton.Instance
                .RegisterSessionOrigin(aRSessionOrigin);
        }

    }

}