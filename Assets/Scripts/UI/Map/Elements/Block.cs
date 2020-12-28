using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Map {
    public class Block : MonoBehaviour, IPointerClickHandler {
        public Image iconImg;
        public Text coordsTxt;
        public Text numberTxt;

        private Vector2Int _coords;
        private bool _reachable;
        public Action<Vector2Int> onClick;

        public Vector2Int Coords => _coords;
        public bool Reachable => _reachable;

        private void Awake() {
            iconImg = transform.GetComponent<Image>();
            coordsTxt = transform.Find("Coords").GetComponent<Text>();
            numberTxt = transform.Find("Number").GetComponent<Text>();
        }

        ///<summary> 初始化数据 </summary>
        public void InitData(Vector2Int coords, Action<Vector2Int> onBlockClick) {
            _coords = coords;
            coordsTxt.text = $"({_coords.y.ToString()}, {_coords.x.ToString()})"; // coords 其实是数组下标，因此显示时要转换为地图坐标 
            onClick = onBlockClick;
            Rebuild();
        }

        ///<summary> 恢复初始状态 </summary>
        public void Reset() {
            InitColor();
            numberTxt.gameObject.SetActive(false);
        }

        ///<summary> 重新生成区块 </summary>
        public void Rebuild() {
            _reachable = UnityEngine.Random.Range(0, 3) != 0;
            Reset();
        }

        /// <summary> 初始化区块颜色，可到达为白色，不可到达为红色 </summary>
        public void InitColor() {
            SetColor(_reachable ? Color.white : Color.red);
        }

        ///<summary> 设置颜色 </summary>
        public void SetColor(Color color) {
            iconImg.color = color;
        }

        /// <summary> 设置坐标文本的显示状态 </summary>
        public void SwitchCoordsActive() {
            bool activeSelf = coordsTxt.gameObject.activeSelf;
            coordsTxt.gameObject.SetActive(!activeSelf);
        }

        /// <summary> 设置路径编号文本 </summary>
        public void SetNumber(int number = 0) {
            numberTxt.gameObject.SetActive(true);
            numberTxt.text = number.ToString();
        }

        /// <summary> 点击区块 </summary>
        public void OnPointerClick(PointerEventData eventData) {
            onClick?.Invoke(_coords);
        }
    }
}