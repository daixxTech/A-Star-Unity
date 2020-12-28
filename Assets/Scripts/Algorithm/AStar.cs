using System.Collections.Generic;
using UI.Map;
using UnityEngine;
using Utils;

namespace Algorithm {
    /// <summary> AStar 算法 </summary>
    public static class AStar {
        private static readonly Vector2Int[] OFFSET_COORDS; // 偏移坐标
        private static LinkedList<AStarPoint> OpenList = new LinkedList<AStarPoint>();
        private static LinkedList<AStarPoint> CloseList = new LinkedList<AStarPoint>();

        static AStar() {
            // 按照 上 -> 右 -> 下 -> 左 的顺序
            OFFSET_COORDS = new[] {new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1)};
        }

        /// <summary> 搜索下一个坐标点 </summary>
        private static void SearchPoint(AStarPoint curPoint, AStarPoint nextPoint) {
            // 不可到达或在 CloseList 中则返回
            if (!nextPoint.IsReachable || CloseList.Contains(nextPoint)) {
                return;
            }

            // next 不在 OpenList 中
            if (!OpenList.Contains(nextPoint)) {
                // 将 next 加入 OpenList
                OpenList.AddLast(nextPoint);
                // 设置 next 的父节点
                nextPoint.Parent = curPoint;
                // 计算 next 的 G 值
                nextPoint.G = curPoint.G + 1;
            }
            // next 在 OpenList 中，且新的 G 值更小
            else if (curPoint.G + 1 < nextPoint.G) {
                // 设置 next 的父节点
                nextPoint.Parent = curPoint;
                // 计算 next 的 G 值
                nextPoint.G = curPoint.G + 1;
            }
        }

        public static IEnumerable<Vector2Int> GetPath(Block[][] blocks, Vector2Int start, Vector2Int end) {
            // 判断目标坐标是否可到达，不可到达则直接返回
            Block block = blocks[end.x][end.y];
            if (!block.Reachable) {
                return null;
            }

            // 根据 blocks 构造 points
            AStarPoint[][] points = new AStarPoint[blocks.Length][];
            for (int i = 0; i < points.Length; i++) {
                points[i] = new AStarPoint[blocks[i].Length];
                for (int j = 0; j < points[i].Length; j++) {
                    points[i][j] = new AStarPoint(blocks[i][j].Coords, blocks[i][j].Reachable, end);
                }
            }

            bool pathIsFound = false; // 路径标识
            OpenList = new LinkedList<AStarPoint>();
            CloseList = new LinkedList<AStarPoint>();
            // 将起始点加入 OpenList
            OpenList.AddLast(points[start.x][start.y]);
            // 循环条件：OpenList 不为空且还未找到路径
            while (OpenList.Count != 0 && !pathIsFound) {
                // 找到 F 值最小的 point
                AStarPoint point = null;
                foreach (var item in OpenList) {
                    if (point == null || point.F >= item.F) {
                        point = item;
                    }
                }
                // 将 point 移出 OpenList，加入 CloseList
                OpenList.Remove(point);
                CloseList.AddLast(point);
                // 依次搜索周围四个点
                foreach (var delta in OFFSET_COORDS) {
                    Vector2Int nextCoords = point.Coords + delta;
                    // 坐标不合法则返回
                    if (!MapUIUtil.IsBlockReachable(blocks, nextCoords)) {
                        continue;
                    }
                    // 搜索 next
                    AStarPoint nextPoint = points[nextCoords.x][nextCoords.y];
                    SearchPoint(point, nextPoint);
                    // 搜索点为 end 说明找到路径了，直接跳出循环
                    if (nextPoint.Coords == end) {
                        pathIsFound = true;
                        break;
                    }
                }
            }

            // 未找到路径则返回 null
            if (!pathIsFound) {
                return null;
            }
            // 找到路径则从 end 开始依次读取父节点并加入 path，最后返回 path
            LinkedList<Vector2Int> path = new LinkedList<Vector2Int>();
            AStarPoint wayPoint = points[end.x][end.y];
            while (wayPoint != null) {
                path.AddFirst(wayPoint.Coords);
                wayPoint = wayPoint.Parent;
            }
            return path;
        }
    }
}