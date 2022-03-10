using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SmartTileBrush {
    public class SmartTileRoom : MonoBehaviour {
#if UNITY_EDITOR
        static Vector3 ms_labelOffset = new Vector3(10.0f, -10.0f);
#endif

        public BoundsInt m_Bounds;

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            GridLayout gridLayout = GetComponentInParent<GridLayout>();
            Vector3 positionTL = gridLayout.CellToWorld(new Vector3Int(m_Bounds.xMin, m_Bounds.yMin));
            Vector3 positionTR = gridLayout.CellToWorld(new Vector3Int(m_Bounds.xMax, m_Bounds.yMin));
            Vector3 positionBL = gridLayout.CellToWorld(new Vector3Int(m_Bounds.xMin, m_Bounds.yMax));
            Vector3 positionBR = gridLayout.CellToWorld(new Vector3Int(m_Bounds.xMax, m_Bounds.yMax));
            Gizmos.DrawLine(positionTL, positionTR);
            Gizmos.DrawLine(positionTR, positionBR);
            Gizmos.DrawLine(positionBR, positionBL);
            Gizmos.DrawLine(positionBL, positionTL);

            var view = UnityEditor.SceneView.currentDrawingSceneView;
            Vector3 screenPos = view.camera.WorldToScreenPoint(positionBL);
            screenPos = screenPos + ms_labelOffset;
            Vector3 worldPosition = view.camera.ScreenToWorldPoint(screenPos);

            Handles.Label(worldPosition, transform.name);
        }
#endif
    }
}
