using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SmartTileBrush {
    [CustomEditor(typeof(SmartTileGridBrush))]
    public class SmartTileGridBrushEditor : GridBrushEditor {
        SmartTileGridBrush smartTileGridBrush => (SmartTileGridBrush)brush;

        public override void OnSelectionInspectorGUI() {
            base.OnSelectionInspectorGUI();

            BoundsInt selection = GridSelection.position;
            Grid grid = GridSelection.grid;//   .target;
            GameObject target = GridSelection.target;//   .target;

            GUILayout.Space(20.0f);

            if (grid.gameObject.scene.name != "Preview Scene") {
                if (GUILayout.Button("Create Room")) {
                    for (int i = 0; ; ++i) {
                        string targetRoomName = $"Room_{i:D4}";
                        if (grid.transform.Find(targetRoomName) == false) {
                            GameObject roomGameObject = new GameObject(targetRoomName);
                            roomGameObject.transform.SetParent(grid.transform, false);
                            SmartTileRoom room = roomGameObject.AddComponent<SmartTileRoom>();
                            room.m_Bounds = selection;

                            Selection.activeGameObject = target;
                            GridSelection.Select(target, selection);

                            break;
                        }
                    }
                    // ...
                }
            }
            
            Tilemap tilemap = target.GetComponent<Tilemap>();
            if (tilemap != null) {
                if (GUILayout.Button("Add to group")) {
                    GenericMenu groupMenu = new GenericMenu();

                    SmartTileGroupCollection smartTileGroupCollection = smartTileGridBrush.m_tileGroupCollection;
                    if (smartTileGroupCollection != null) {
                        foreach (SmartTileGroup group in SmartTileGroupUtility.GetCollectionGroups(smartTileGridBrush.m_tileGroupCollection)) {
                            groupMenu.AddItem(new GUIContent($"Add to {group.name}"), false, OnAddToGroup, group);
                        }
                        groupMenu.AddSeparator("");
                    }

                    groupMenu.AddItem(new GUIContent("New Group"), false, OnCreateNewGroup);

                    groupMenu.ShowAsContext();
                }
            }
        }

        void OnCreateNewGroup() {
            SmartTileGroupCollection tileGroupCollection = smartTileGridBrush.m_tileGroupCollection;

            if (tileGroupCollection == null) {
                string collectionPath = EditorUtility.SaveFilePanelInProject("Create Group Collection", "SmartTileGroupCollection", "asset", "Create a new collection asset for smart brush");

                tileGroupCollection = ScriptableObject.CreateInstance<SmartTileGroupCollection>();
//                tileGroupCollection.name = 

                AssetDatabase.CreateAsset(tileGroupCollection, collectionPath);

                smartTileGridBrush.m_tileGroupCollection = tileGroupCollection;
            }

            SmartTileGroup tileGroup = SmartTileGroupUtility.AddGroupToCollection(tileGroupCollection, "TileGroup");
            AddSelectedTilesToGroup(tileGroup);
        }

        void OnAddToGroup(object tileGroup) {
            AddSelectedTilesToGroup((SmartTileGroup)tileGroup);
        }

        void AddSelectedTilesToGroup(SmartTileGroup tileGroup) {
            BoundsInt selection = GridSelection.position;
            Tilemap tilemap = GridSelection.target.GetComponent<Tilemap>();//   .target;

            List<TileBase> tiles = tileGroup.m_tiles != null ? new List<TileBase>(tileGroup.m_tiles) : new List<TileBase>();

            foreach (Vector3Int position in selection.allPositionsWithin) {
                TileBase tile = tilemap.GetTile(position);
                if (tile != null && tiles.Contains(tile) == false) {
                    tiles.Add(tile);
                }
            }

            tileGroup.m_tiles = tiles.ToArray();
        }

        public override void PaintPreview(GridLayout gridLayout, GameObject brushTarget, Vector3Int position) {
            base.PaintPreview(gridLayout, GetPreviewBrushTarget(gridLayout), position);
        }

        public override void BoxFillPreview(GridLayout gridLayout, GameObject brushTarget, BoundsInt position) {
            base.BoxFillPreview(gridLayout, GetPreviewBrushTarget(gridLayout), position);
        }

        public override void FloodFillPreview(GridLayout gridLayout, GameObject brushTarget, Vector3Int position) {
            base.FloodFillPreview(gridLayout, GetPreviewBrushTarget(gridLayout), position);
        }

        protected GameObject GetPreviewBrushTarget(GridLayout gridLayout) {
            Transform previewTileTarget = gridLayout.transform.Find("PREVIEW_ONLY_DO_NOT_REMOVE");
            if (previewTileTarget == false) {
                GameObject previewGameObject = new GameObject("PREVIEW_ONLY_DO_NOT_REMOVE"); // fixme
                previewGameObject.transform.SetParent(gridLayout.transform, false);
                previewGameObject.hideFlags = HideFlags.HideAndDontSave;
                previewGameObject.AddComponent<Tilemap>();
                TilemapRenderer tr = previewGameObject.AddComponent<TilemapRenderer>();
                tr.sortingOrder = 100;
                previewTileTarget = previewGameObject.transform;
            }
            return previewTileTarget.gameObject;
        }
    }
}
