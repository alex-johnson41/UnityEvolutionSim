using UnityEngine;

[RequireComponent(typeof(CustomGrid))] // Ensure the CustomGrid component is present
public class GridDebugDrawer : MonoBehaviour
{
    private CustomGrid customGrid;

    private void OnDrawGizmos()
    {
        DrawGrid();
    }

    private void OnDrawGizmosSelected()
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        customGrid = GetComponent<CustomGrid>();
        if (customGrid == null)
        {
            Debug.LogWarning("CustomGrid component not found.");
            return;
        }

        int gridSizeX = customGrid.xSize;
        int gridSizeY = customGrid.ySize;
        float cellSizeX = customGrid.cellSize.x;
        float cellSizeY = customGrid.cellSize.y;

        Gizmos.color = Color.white;

        float halfGridSizeX = gridSizeX * 0.5f * cellSizeX;
        float halfGridSizeY = gridSizeY * 0.5f * cellSizeY;

        // Get the position of the grandparent of the CustomGrid.
        Transform grandparentTransform = customGrid.transform.parent;
        Vector3 grandparentPosition = grandparentTransform.position;

        // Adjust the originPosition to be relative to the grandparent.
        Vector3 adjustedOriginPosition = customGrid.originPosition - grandparentPosition;

        for (int x = 0; x <= gridSizeX; x++)
        {
            float xPos = x * cellSizeX - halfGridSizeX - adjustedOriginPosition.x;
            Gizmos.DrawLine(new Vector3(xPos, -halfGridSizeY - adjustedOriginPosition.y, 0), new Vector3(xPos, halfGridSizeY - adjustedOriginPosition.y, 0));
        }

        for (int y = 0; y <= gridSizeY; y++)
        {
            float yPos = y * cellSizeY - halfGridSizeY - adjustedOriginPosition.y;
            Gizmos.DrawLine(new Vector3(-halfGridSizeX - adjustedOriginPosition.x, yPos, 0), new Vector3(halfGridSizeX - adjustedOriginPosition.x, yPos, 0));
        }
    }
}
