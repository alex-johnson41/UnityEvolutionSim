using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;


public class World : MonoBehaviour
{
    private int width;
    private int height;
    private Individual[,] indivMap;
    private Dictionary<(int, int), bool> openCells;

    public int Width {get;}
    public int Height {get;}

    public World(int width, int height){
        this.width = width;
        this.height = height;
        indivMap = new Individual[width, height];
        openCells = initializeOpenCells();
    }

    public void addIndividual(Individual indiv, int x, int y){
        indivMap[x,y] = indiv;
        openCells[(x,y)] = false;
    }

    public void removeIndividual((int, int) coordinates){
        indivMap[coordinates.Item1, coordinates.Item2] = null;
        openCells[(coordinates.Item1, coordinates.Item2)] = true;
    }

    public void tryMoveIndividual(Individual indiv, int xOffset, int yOffset, out int? newX, out int? newY){
        findIndividual(indiv, out int x, out int y);
        int endX = x + xOffset;
        int endY = y + yOffset;
        if (openCells.ContainsKey((endX, endY)) & openCells[(endX, endY)]){
            removeIndividual((endX, endY));
            addIndividual(indiv, endX, endY);
            newX = endX;
            newY = endY;
            return;
        }
        newX = null;
        newY = null;
    }

    public void clearMap(){
        for (int i = 0; i < width; i ++){
            for (int j = 0; j < height; j ++){
                indivMap[i,j] = null;
            }
        }
    }

    public bool cellOpen(int x, int y){
        return openCells[(x, y)];
    }

    public void randomOpenCell(out int x, out int y){
        (int, int)[] cells = openCells.Where(item => item.Value == true)
                                      .Select(item => item.Key).ToArray(); 
        System.Random rnd = new();
        int index = rnd.Next(cells.Length);
        (x, y) = cells[index];
    }

    public void findIndividual(Individual indiv, out int x, out int y){
        (double oldX, double oldY) = indiv.getLocation();
        x = (int) oldX * width;
        y = (int) oldY * height;
    }
    
    private Dictionary<(int, int), bool> initializeOpenCells(){
        Dictionary<(int, int), bool> openCellsDict = new Dictionary<(int, int), bool>{};
        for (int i = 0; i < width; i ++){
            for (int j = 0; j < height; j++){
                openCellsDict.Add((i, j), true);
            }
        }
        return openCellsDict;
    }
}
