using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Pathfinding 
{
    Grid grid;
    List<PathNode> openList;
    HashSet<PathNode> closedList;

    const int STRAIGHT_MOVEMENT_COST = 10;
    const int DIAGONAL_MOVEMENT_COST = 14;

    Vector3 start;
    Vector3 target;
    GameObject obj; // represents the object that is initializing this class

    public Pathfinding(Grid grid, Vector3 start, Vector3 target, GameObject obj)
    {
        this.grid = grid;
        this.start = start;
        this.target = target;
        this.obj = obj;
    }

    // Algorithm for A* search
    public Queue<PathNode> FindPath()
    {
        PathNode startNode = grid.PathNodeFromWorldPoint(start);    // get the startnode in grid
        PathNode targetNode = grid.PathNodeFromWorldPoint(target);  // get the target node in grid

        if (!targetNode.walkable) // check if the targetnode is walkable (i.e. no obstacles is directly on the goal)
        {
            return new Queue<PathNode>();
        }

        openList = new List<PathNode> { startNode }; // initialize openlist
        closedList = new HashSet<PathNode>();   // initialize closedlist

        // loop while the number of nodes in open list is non=empty
        while(openList.Count > 0)
        {
            PathNode currentNode = openList[0]; // extract the first node in openlist

            for(int i = 1; i < openList.Count; i++) // loop throgh rest of openlist
            {
                // determine which fCost is minimal or if the fCost and hCost is the same between the currentnode and the node in openlist[i]
                if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost && openList[i].hCost == currentNode.hCost)
                {
                    currentNode = openList[i]; // set currentnode as openlist[i]
                }
            }

            openList.Remove(currentNode); // remove currentnode of openlist
            closedList.Add(currentNode); // add current node to closedlist

            if(currentNode == targetNode) // check if we've reached target node
            {
                return RetracePath(startNode, targetNode); // return the retraced path from targetnode to startingnode
            }


            // loop through every neighbour node of the currentnode
            foreach(PathNode neighbourNode in grid.GetNeighboursList(currentNode))
            {
                // check if the neighbournode is walkable and the obstacle in the node is not the gameobject itself, or if the closedlist contains neighbournode
                if ((!neighbourNode.walkable && !CheckCollider(neighbourNode)) || closedList.Contains(neighbourNode)) continue;

                int costToMove = currentNode.gCost + FindDistanceCost(currentNode, neighbourNode); // calculate the cost to move from the currentnode to neighbournode

                if(costToMove < neighbourNode.gCost || !openList.Contains(neighbourNode)) // check if the costtomove is less than the neighbournode gcost or the openlist doesn;t contain the neighbournode
                {

                    neighbourNode.gCost = costToMove;   // set the neighbournode's gcost to the cost to move from the starting position to the neighbournode
                    neighbourNode.hCost = FindDistanceCost(neighbourNode, targetNode);  // calculate the distance between the neighbournode to the targetnode
                    neighbourNode.prevNode = currentNode; // set the neighbournode's previous node to current node

                    if (!openList.Contains(neighbourNode))  // check if the openlist doesn't contain the neighbournode
                    {
                        openList.Add(neighbourNode);    // add the neighbournode to the openlist
                    }
                }
            }
        }

        return null;
    }

    // retrace a path given the target node and startnode
    Queue<PathNode> RetracePath(PathNode startNode, PathNode targetNode)
    {
        List<PathNode> pathInReverse= new List<PathNode>();
        PathNode currentNode = targetNode; // set current node as target node

        while(currentNode != startNode) // loop while the current node is not equal to the starting node
        {
            if (obj.tag == "Person" && !GameManager.instance.nodesInAPath.Contains(currentNode))    // the if obj is a person and if nodesInAPath list doesn't contain the current node
            {
                // add current node to nodesInAPath
                GameManager.instance.nodesInAPath.Add(currentNode);
            }

            // add current node to path
            pathInReverse.Add(currentNode);

            // set current node to the previous node of the most recent current node.
            currentNode = currentNode.prevNode;
        }

        // reveerse the path
        pathInReverse.Reverse();

        //grid.path = pathInReverse;

        return new Queue<PathNode>(pathInReverse);  // return a queue of pathInReverse
    }

    // calculate the distance cost between nodeA and nodeB given the diagonal and straight movement costs
    int FindDistanceCost(PathNode nodeA, PathNode nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);   // distance between nodeA and nodeB in x 
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);   // distance between nodeA and nodeB in y

        // calculate cost moving diagonally and horizontally/vertically
        return DIAGONAL_MOVEMENT_COST * Mathf.Min(distanceX, distanceY) + STRAIGHT_MOVEMENT_COST * Mathf.Abs(distanceX - distanceY);
    }

    // check if the colliders of a node is not the gameobject obj.
    bool CheckCollider(PathNode node)
    {
        // check if the list of obstacles is 1 and if that obstacle is obj
        if(node.obstacles.Length == 1 && node.obstacles[0].gameObject == obj)
        {
            return true;
        }

        return false;
    }
}
