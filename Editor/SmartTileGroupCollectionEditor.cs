using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace SmartTileBrush {
    [CustomEditor(typeof(SmartTileGroupCollection))]
    public class SmartTileGroupCollectionEditor : Editor {
        SmartTileGroupCollection groupCollectionTarget => (SmartTileGroupCollection)target;

        public override void OnInspectorGUI() {
            foreach (SmartTileGroup group in SmartTileGroupUtility.GetCollectionGroups(groupCollectionTarget)) {
                if (EditorGUILayout.BeginFoldoutHeaderGroup(true, group.name)) {
                    group.name = EditorGUILayout.TextField(new GUIContent("Name"), group.name);

                    if (group.m_tiles != null) {
                        EditorGUILayout.BeginHorizontal();

                        foreach (TileBase tileBase in group.m_tiles) {
                            Tile tile = tileBase as Tile;
                            if (tile != null) {
                                Rect spriteRect = tile.sprite.rect;
                                Rect guiRect = GUILayoutUtility.GetRect(spriteRect.width, spriteRect.height);

                                //if (guiRect.xMax > EditorGUIUtility.currentViewWidth) {
                                //    GUILayout.FlexibleSpace();
                                //    EditorGUILayout.EndHorizontal();
                                //    EditorGUILayout.BeginHorizontal();
                                //    guiRect = GUILayoutUtility.GetRect(spriteRect.width, spriteRect.height);
                                //}

                                float textureWidth = tile.sprite.texture.width;
                                float textureHeight = tile.sprite.texture.height;
                                Rect textureRect = tile.sprite.rect;
                                Rect normalizedTextureRect = new Rect(textureRect.xMin / textureWidth, textureRect.yMin / textureHeight, textureRect.width / textureWidth, textureRect.height / textureHeight);
                                GUI.DrawTextureWithTexCoords(guiRect, tile.sprite.texture, normalizedTextureRect, true);
                            }
                        }

                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    if (GUILayout.Button("Delete Group")) {
                        SmartTileGroupUtility.RemoveGroupFromCollection(groupCollectionTarget, group);
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Save")) {
                AssetDatabase.SaveAssetIfDirty(target);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
            }

            if (GUILayout.Button("Add Group")) {
                SmartTileGroupUtility.AddGroupToCollection(groupCollectionTarget, "TileGroup");
            }
        }
    }
}
