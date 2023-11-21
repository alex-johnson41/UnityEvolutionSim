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
        transform.position = grid.gridToWorldPosition(new Vector3(gridX, gridY, 0));
        transform.localScale = new Vector3(1.0f / grid.xSize, 1.0f/grid.ySize);
        changeColor();
    }

    public void Update(){
        gridX = indiv.x;
        gridY = indiv.y;
        transform.position = grid.gridToWorldPosition(new Vector3(gridX, gridY, 0));
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