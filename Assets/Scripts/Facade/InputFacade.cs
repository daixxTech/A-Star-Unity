using System;
using UnityEngine;

namespace Facade {
    public class InputFacade {
        public static Action<KeyCode, Action> AddKeyDownAction;
        public static Action<KeyCode, Action> RemoveKeyDownAction;
    }
}