using System.Collections;
using System.Collections.Generic;
using Algorithm;
using Maps;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     移动方向
/// </summary>
public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class Player : MonoBehaviour
{
    private const float _KEEP_MOVING_INTERVAL = 0.25F; // 保持移动的时间间隔

    [SerializeField] private Vector2Int _curCoords; // 当前坐标
    [SerializeField] private Vector2Int _destination; // 目标坐标
    private Dictionary<Direction, float> _movingBuffers; // 移动缓冲
    private Coroutine _movingCoroutine; // 移动协程

    public Text PathInfoTxt; // 路径信息提示文本

    /// <summary>
    ///     单例
    /// </summary>
    public static Player Instance { get; private set; }

    /// <summary>
    ///     当前坐标
    /// </summary>
    public Vector2Int CurCoords {
        get => _curCoords;
        private set {
            // 坐标不合法或不可达则返回
            if (!Map.ValidateCoords(value) || !Map.Instance.Blocks[value.x][value.y].IsReachable) {
                return;
            }
            _curCoords = value;
            // 设置坐标映射到地图中的位置
            transform.localPosition = Map.Coords2Position(_curCoords);
        }
    }

    /// <summary>
    ///     目标坐标
    /// </summary>
    public Vector2Int Destination {
        set {
            // 目标坐标与当前坐标相等则返回
            if (value == CurCoords) {
                return;
            }
            _destination = value;
            // 计算路径并移动到目标坐标
            ComputePathAndMoveToDestination();
        }
    }

    private void Awake()
    {
        // 处理单例
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        // 初始化移动缓冲
        _movingBuffers = new Dictionary<Direction, float> {
            {Direction.Up, _KEEP_MOVING_INTERVAL}, {Direction.Down, _KEEP_MOVING_INTERVAL},
            {Direction.Left, _KEEP_MOVING_INTERVAL}, {Direction.Right, _KEEP_MOVING_INTERVAL}
        };
        // 设置玩家的缩放系数
        transform.localScale = new Vector3(Block.SCALE, Block.SCALE, 1);
    }

    private void Start()
    {
        Rebuild();
    }

    private void Update()
    {
        DoMovement();
    }

    /// <summary>
    ///     重新生成玩家
    /// </summary>
    public void Rebuild()
    {
        // 移动协程不为 null 则停止并置为 null
        if (_movingCoroutine != null) {
            StopCoroutine(_movingCoroutine);
            _movingCoroutine = null;
        }
        // 隐藏路径提示文本
        PathInfoTxt.gameObject.SetActive(false);
        // 随机生成玩家的初始位置
        Vector2Int start;
        do {
            start = new Vector2Int(Random.Range(0, Map.VERTICAL_BLOCK_COUNT),
                Random.Range(0, Map.HORIZONTAL_BLOCK_COUNT));
        } while (!Map.Instance.Blocks[start.x][start.y].IsReachable);
        CurCoords = start;
    }

    /// <summary>
    ///     保持移动
    /// </summary>
    private void KeepMoving(Direction direction)
    {
        // 更新对应方向的移动缓冲
        _movingBuffers[direction] += Time.deltaTime;
        // 判断缓冲是否大于间隔
        if (_movingBuffers[direction] >= _KEEP_MOVING_INTERVAL) {
            // 玩家向对应方向移动
            DoMovement(direction);
            // 缓冲置 0
            _movingBuffers[direction] = 0;
        }
    }

    /// <summary>
    ///     读取键盘输入进行移动
    /// </summary>
    private void DoMovement()
    {
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
            _movingBuffers[Direction.Up] = _KEEP_MOVING_INTERVAL;
        }
        if (Input.GetKeyUp(KeyCode.S)) {
            _movingBuffers[Direction.Down] = _KEEP_MOVING_INTERVAL;
        }
        if (Input.GetKeyUp(KeyCode.A)) {
            _movingBuffers[Direction.Left] = _KEEP_MOVING_INTERVAL;
        }
        if (Input.GetKeyUp(KeyCode.D)) {
            _movingBuffers[Direction.Right] = _KEEP_MOVING_INTERVAL;
        }
    }

    /// <summary>
    ///     根据输入方向进行移动
    /// </summary>
    private void DoMovement(Direction direction)
    {
        if (direction == Direction.Up) {
            CurCoords = new Vector2Int(CurCoords.x + 1, CurCoords.y);
        } else if (direction == Direction.Down) {
            CurCoords = new Vector2Int(CurCoords.x - 1, CurCoords.y);
        } else if (direction == Direction.Left) {
            CurCoords = new Vector2Int(CurCoords.x, CurCoords.y - 1);
        } else if (direction == Direction.Right) {
            CurCoords = new Vector2Int(CurCoords.x, CurCoords.y + 1);
        }
    }

    /// <summary>
    ///     计算路径并移动到目标坐标
    /// </summary>
    private void ComputePathAndMoveToDestination()
    {
        // 计算路径
        IEnumerable<Vector2Int> path = AStar.GetPath(CurCoords, _destination);
        // 移动协程不为 null 则停止并置为 null
        if (_movingCoroutine != null) {
            StopCoroutine(_movingCoroutine);
            _movingCoroutine = null;
        }
        // 路径为空则无法抵达目标坐标
        if (path == null) {
            // 显示路径提示信息文本
            PathInfoTxt.gameObject.SetActive(true);
            return;
        }
        // 隐藏路径提示信息文本
        PathInfoTxt.gameObject.SetActive(false);
        // 开启移动协程
        _movingCoroutine = StartCoroutine(MoveToDestination(path));
    }

    /// <summary>
    ///     移动协程
    /// </summary>
    private IEnumerator MoveToDestination(IEnumerable<Vector2Int> path)
    {
        int number = 0; // 路径编号
        foreach (var coords in path) {
            // 根据坐标进行移动
            int deltaX = coords.x - CurCoords.x;
            if (deltaX == -1) {
                DoMovement(Direction.Down);
            } else if (deltaX == 1) {
                DoMovement(Direction.Up);
            }
            int deltaY = coords.y - CurCoords.y;
            if (deltaY == -1) {
                DoMovement(Direction.Left);
            } else if (deltaY == 1) {
                DoMovement(Direction.Right);
            }
            // 更新当前坐标
            CurCoords = coords;
            // 路径点染色
            Map.Instance.Blocks[CurCoords.x][CurCoords.y].SetImgColor(Color.yellow);
            // 设置路径编号
            Map.Instance.Blocks[CurCoords.x][CurCoords.y].SetNumberTxt(true, ++number);
            // 迭代间隔 0.1s
            yield return Yielder.DotOneSeconds;
        }
        // 移动协程结束，置为 null
        _movingCoroutine = null;
    }
}