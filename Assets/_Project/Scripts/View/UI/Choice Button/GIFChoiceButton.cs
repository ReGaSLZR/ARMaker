namespace ARMarker
{

    public class GIFChoiceButton : BaseChoiceButton<AnimatorLayerData>
    {

        public override void SetUp(AnimatorLayerData data)
        {
            base.SetUp(data);
            SetImage(data.Sprite.texture);
        }

    }

}