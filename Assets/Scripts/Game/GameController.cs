using Maps;
using UnityEngine;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        private void Update()
        {
            // ESC 退出
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Exit();
            }
            // Enter 重新生成地图
            if (Input.GetKeyDown(KeyCode.Return)) {
                Rebuild();
            }
            // C 切换地图区块坐标文本的激活状态
            if (Input.GetKeyDown(KeyCode.C)) {
                SetMapBlockCoordsActive();
            }
        }

        /// <summary>
        ///     退出
        /// </summary>
        private static void Exit()
        {
            Application.Quit();
        }

        /// <summary>
        ///     重新生成地图及玩家
        /// </summary>
        private static void Rebuild()
        {
            Map.Instance.Rebuild();
            Player.Instance.Rebuild();
        }

        /// <summary>
        ///     切换地图区块坐标文本的激活状态
        /// </summary>
        private static void SetMapBlockCoordsActive()
        {
            Map.Instance.SetBlockCoordsActive();
        }
    }
}