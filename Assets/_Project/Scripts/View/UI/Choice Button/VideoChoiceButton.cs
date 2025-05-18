namespace ARMarker
{
    public class VideoChoiceButton : BaseChoiceButton<VideoLayerData>
    {

        public override void SetUp(VideoLayerData data)
        {
            base.SetUp(data);
            SetImage(data.Sprite.texture);
        }

    }

}