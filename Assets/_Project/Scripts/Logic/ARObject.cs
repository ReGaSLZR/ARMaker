using UnityEngine;
using UnityEngine.Rendering;

namespace ARMarker
{

    public class ARObject : MonoBehaviour
    {

        [SerializeField]
        private SpriteRenderer[] spriteRenderer;

        public void SetSprite(Sprite sprite)
        {
            //Rect rect = new Rect(0, 0, sprite.width, sprite.height); // Define the sprite's rect
            //Sprite sprite = Sprite.Create(sprite, rect, new Vector2(0.5f, 0.5f), 100);

            foreach (var renderer in spriteRenderer)
            {
                renderer.sprite = sprite;
            }
        }

    }

}