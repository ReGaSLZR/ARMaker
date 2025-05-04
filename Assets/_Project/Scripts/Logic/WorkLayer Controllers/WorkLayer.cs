using UnityEngine;

namespace ARMarker
{
    public class WorkLayer : MonoBehaviour
    {

        [Header("Settings")]

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        private WorkLayerData cachedData;

        public void SetUp(WorkLayerData data)
        {
            if (data == null)
            {
                Debug.LogError($"{GetType().Name}.SetUp(): data is null!", gameObject);
                return;
            }

            cachedData = data;

            gameObject.transform.localPosition = data.position;
            gameObject.transform.rotation = data.rotation;
            gameObject.transform.localScale = data.scale;

            SetUpSprite();
        }

        private void SetUpSprite()
        {
            if (cachedData == null || cachedData.sprite == null)
            {
                Destroy(spriteRenderer);
                return;
            }

            spriteRenderer.sprite = cachedData.sprite;
            spriteRenderer.size = new Vector2(
                ConstantInts.AR_OBJECT_SIZE_DIVISOR, 
                ConstantInts.AR_OBJECT_SIZE_DIVISOR);
        }

    }

}