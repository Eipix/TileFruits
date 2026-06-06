using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using PlayerInput = Input.PlayerInput;

namespace View
{
    public class InputMapsManager : MonoBehaviour
    {
        [Inject] private PlayerInput _input;

        private void Update()
        {
            if(EventSystem.current.IsPointerOverGameObject())
                _input.Player.Disable();
            else
                _input.Player.Enable();
        }
    }
}
