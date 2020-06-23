using System;
using System.Collections.Generic;
using Maps;
using UnityEngine;

namespace Algorithm
{
    /// <summary>
    ///     AStar 算法
    /// </summary>
    public static class AStar
    {
        private static readonly Vector2Int[] _OffsetCoords; // 偏移坐标
        private static LinkedList<Point> _OpenList = new LinkedList<Point>();
        private static LinkedList<Point> _CloseList = new LinkedList<Point>();

        static AStar()
        {
            // 按照 上 -> 右 -> 下 -> 左 的顺序
            _OffsetCoords = new[] {
                new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1)
            };
        }

        /// <summary>
        ///     判断坐标点是否可到达且不在 CloseList 中
        /// </summary>
        private static bool IsReachableAndNotInCloseList(Point point)
        {
            return point.IsReachable && !_CloseList.Contains(point);
        }

        /// <summary>
        ///     计算曼哈顿距离
        /// </summary>
        private static int GetManhattanDistance(Vector2Int start, Vector2Int end)
        {
            return Math.Abs(start.x - end.x) + Math.Abs(start.y - end.y);
        }

        /// <summary>
        ///     搜索下一个坐标点
        /// </summary>
        private static void SearchPoint(Point next, Point cur)
        {
            // 不可到达或在 CloseList 中则返回
            if (!IsReachableAndNotInCloseList(next)) {
                return;
            }

            // next 不在 OpenList 中
            if (!_OpenList.Contains(next)) {
                // 将 next 加入 OpenList
                _OpenList.AddLast(next);
                // 设置 next 的父节点
                next.Parent = cur;
                // 计算 next 的 G 值
                next.G = cur.G + 1;
            }
            // next 在 OpenList 中，且新的 G 值更小
            else if (cur.G + 1 < next.G) {
                // 设置 next 的父节点
                next.Parent = cur;
                // 计算 next 的 G 值
                next.G = cur.G + 1;
            }
        }

        public static IEnumerable<Vector2Int> GetPath(Vector2Int start, Vector2Int end)
        {
            // 判断目标坐标是否可到达，不可到达则直接返回
            Block[][] blocks = Map.Instance.Blocks;
            if (!blocks[end.x][end.y].IsReachable) {
                return null;
            }

            // 根据 blocks 构造 points
            Point[][] points = new Point[Map.VERTICAL_BLOCK_COUNT][];
            for (int i = 0; i < points.Length; i++) {
                points[i] = new Point[Map.HORIZONTAL_BLOCK_COUNT];
                for (int j = 0; j < points[i].Length; j++) {
                    points[i][j] = new Point(blocks[i][j].Coords, blocks[i][j].IsReachable, end);
                }
            }

            bool pathIsFound = false; // 路径标识
            _OpenList = new LinkedList<Point>();
            _CloseList = new LinkedList<Point>();
            // 将起始点加入 OpenList
            _OpenList.AddLast(points[start.x][start.y]);
            // 循环条件：OpenList 不为空且还未找到路径
            while (_OpenList.Count != 0 && !pathIsFound) {
                // 找到 F 值最小的 point
                Point point = null;
                foreach (var item in _OpenList) {
                    if (point == null || point.F >= item.F) {
                        point = item;
                    }
                }
                // 将 point 移出 OpenList，加入 CloseList
                _OpenList.Remove(point);
                _CloseList.AddLast(point);
                // 依次搜索周围四个点
                foreach (var delta in _OffsetCoords) {
                    Vector2Int nextCoords = point.Coords + delta;
                    // 坐标不合法则返回
                    if (!Map.ValidateCoords(nextCoords)) {
                        continue;
                    }
                    // 搜索 next
                    Point next = points[nextCoords.x][nextCoords.y];
                    SearchPoint(next, point);
                    // 搜索点为 end 说明找到路径了，直接跳出循环
                    if (next == points[end.x][end.y]) {
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
            Point wayPoint = points[end.x][end.y];
            while (wayPoint != null) {
                path.AddFirst(wayPoint.Coords);
                wayPoint = wayPoint.Parent;
            }
            return path;
        }

        public class Point
        {
            public Point(Vector2Int coords, bool isReachable, Vector2Int endCoords)
            {
                Coords = coords;
                IsReachable = isReachable;
                H = GetManhattanDistance(Coords, endCoords);
            }

            /// <summary>
            ///     坐标
            /// </summary>
            public Vector2Int Coords { get; }

            /// <summary>
            ///     是否可到达
            /// </summary>
            public bool IsReachable { get; }

            /// <summary>
            ///     父节点
            /// </summary>
            public Point Parent { get; set; }

            public int F => G + H;

            public int G { get; set; }

            public int H { get; }
        }
    }
}