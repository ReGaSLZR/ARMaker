using UnityEngine;

namespace ARMarker
{

    public class ARObject : MonoBehaviour
    {

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        public void SetSprite(Texture2D texture)
        { 
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, 0.5f, 0.5f), Vector2.zero);
        }

    }

}