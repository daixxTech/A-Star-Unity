using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace UI.Map {
    public enum Direction {
        Up,
        Down,
        Left,
        Right
    }

    public class Player : MonoBehaviour {
        private const float KEEP_MOVING_INTERVAL = 0.25F; // 保持移动的时间间隔
        private readonly Dictionary<Direction, float> _movingBuffers = new Dictionary<Direction, float> {
            {Direction.Up, KEEP_MOVING_INTERVAL},
            {Direction.Down, KEEP_MOVING_INTERVAL},
            {Direction.Left, KEEP_MOVING_INTERVAL},
            {Direction.Right, KEEP_MOVING_INTERVAL}
        }; // 移动缓冲
        private Vector2Int _curCoords;
        private Coroutine _movingCoroutine;
        private Block[][] _blocks;

        /// <summary> 当前坐标 </summary>
        public Vector2Int CurCoords {
            get => _curCoords;
            private set {
                if (!MapUIUtil.IsBlockReachable(_blocks, value)) {
                    return;
                }
                _curCoords = value;
                transform.position = _blocks[_curCoords.x][_curCoords.y].transform.position;
            }
        }

        private void Update() {
            DoMovement();
        }

        ///<summary> 初始化数据 </summary>
        public void InitData(Block[][] blocks) {
            _blocks = blocks;
        }

        /// <summary> 重新生成玩家 </summary>
        public void Rebuild() {
            // 若移动协程不为 null 则停止并置为 null
            if (_movingCoroutine != null) {
                StopCoroutine(_movingCoroutine);
                _movingCoroutine = null;
            }
            // 随机生成玩家的初始位置
            Vector2Int start;
            do {
                start = new Vector2Int(Random.Range(0, _blocks.Length), Random.Range(0, _blocks[0].Length));
            } while (!_blocks[start.x][start.y].Reachable);
            CurCoords = start;
        }

        /// <summary> 读取键盘输入进行移动 </summary>
        private void DoMovement() {
            // 若移动协程不为 null 则返回
            if (_movingCoroutine != null) {
                return;
            }

            // 键位处于按下状态则对应方向保持移动
            if (Input.GetKey(KeyCode.W)) {
                KeepMoving(Direction.Up);
            }
            if (Input.GetKey(KeyCode.S)) {
                KeepMoving(Direction.Down);
            }
            if (Input.GetKey(KeyCode.A)) {
                KeepMoving(Direction.Left);
            }
            if (Input.GetKey(KeyCode.D)) {
                KeepMoving(Direction.Right);
            }
            // 键位松开则对应方向的移动缓冲重置
            if (Input.GetKeyUp(KeyCode.W)) {
                _movingBuffers[Direction.Up] = KEEP_MOVING_INTERVAL;
            }
            if (Input.GetKeyUp(KeyCode.S)) {
                _movingBuffers[Direction.Down] = KEEP_MOVING_INTERVAL;
            }
            if (Input.GetKeyUp(KeyCode.A)) {
                _movingBuffers[Direction.Left] = KEEP_MOVING_INTERVAL;
            }
            if (Input.GetKeyUp(KeyCode.D)) {
                _movingBuffers[Direction.Right] = KEEP_MOVING_INTERVAL;
            }
        }

        /// <summary> 保持移动 </summary>
        private void KeepMoving(Direction direction) {
            _movingBuffers[direction] += Time.deltaTime; // 更新对应方向的移动缓冲
            // 判断缓冲是否大于间隔
            if (_movingBuffers[direction] >= KEEP_MOVING_INTERVAL) {
                DoMovement(direction); // 玩家向对应方向移动
                _movingBuffers[direction] = 0; // 缓冲置 0
            }
        }

        /// <summary> 根据输入方向进行移动 </summary>
        private void DoMovement(Direction direction) {
            switch (direction) {
            case Direction.Up:
                CurCoords = new Vector2Int(_curCoords.x + 1, _curCoords.y);
                break;
            case Direction.Down:
                CurCoords = new Vector2Int(_curCoords.x - 1, _curCoords.y);
                break;
            case Direction.Left:
                CurCoords = new Vector2Int(_curCoords.x, _curCoords.y - 1);
                break;
            case Direction.Right:
                CurCoords = new Vector2Int(_curCoords.x, _curCoords.y + 1);
                break;
            }
        }

        ///<summary> 开始移动到目标点 </summary>
        public void StartMoveToDestination(IEnumerable<Vector2Int> wayPoints) {
            // 若移动协程不为 null 则停止并置为 null
            if (_movingCoroutine != null) {
                StopCoroutine(_movingCoroutine);
                _movingCoroutine = null;
            }
            if (wayPoints != null) {
                _movingCoroutine = StartCoroutine(MoveToDestination(wayPoints));
            }
        }

        /// <summary> 移动协程 </summary>
        private IEnumerator MoveToDestination(IEnumerable<Vector2Int> wayPoints) {
            int number = 0; // 路径编号
            foreach (var coords in wayPoints) {
                // 根据坐标进行移动
                int deltaX = coords.x - _curCoords.x;
                switch (deltaX) {
                case -1:
                    DoMovement(Direction.Down);
                    break;
                case 1:
                    DoMovement(Direction.Up);
                    break;
                }
                int deltaY = coords.y - _curCoords.y;
                switch (deltaY) {
                case -1:
                    DoMovement(Direction.Left);
                    break;
                case 1:
                    DoMovement(Direction.Right);
                    break;
                }
                CurCoords = coords;
                // 设置颜色及路径编号
                _blocks[_curCoords.x][_curCoords.y].SetColor(Color.yellow);
                _blocks[_curCoords.x][_curCoords.y].SetNumber(++number);
                yield return CoroutineUtil.DOT_ONE_SECONDS;
            }
            _movingCoroutine = null;
        }
    }
}