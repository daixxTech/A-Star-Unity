using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Maps
{
    /// <summary>
    ///     地图区块
    /// </summary>
    public class Block : MonoBehaviour, IPointerClickHandler
    {
        public const int SOURCE_SIZE = 120; // 原始尺寸
        public const int SIZE = Map.WIDTH / Map.HORIZONTAL_BLOCK_COUNT; // 实际尺寸
        public const float SCALE = 1.0F * SIZE / SOURCE_SIZE; // 缩放系数

        private Vector2Int _coords; // 坐标
        private bool _isReachable; // 是否可到达

        public Image Img; // 图像
        public Text CoordsTxt; // 坐标文本
        public Text NumberTxt; // 路径编号文本

        /// <summary>
        ///     坐标
        /// </summary>
        public Vector2Int Coords {
            get => _coords;
            internal set {
                _coords = value;
                // 修改坐标文本
                CoordsTxt.text = $"({_coords.x}, {_coords.y})";
                // 设置坐标映射到地图中的位置
                transform.localPosition = Map.Coords2Position(_coords);
            }
        }

        /// <summary>
        ///     是否可到达
        /// </summary>
        public bool IsReachable {
            get => _isReachable;
            internal set {
                _isReachable = value;
                // 设置区块的颜色
                SetImgColor();
            }
        }

        /// <summary>
        ///     处理单击事件
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            // 恢复地图的初始状态
            Map.Instance.Clear();
            // 设置玩家的目标坐标
            Player.Instance.Destination = Coords;
        }

        /// <summary>
        ///     恢复区块的默认颜色，可到达为白色，不可到达为红色
        /// </summary>
        public void SetImgColor()
        {
            Img.color = _isReachable ? Color.white : Color.red;
        }

        /// <summary>
        ///     设置图像的颜色
        /// </summary>
        public void SetImgColor(Color color)
        {
            Img.color = color;
        }

        /// <summary>
        ///     设置坐标文本的激活状态
        /// </summary>
        public void SetCoordsActive()
        {
            // 切换状态（当前为显示则隐藏，当前为隐藏则显示）
            CoordsTxt.gameObject.SetActive(!CoordsTxt.gameObject.activeSelf);
        }

        /// <summary>
        ///     设置路径编号文本
        /// </summary>
        public void SetNumberTxt(bool visible, int number = 0)
        {
            // 设置显示状态
            NumberTxt.gameObject.SetActive(visible);
            // 若文本可见则修改文本
            if (visible) {
                NumberTxt.text = number.ToString();
            }
        }
    }
}