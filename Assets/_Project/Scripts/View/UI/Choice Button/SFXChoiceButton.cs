namespace ARMarker
{

    public class SFXChoiceButton : BaseChoiceButton<SFXLayerData>
    {

        public override void SetUp(SFXLayerData data)
        {
            base.SetUp(data);
            SetImage(data.Sprite.texture);
        }

    }

}