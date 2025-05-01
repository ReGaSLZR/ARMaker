using System.Collections;
using UnityEngine;

namespace ARMarker
{

    public class CameraPerspectiveSwitcher : MonoBehaviour
    {

        [SerializeField]
        private Camera camera;

        [SerializeField]
        private float switchDuration = 1f;

        [Space]

        [SerializeField]
        private Vector3 position2D;

        [SerializeField]
        private Quaternion rotation2D;

        [Space]

        [SerializeField]
        private Vector3 position3D;

        [SerializeField]
        private Quaternion rotation3D;

        private void Start()
        {
            GameManager.Instance.RegisterCamSwitcher(this);
        }

        public void SwitchPerspective(bool is2D)
        {
            var position = is2D ? position2D : position3D;
            var rotation = is2D ? rotation2D : rotation3D;

            StopAllCoroutines();
            StartCoroutine(LerpTransform(position, rotation, switchDuration));
        }

        private IEnumerator LerpTransform(Vector3 targetPos, Quaternion targetRot, float duration)
        {
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                transform.position = Vector3.Lerp(startPos, targetPos, t);
                transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

                yield return null;
            }

            transform.position = targetPos;
            transform.rotation = targetRot;
        }

    }

}