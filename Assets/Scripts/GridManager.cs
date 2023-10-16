using UnityEngine;


class GridManager : MonoBehaviour{

    private new SpriteRenderer renderer;
    public Grid grid;

    public void Start(){
        renderer = GetComponent<SpriteRenderer>();
        grid = GetComponentInChildren<Grid>();
        Vector3 size = renderer.bounds.size;
        grid.cellSize = new Vector3(1 / size.x, 1 / size.y);
        grid.transform.position = this.transform.position;
    }
}