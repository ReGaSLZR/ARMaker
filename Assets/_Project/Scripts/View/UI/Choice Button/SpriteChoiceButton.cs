using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    public class SpriteChoiceButton : BaseChoiceButton<Sprite>
    {

        public override void SetUp(Sprite sprite, ScrollRect scrollRect, RectTransform dropArea)
        {
            base.SetUp(sprite, scrollRect, dropArea);
            SetImage(sprite.texture);
        }

    }

}