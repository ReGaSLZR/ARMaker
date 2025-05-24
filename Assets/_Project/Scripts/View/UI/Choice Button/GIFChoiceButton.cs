using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    public class GIFChoiceButton : BaseChoiceButton<AnimatorLayerData>
    {

        public override void SetUp(AnimatorLayerData data, ScrollRect scrollRect, RectTransform dropArea)
        {
            base.SetUp(data, scrollRect, dropArea);
            SetImage(data.Sprite.texture);
        }

    }

}