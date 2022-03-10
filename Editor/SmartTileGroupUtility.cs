using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SmartTileBrush {
    public static class SmartTileGroupUtility {
        public static SmartTileGroup AddGroupToCollection(SmartTileGroupCollection groupCollection, string name) {
            SmartTileGroup group = ScriptableObject.CreateInstance<SmartTileGroup>();
            group.name = name;

            AssetDatabase.AddObjectToAsset(group, groupCollection);
            AssetDatabase.SaveAssetIfDirty(groupCollection);

            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(groupCollection));

            return group;
        }

        public static void RemoveGroupFromCollection(SmartTileGroupCollection groupCollection, SmartTileGroup group) {
            AssetDatabase.RemoveObjectFromAsset(group);
            AssetDatabase.SaveAssetIfDirty(groupCollection);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(group));
        }

        public static IEnumerable<SmartTileGroup> GetCollectionGroups(SmartTileGroupCollection groupCollection) {
            Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(groupCollection));
            foreach (Object asset in assets) {
                SmartTileGroup group = asset as SmartTileGroup;
                if (group != null) {
                    yield return group;
                }
            }
        }
    }
}
