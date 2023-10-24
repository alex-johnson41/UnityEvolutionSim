using System;
using UnityEngine;


class IndividualWrapper : MonoBehaviour{

    public CustomGrid grid;
    public Individual indiv;
    public int gridX;
    public int gridY;

    public void Start(){
        gridX = indiv.x;
        gridY = indiv.y;
        Transform transfrom = GetComponent<Transform>();
        transform.position = grid.gridToWorldPosition(gridX, gridY);
        transfrom.localScale = Vector3.Scale(grid.cellSize, grid.cellSize) * .75f ;
        changeColor();
    }

    public void Update(){
        gridX = indiv.x;
        gridY = indiv.y;
        transform.position = grid.gridToWorldPosition(gridX, gridY);
    }

    private void changeColor(){
        string colorStr = indiv.color;
        byte r = Convert.ToByte(colorStr.Substring(0,2), 16);
        byte g = Convert.ToByte(colorStr.Substring(2,2), 16);
        byte b = Convert.ToByte(colorStr.Substring(4,2), 16);
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color32(r,g,b,255);
    }
}