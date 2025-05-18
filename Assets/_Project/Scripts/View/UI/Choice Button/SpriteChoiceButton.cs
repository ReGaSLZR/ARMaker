using UnityEngine;

namespace ARMarker
{

    public class SpriteChoiceButton : BaseChoiceButton<Sprite>
    {

        public override void SetUp(Sprite sprite)
        {
            base.SetUp(sprite);
            SetImage(sprite.texture);
        }

    }

}