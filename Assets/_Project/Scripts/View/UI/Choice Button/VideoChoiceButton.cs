using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{
    public class VideoChoiceButton : BaseChoiceButton<VideoLayerData>
    {

        public override void SetUp(VideoLayerData data, ScrollRect scrollRect, RectTransform dropArea)
        {
            base.SetUp(data, scrollRect, dropArea);
            SetImage(data.Sprite.texture);
        }

    }

}