using System.Collections.Generic;
using UnityEngine;

namespace ARMarker
{

    [CreateAssetMenu(
        menuName = ConstantStrings.MENU_ROOT + "Create ARMarker Choice List")]
    public class ARMarkerChoices : ScriptableObject
    {

        [SerializeField]
        private List<Texture2D> choices = new();

        public List<Texture2D> Choices => choices;

    }

}