using System.Collections.Generic;
using Facade;
using Modules;
using UI;
using UnityEngine;

public class Main : MonoBehaviour {
    private List<BaseModule> _moduleList;
    private Dictionary<string, Transform> _uiDict;
    private GameObject _uiRoot;

    public static Main Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(this);
        }

        _moduleList = new List<BaseModule>();
        _moduleList.Add(new InputModule());
        foreach (var module in _moduleList) {
            module.Awake();
        }

        _uiRoot = GameObject.Find("UIRoot");
        _uiDict = new Dictionary<string, Transform>();
    }

    private void Start() {
        InputFacade.AddKeyDownAction?.Invoke(KeyCode.Escape, Quit);
        ShowUI(UIDef.MAP_UI);
    }

    private void Update() {
        foreach (var module in _moduleList) {
            module.Update();
        }
    }

    public void OnDestroy() {
        InputFacade.RemoveKeyDownAction?.Invoke(KeyCode.Escape, Quit);

        foreach (var module in _moduleList) {
            module.Dispose();
        }
    }

    ///<summary> 显示 UI </summary>
    public void ShowUI(string uiName) {
        if (_uiDict.TryGetValue(uiName, out Transform uiTrans)) {
            uiTrans.gameObject.SetActive(true);
        } else {
            uiTrans = _uiRoot.transform.Find(uiName);
            if (uiTrans != null) {
                uiTrans.gameObject.SetActive(true);
                _uiDict.Add(uiName, uiTrans);
            }
        }
    }

    ///<summary> 隐藏 UI </summary>
    public void HideUI(string uiName) {
        if (_uiDict.TryGetValue(uiName, out Transform uiTrans)) {
            uiTrans.gameObject.SetActive(false);
        } else {
            uiTrans = _uiRoot.transform.Find(uiName);
            if (uiTrans != null) {
                uiTrans.gameObject.SetActive(false);
            }
        }
    }

    public void Quit() {
        Application.Quit();
    }
}