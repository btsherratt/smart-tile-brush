using UnityEngine;
using UnityEngine.Tilemaps;

namespace SmartTileBrush {
    public class SmartTileGroup : ScriptableObject {
        public TileBase[] m_tiles;
        public int m_layer;
    }
}
