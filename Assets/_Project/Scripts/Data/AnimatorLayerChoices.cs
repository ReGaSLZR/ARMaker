using System.Collections.Generic;
using UnityEngine;

namespace ARMarker
{

    [CreateAssetMenu(
        menuName = ConstantStrings.MENU_ROOT + "Create WorkLayer Animator Choices")]
    public class AnimatorLayerChoices : ScriptableObject
    {

        [SerializeField]
        private new string name;

        [SerializeField]
        private List<AnimatorLayerData> choices = new();

        public string Name => name;
        public List<AnimatorLayerData> Choices => choices;

    }

}