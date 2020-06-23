using UnityEngine;

namespace Maps
{
    /// <summary>
    ///     地图
    /// </summary>
    public class Map : MonoBehaviour
    {
        public const int WIDTH = 1920; // 地图宽度
        public const int HEIGHT = 1080; // 地图高度
        public const int HORIZONTAL_BLOCK_COUNT = 32; // 水平区块数量
        public const int VERTICAL_BLOCK_COUNT = 18; // 垂直区块数量

        /// <summary>
        ///     单例
        /// </summary>
        public static Map Instance { get; private set; }

        /// <summary>
        ///     边界
        /// </summary>
        public Bounds4 Bounds { get; private set; }

        /// <summary>
        ///     区块
        /// </summary>
        public Block[][] Blocks { get; set; }

        /// <summary>
        ///     判断坐标是否合法，即是否在边界内
        /// </summary>
        public static bool ValidateCoords(Vector2Int coords)
        {
            return coords.x >= 0 && coords.x < VERTICAL_BLOCK_COUNT &&
                   coords.y >= 0 && coords.y < HORIZONTAL_BLOCK_COUNT;
        }

        /// <summary>
        ///     坐标映射到地图中的位置
        /// </summary>
        public static Vector3 Coords2Position(Vector2Int coords)
        {
            return new Vector3(Instance.Bounds.Left + Block.SIZE * (coords.y + 0.5F),
                Instance.Bounds.Down + Block.SIZE * (coords.x + 0.5F), 0);
        }

        private void Awake()
        {
            // 处理单例
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(this);
            }

            // 初始化边界
            Bounds = new Bounds4(HEIGHT, WIDTH);

            // 初始化区块
            Blocks = new Block[VERTICAL_BLOCK_COUNT][];
            // 加载区块预制件
            GameObject blockPrefabs = Resources.Load<GameObject>("Prefabs/Maps/Block");
            for (int i = 0; i < VERTICAL_BLOCK_COUNT; i++) {
                Blocks[i] = new Block[HORIZONTAL_BLOCK_COUNT];
                for (int j = 0; j < HORIZONTAL_BLOCK_COUNT; j++) {
                    // 实例化区块预制件
                    Blocks[i][j] = Instantiate(blockPrefabs, transform).GetComponent<Block>();
                    // 设置区块缩放系数
                    Blocks[i][j].transform.localScale = new Vector3(Block.SCALE, Block.SCALE, 1);
                    // 设置区块坐标
                    Blocks[i][j].Coords = new Vector2Int(i, j);
                    // 设置区块是否可到达
                    Blocks[i][j].IsReachable = Random.Range(0, 3) != 0;
                }
            }
        }

        /// <summary>
        ///     设置所有区块的坐标文本的激活状态
        /// </summary>
        public void SetBlockCoordsActive()
        {
            foreach (var row in Blocks) {
                foreach (var block in row) {
                    block.SetCoordsActive();
                }
            }
        }

        /// <summary>
        ///     重新生成地图
        /// </summary>
        public void Rebuild()
        {
            for (int i = 0; i < VERTICAL_BLOCK_COUNT; i++) {
                for (int j = 0; j < HORIZONTAL_BLOCK_COUNT; j++) {
                    // 设置区块是否可到达
                    Blocks[i][j].IsReachable = Random.Range(0, 3) != 0;
                    // 隐藏路径编号文本
                    Blocks[i][j].SetNumberTxt(false);
                }
            }
        }

        /// <summary>
        ///     恢复地图的初始状态
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < VERTICAL_BLOCK_COUNT; i++) {
                for (int j = 0; j < HORIZONTAL_BLOCK_COUNT; j++) {
                    // 区块恢复默认颜色
                    Blocks[i][j].SetImgColor();
                    // 隐藏路径编号文本
                    Blocks[i][j].SetNumberTxt(false);
                }
            }
        }

        /// <summary>
        ///     四向边界
        /// </summary>
        public class Bounds4
        {
            public Bounds4(int height, int width)
            {
                Up = height / 2;
                Right = width / 2;
                Down = -Up;
                Left = -Right;
            }

            public int Up { get; }

            public int Down { get; }

            public int Left { get; }

            public int Right { get; }
        }
    }
}