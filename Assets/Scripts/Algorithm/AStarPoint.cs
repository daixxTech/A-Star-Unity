using System;
using UnityEngine;

namespace Algorithm {
    public class AStarPoint {
        public AStarPoint(Vector2Int coords, bool isReachable, Vector2Int endCoords) {
            Coords = coords;
            IsReachable = isReachable;
            H = Math.Abs(Coords.x - endCoords.x) + Math.Abs(Coords.y - endCoords.y);
        }

        /// <summary> 坐标 </summary>
        public Vector2Int Coords { get; }

        /// <summary> 是否可到达 </summary>
        public bool IsReachable { get; }

        /// <summary> 父节点 </summary>
        public AStarPoint Parent { get; set; }

        public int F => G + H;

        public int G { get; set; }

        public int H { get; }
    }
}