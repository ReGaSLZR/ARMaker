using UnityEngine;

namespace ARMarker
{
    public class AutoDestroy : MonoBehaviour
    {

        [SerializeField]
        private float countdown = 1f;

        private void Start()
        {
            Destroy(gameObject, countdown);
        }

    }

}