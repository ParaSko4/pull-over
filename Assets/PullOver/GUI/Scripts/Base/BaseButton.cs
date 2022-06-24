using System;
using UnityEngine;
using UnityEngine.UI;

namespace GUI.Base
{
    [RequireComponent(typeof(Button))]
    public abstract class BaseButton : MonoBehaviour
    {
        private event Action onClickAction;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();

            OnAwake();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
        }

        public void SetOnClickAction(Action onClickAction)
        {
            this.onClickAction = onClickAction;
        }

        protected virtual void OnAwake() { }

        /// <summary>
        /// If u wanna click button from code
        /// </summary>
        protected virtual void OnClick()
        {
            if (gameObject.activeSelf)
            {
                onClickAction();
            }
            else
            {
                throw new InvalidOperationException("Button was disabled");
            }
        }
    }
}
