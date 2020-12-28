using System;
using System.Collections.Generic;
using Facade;
using UnityEngine;

namespace Modules {
    public class InputModule : BaseModule {
        private Dictionary<KeyCode, Action> _onKeyDown;

        public override void Awake() {
            _onKeyDown = new Dictionary<KeyCode, Action>();
            InputFacade.AddKeyDownAction += AddKeyDownAction;
            InputFacade.RemoveKeyDownAction += RemoveKeyDownAction;
        }

        public override void Update() {
            foreach (var pair in _onKeyDown) {
                if (Input.GetKeyDown(pair.Key)) {
                    pair.Value?.Invoke();
                }
            }
        }

        public override void Dispose() {
            InputFacade.AddKeyDownAction -= AddKeyDownAction;
            InputFacade.RemoveKeyDownAction -= RemoveKeyDownAction;
        }

        private void AddKeyDownAction(KeyCode code, Action action) {
            if (_onKeyDown.ContainsKey(code)) {
                _onKeyDown[code] += action;
            } else {
                _onKeyDown.Add(code, action);
            }
        }

        private void RemoveKeyDownAction(KeyCode code, Action action) {
            if (_onKeyDown.ContainsKey(code)) {
                _onKeyDown[code] -= action;
            }
        }
    }
}