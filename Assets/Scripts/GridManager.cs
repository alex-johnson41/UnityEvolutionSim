using UnityEngine;


class GridManager : MonoBehaviour{

    [SerializeField] private int width;
    [SerializeField] private int height;
    private new SpriteRenderer renderer;
    private Grid grid;
    private World world;

    public void Start(){
        renderer = GetComponent<SpriteRenderer>();
        grid = GetComponent<Grid>();
        Vector3 size = renderer.bounds.size;
        grid.cellSize = new Vector3(1 / size.x, 1 / size.y);
    }
}