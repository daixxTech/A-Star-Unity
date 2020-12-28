using System;
using System.Collections.Generic;
using Algorithm;
using Facade;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Map {
    public class MapUILogic : MonoBehaviour {
        public Transform blockTile;
        public Transform blockPrefabTf;
        public Transform playerTf;

        public Vector2 tileSize;
        public Vector2 blockSize;
        public int updateTimes;
        private Block[][] _blocks;
        private Player _player;

        private void Awake() {
            blockTile = transform.Find("BlockTile");
            blockPrefabTf = blockTile.Find("Block");
            blockPrefabTf.gameObject.AddComponent<Block>();
            playerTf = transform.Find("Player");

            tileSize = blockTile.GetComponent<RectTransform>().sizeDelta;
            blockSize = blockTile.GetComponent<GridLayoutGroup>().cellSize;
            int col = (int) (tileSize.x / blockSize.x), row = (int) (tileSize.y / blockSize.y);
            Action<Vector2Int> onBlockClick = MoveToTargetBlock;
            _blocks = new Block[row][];
            for (int i = 0; i < row; i++) {
                _blocks[i] = new Block[col];
                for (int j = 0; j < col; j++) {
                    _blocks[i][j] = Instantiate(blockPrefabTf, blockTile).GetComponent<Block>();
                    _blocks[i][j].InitData(new Vector2Int(i, j), onBlockClick);
                }
            }
            blockPrefabTf.gameObject.SetActive(false);

            _player = playerTf.gameObject.AddComponent<Player>();
            _player.InitData(_blocks);
        }

        private void OnEnable() {
            InputFacade.AddKeyDownAction?.Invoke(KeyCode.C, SwitchBlockCoordsActive);
            InputFacade.AddKeyDownAction?.Invoke(KeyCode.Return, Rebuild);
        }

        private void Update() {
            /* 比较疑惑的点：
             * GridLayoutGroup 在第 1 次 Update 时才会设置子物体的 transform.position
             * 因此第 1 次设置玩家位置需要在第 2 次 Update 时进行
             */
            ++updateTimes;
            if (updateTimes == 2) {
                Rebuild();
            }
        }

        private void OnDisable() {
            InputFacade.RemoveKeyDownAction?.Invoke(KeyCode.C, SwitchBlockCoordsActive);
            InputFacade.RemoveKeyDownAction?.Invoke(KeyCode.Return, Rebuild);
        }

        /// <summary> 切换所有区块坐标文本的状态 </summary>
        public void SwitchBlockCoordsActive() {
            foreach (var blockRow in _blocks) {
                foreach (var block in blockRow) {
                    block.SwitchCoordsActive();
                }
            }
        }

        /// <summary> 重新生成地图 </summary>
        public void Rebuild() {
            Main.Instance.HideUI(UIDef.TIPS_UI);
            foreach (var blockRow in _blocks) {
                foreach (var block in blockRow) {
                    block.Rebuild();
                }
            }
            _player.Rebuild();
        }

        /// <summary> 移动到目标区块 </summary>
        public void MoveToTargetBlock(Vector2Int coords) {
            // 重置所有区块的状态
            foreach (var blockRow in _blocks) {
                foreach (var block in blockRow) {
                    block.Reset();
                }
            }
            // 进行 AStar 寻路
            IEnumerable<Vector2Int> wayPoints = AStar.GetPath(_blocks, _player.CurCoords, coords);
            if (wayPoints == null) {
                Main.Instance.ShowUI(UIDef.TIPS_UI);
            } else {
                Main.Instance.HideUI(UIDef.TIPS_UI);
            }
            _player.StartMoveToDestination(wayPoints);
        }
    }
}