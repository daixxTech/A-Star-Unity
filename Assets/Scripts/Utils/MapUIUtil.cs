using UI.Map;
using UnityEngine;

namespace Utils {
    public class MapUIUtil {
        ///<summary> 坐标是否可达 </summary>
        public static bool IsBlockReachable(Block[][] blocks, Vector2Int coords) {
            return coords.x >= 0 && coords.x < blocks.Length &&
                   coords.y >= 0 && coords.y < blocks[0].Length &&
                   blocks[coords.x][coords.y].Reachable;
        }
    }
}