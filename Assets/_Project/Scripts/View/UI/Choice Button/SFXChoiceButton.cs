using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    public class SFXChoiceButton : BaseChoiceButton<SFXLayerData>
    {

        public override void SetUp(SFXLayerData data, ScrollRect scrollRect, RectTransform dropArea)
        {
            base.SetUp(data, scrollRect, dropArea);
            SetImage(data.Sprite.texture);
        }

    }

}