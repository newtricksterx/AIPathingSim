using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public bool walkable;
    public Vector3 position;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;

    public PathNode prevNode;
    public Collider[] obstacles;

    public PathNode(bool walkable, Vector3 position, int gridX, int gridY, Collider[] obstacles)
    {
        this.walkable = walkable;
        this.position = position;
        this.gridX = gridX;
        this.gridY = gridY;
        this.obstacles = obstacles;
    }

    // represents the fcost, where if extracted, it will return the sum of gCost and hCost
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

}
