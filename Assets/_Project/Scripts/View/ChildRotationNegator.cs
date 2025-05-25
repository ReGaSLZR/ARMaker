using UnityEngine;

namespace ARMarker
{

    public class ChildRotationNegator : MonoBehaviour
    {

        [SerializeField]
        private Transform child;

        private Quaternion initialWorldRotation;

        private void Awake()
        {
            initialWorldRotation = child.rotation;
        }

        private void LateUpdate()
        {
            child.rotation = initialWorldRotation;   
        }

    }

}