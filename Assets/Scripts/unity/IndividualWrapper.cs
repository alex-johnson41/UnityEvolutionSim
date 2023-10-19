using UnityEngine;


class IndividualWrapper : MonoBehaviour{

    public Grid grid;
    public Individual indiv;
    public int gridX;
    public int gridY;

    public void Start(){
        gridX = indiv.x;
        gridY = indiv.y;
        Transform transfrom = this.GetComponent<Transform>();
        transform.position = grid.CellToLocal(new Vector3Int(gridX, gridY, 0));
        transfrom.localScale = Vector3.Scale(grid.cellSize, grid.cellSize);
    }

    public void Update(){
        gridX = indiv.x;
        gridY = indiv.y;
        transform.position = grid.CellToLocal(new Vector3Int(gridX, gridY, 0));
    }
}