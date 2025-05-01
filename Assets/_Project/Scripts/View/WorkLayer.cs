using UnityEngine;

namespace ARMarker
{
    public class WorkLayer : MonoBehaviour
    {

        [Header("Settings")]

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private float spriteRendererTargetSize;

        [Header("Runtime Data")]

        [SerializeField]
        private WorkLayerData data;

        public void SetUp(WorkLayerData data)
        {
            if (data == null)
            {
                Debug.LogError($"{GetType().Name}.SetUp(): data is null!", gameObject);
                return;
            }

            this.data = data;

            gameObject.transform.localPosition = data.position;
            gameObject.transform.rotation = data.rotation;
            gameObject.transform.localScale = data.scale;

            spriteRenderer.sprite = data.sprite;
            spriteRenderer.size = new Vector2(spriteRendererTargetSize, spriteRendererTargetSize);
        }

    }

}