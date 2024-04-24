using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chair : MovableObject
{

    protected override void Start()
    {
        base.Start();
        velocity *= Random.Range(0.25f, 0.5f); // set velocity to between 1/4 to 1/2 of it.
    }

    // functon that calculates that cell that is in a path of a person closest to the chair.
    public PathNode ComputeNewGoal()
    {
        PathNode goal = null;

        PathNode playerGoal = null;

        if(GameObject.FindGameObjectWithTag("Goal") != null)
        {
            playerGoal = GameManager.instance.grid.PathNodeFromWorldPoint(GameObject.FindGameObjectWithTag("Goal").transform.position);
        }

        float minDistance = float.MaxValue;

        foreach(PathNode node in GameManager.instance.nodesInAPath) // loop through every cell that is part of a path in a person
        {
            if(node.obstacles.Length > 0) //if the number of obsctales on that cell is greater than 0, we skip it
            {
                continue;
            }

            if(playerGoal != null && node == playerGoal)    // check if the node is equal to the goal node of players
            {
                continue;
            }

            float distance = CalculateDistance(transform.position, node.position); // calc distance between cell in world position and this gameobject's position
            if (distance < minDistance) // compare the current min distance with the calculated distance
            {


                minDistance = distance;
                goal = node;
            }
        }

        return goal;
    }

    public float CalculateDistance(Vector3 posA, Vector3 posB) // caluclates disatnce between two vectors.
    {
        return Mathf.Sqrt(Mathf.Pow(posA.x - posB.x, 2) + Mathf.Pow(posA.z - posB.z, 2));
    }
    public override PathNode SetGoal() //  sets the goal of the chair
    {
        return ComputeNewGoal();
    }
}
