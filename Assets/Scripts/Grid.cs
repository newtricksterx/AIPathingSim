using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public LayerMask goalMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public float radiusOffset = 0.1f;
    PathNode[,] grid;

    float nodeDiameter;
    int gridSizeX;
    int gridSizeY;

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new PathNode[gridSizeX, gridSizeY]; // create a node encapsulating the entire space of the grid
        Vector3 bottomLeftWorld = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2; // get bottom left position

        // create new pathnodes within grid
        for(int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = bottomLeftWorld + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius); // get the world position of new node
                Collider[] obstaclesInCell = Physics.OverlapSphere(worldPoint, nodeRadius + radiusOffset, unwalkableMask); // get list of obstacles in node
                bool walkable = !(obstaclesInCell.Length > 0); // check if node is walkable
                grid[x, y] = new PathNode(walkable, worldPoint, x, y, obstaclesInCell); // set the grid at x, y to a new pathnode
            }
        }
    }

    // returns the list of the neighbours of a node
    public List<PathNode> GetNeighboursList(PathNode node)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        // this for loop will add the next horizontal, vertical, and diagonal neighbour nodes
        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            { 
                if(x == 0 && y == 0) // check if x and y are both equal to 0
                {
                    continue;
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbourList.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbourList;
    }

    // get the path node based on a psoition in the world frame
    public PathNode PathNodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    // return the grid
    public PathNode[,] GetGrid()
    {
        return grid;
    }

    // Get the gridsize of the grid
    public Vector2 GetGridSize()
    {
        return new Vector2(gridSizeX, gridSizeY);
    }

    // update the walkability of every node
    public void UpdateWalkability()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                PathNode node = grid[x, y]; // select the node at x,y
                Collider[] obstaclesInCell = Physics.OverlapSphere(node.position, nodeRadius + radiusOffset, unwalkableMask); // get all of the colliders
                node.walkable = !(obstaclesInCell.Length > 0);  // set walability based on if the number of obstacles in the node recently calculated in greater than 0
                node.obstacles = obstaclesInCell; // set the list of obstacles as the recently calculated list of obstacles
            }
        }
    }

}
