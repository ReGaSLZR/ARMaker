using System;
using UnityEngine;

namespace ARMarker
{
    public abstract class BaseWorkLayerHandler : MonoBehaviour
    {

        protected Action onSelect;

        public virtual void Select()
        {
            onSelect?.Invoke();
        }

        public abstract void Deselect();

        public void RegisterListener(Action listener)
        {
            if (listener == null)
            {
                return;
            }

            onSelect += listener;
        }

    }

}