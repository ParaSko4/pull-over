using PullOver.GUI.VFX;
using UnityEngine;

namespace PullOver.GUI
{
    [RequireComponent(typeof(Fade))]
    public abstract class BaseScreen : MonoBehaviour
    {
        public Fade Fade { get; private set; }

        private void Awake()
        {
            Fade = GetComponent<Fade>();

            OnAwake();
        }

        protected virtual void OnAwake() { }
    }
}

