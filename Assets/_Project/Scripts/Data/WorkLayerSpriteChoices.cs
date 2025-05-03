using System.Collections.Generic;
using UnityEngine;

namespace ARMarker
{

    [CreateAssetMenu(
        menuName = ConstantStrings.MENU_ROOT + "Create WorkLayer Sprite Choices")]
    public class WorkLayerSpriteChoices : ScriptableObject
    {

        [SerializeField]
        private new string name;

        [SerializeField]
        private List<Sprite> choices = new();

        public string Name => name;
        public List<Sprite> Choices => choices;

    }

}