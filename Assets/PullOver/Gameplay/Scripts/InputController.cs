using PullOver.Gameplay.Cars.Enum;
using System;
using UnityEngine;

namespace PullOver.Gameplay
{
    public class InputController : MonoBehaviour
    {
        public event Action<TurningSide> Swiped;

        private Vector3 firstTouch;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                firstTouch = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (firstTouch.x < Input.mousePosition.x)
                {
                    Swiped?.Invoke(TurningSide.Right);
                }
                else if (firstTouch.x > Input.mousePosition.x)
                {
                    Swiped?.Invoke(TurningSide.Left);
                }
            }
        }
    }
}
