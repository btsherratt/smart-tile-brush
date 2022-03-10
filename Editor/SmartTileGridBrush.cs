using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor.Tilemaps;

namespace SmartTileBrush {
    [CustomGridBrush(true, false, false, "Smart Brush")]
    public class SmartTileGridBrush : GridBrush {
        public SmartTileGroupCollection m_tileGroupCollection;

        /*public override void Select(GridLayout gridLayout, GameObject brushTarget, BoundsInt position) {
            base.Select(gridLayout, brushTarget, position);
        }*/

        /*public override void Move(GridLayout gridLayout, GameObject brushTarget, BoundsInt from, BoundsInt to) {
            base.Move(gridLayout, brushTarget, from, to);
        }*/

        /*public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position) {
            base.Paint(gridLayout, GetTrueBrushTarget(gridLayout, position), position);
        }*/ // NB: implemented using BoxFill

        /*public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position) {
            base.Erase(gridLayout, GetTrueBrushTarget(gridLayout, position), position);
        }*/ // NB: implemented using BoxErase

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position) {
            BoundsInt safePosition;
            foreach (GameObject trueTarget in GetTrueBrushTargets(out safePosition, gridLayout, position, cells, true)) {
                base.BoxFill(gridLayout, trueTarget, safePosition);
            }
        }

        public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position) {
            BoundsInt safePosition;
            foreach (GameObject trueTarget in GetTrueBrushTargets(out safePosition, gridLayout, position, null, false)) {
                Tilemap targetTilemap = trueTarget.GetComponent<Tilemap>();
                if (System.Array.Exists(targetTilemap.GetTilesBlock(safePosition), (TileBase tile) => tile != null)) {
                    base.BoxErase(gridLayout, trueTarget, safePosition);
                    break;
                }
            }
        }

        public override void FloodFill(GridLayout gridLayout, GameObject brushTarget, Vector3Int position) {
            Vector3Int safePosition;
            foreach (GameObject trueTarget in GetTrueBrushTargets(out safePosition, gridLayout, position, cells, true)) {
                base.FloodFill(gridLayout, trueTarget, safePosition);
            }
        }

        internal GameObject[] GetTrueBrushTargets(out Vector3Int safePosition, GridLayout gridLayout, Vector3Int position, BrushCell[] brushCells, bool createMissingTargets) {
            BoundsInt safePositionBounds;
            BoundsInt boundsPosition = new BoundsInt(position, Vector3Int.one);
            GameObject[] trueTargets = GetTrueBrushTargets(out safePositionBounds, gridLayout, boundsPosition, brushCells, createMissingTargets);
            safePosition = safePositionBounds.position;
            return trueTargets;
        }

        internal GameObject[] GetTrueBrushTargets(out BoundsInt safeBounds, GridLayout gridLayout, BoundsInt position, BrushCell[] brushCells, bool createMissingTargets) {
            List<GameObject> targets = new List<GameObject>();

            SmartTileRoom room = GetTargetRoom(out safeBounds, gridLayout, position);
            if (room != null) {
                foreach (SmartTileGroup tileGroup in GetSmartTileGroups(brushCells)) {
                    string targetName = $"Group_{tileGroup.name}";

                    Transform targetTransform = room.transform.Find(targetName);
                    if (targetTransform == null && createMissingTargets) {
                        GameObject targetHost = new GameObject(targetName);
                        targetHost.transform.SetParent(room.transform);
                        targetHost.AddComponent<Tilemap>();
                        TilemapRenderer tr = targetHost.AddComponent<TilemapRenderer>();
                        tr.sortingOrder = tileGroup.m_layer;
                        targetTransform = targetHost.transform;
                    }

                    if (targetTransform != null) {
                        GameObject target = targetTransform.gameObject;
                        if (targets.Contains(target) == false) {
                            targets.Add(target);
                        }
                    }
                }
            }

            targets.Sort(new System.Comparison<GameObject>((lhs, rhs) => rhs.GetComponent<TilemapRenderer>().sortingOrder - lhs.GetComponent<TilemapRenderer>().sortingOrder));

            return targets.ToArray();
        }

        IEnumerable<SmartTileGroup> GetSmartTileGroups(BrushCell[] brushCells) {
            foreach (SmartTileGroup tileGroup in SmartTileGroupUtility.GetCollectionGroups(m_tileGroupCollection)) {
                if (brushCells == null) {
                    yield return tileGroup;
                } else {
                    foreach (BrushCell brushCell in brushCells) {
                        if (System.Array.IndexOf(tileGroup.m_tiles, brushCell.tile) >= 0) {
                            yield return tileGroup;
                            break;
                        }
                    }
                }
            }
        }

        SmartTileRoom GetTargetRoom(out BoundsInt safeBounds, GridLayout gridLayout, BoundsInt position) {
            SmartTileRoom[] rooms = gridLayout.GetComponentsInChildren<SmartTileRoom>();
            foreach (SmartTileRoom room in rooms) {
                if (room.m_Bounds.xMin < position.xMax && room.m_Bounds.xMax > position.xMin &&
                    room.m_Bounds.yMin < position.yMax && room.m_Bounds.yMax > position.yMin &&
                    room.m_Bounds.zMin < position.zMax && room.m_Bounds.zMax > position.zMin) {

                    int xMin = Mathf.Max(room.m_Bounds.xMin, position.xMin);
                    int xSize = Mathf.Min(room.m_Bounds.xMax, position.xMax) - xMin;

                    int yMin = Mathf.Max(room.m_Bounds.yMin, position.yMin);
                    int ySize = Mathf.Min(room.m_Bounds.yMax, position.yMax) - yMin;

                    int zMin = Mathf.Max(room.m_Bounds.zMin, position.zMin);
                    int zSize = Mathf.Min(room.m_Bounds.zMax, position.zMax) - zMin;

                    safeBounds = new BoundsInt(xMin, yMin, zMin, xSize, ySize, zSize);

                    return room;
                }
            }

            safeBounds = new BoundsInt();
            return null;
        }
    }
}
