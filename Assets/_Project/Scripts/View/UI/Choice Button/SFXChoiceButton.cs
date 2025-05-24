using UnityEngine;
using UnityEngine.UI;

namespace ARMarker
{

    public class SFXChoiceButton : BaseChoiceButton<SFXLayerData>
    {

        public override void SetUp(SFXLayerData data, ScrollRect scrollRect,
            RectTransform dropArea, bool isDraggable)
        {
            base.SetUp(data, scrollRect, dropArea, isDraggable);
            SetImage(data.Sprite.texture);
        }

        protected override WorkLayer AddLayer(SFXLayerData data)
        {
            return WorkSpaceSingleton.Instance.AddSFXLayer(data);
        }
    }

}