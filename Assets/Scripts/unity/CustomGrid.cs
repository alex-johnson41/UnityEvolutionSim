using UnityEngine;


public class CustomGrid : MonoBehaviour
{
    public Vector3 cellSize {get; private set;}
    public Vector3 originPosition {get; private set;}
    public int xSize {get; private set;}
    public int ySize {get; private set;}

    private void Update(){
        originPosition = GetComponentInParent<Transform>().position;
        transform.position = originPosition;
        Transform parentTranform = GetComponentInParent<Transform>().parent;
        float xScale = parentTranform.localScale.x;
        float yScale = parentTranform.localScale.y;
        cellSize = new Vector3((float) xScale/xSize, (float) yScale/ySize);
    }

    public Vector3 gridToWorldPosition(Vector3 position){
        return originPosition + Vector3.Scale(position, cellSize);
    }

    public void initializeGrid(int xSize, int ySize){
        this.xSize = xSize;
        this.ySize = ySize;
        Transform parentTranform = GetComponentInParent<Transform>().parent;
        float xScale = parentTranform.localScale.x;
        float yScale = parentTranform.localScale.y;
        cellSize = new Vector3((float) xScale/xSize, (float) yScale/ySize);
    }
}