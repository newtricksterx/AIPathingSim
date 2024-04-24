using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    public float velocity;

    protected PathNode goal;

    protected Pathfinding pathfinder;

    public Queue<PathNode> path;

    protected PathNode nextPathNode;

    protected Vector2 movement;

    protected virtual void Start()
    {
        path = new Queue<PathNode>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (GameManager.instance.isPaused) // check if game is paused.
        {
            return;
        }

        // we check if the next path node is not null and if the node that this object is on hasn't reached the next path node.
        if (nextPathNode != null && GameManager.instance.grid.PathNodeFromWorldPoint(transform.position) != nextPathNode) 
        {
            // move towards the nextpathnode in world position
            movement = MoveTowards(new Vector2(transform.position.x, transform.position.z), new Vector2(nextPathNode.position.x, nextPathNode.position.z));

            // set the object's position to thw calculated movement
            transform.position = new Vector3(movement.x, transform.position.y, movement.y);
        }

    }

    protected virtual void LateUpdate()
    {
        goal = SetGoal(); // set the goal of the object

        if (goal != null) // checks if the goal is not null
        {
            // create a new pathfider object to setup the algorithm
            pathfinder = new Pathfinding(GameManager.instance.grid, gameObject.transform.position, goal.position, gameObject);

            // get the Queue of nodes to take based on A* search
            path = pathfinder.FindPath();
        }

        // check if the path count is greater than 0 and if next path node is null or if this object's node position is equal to the next path node.
        if (path != null && path.Count > 0 && (nextPathNode == null || GameManager.instance.grid.PathNodeFromWorldPoint(transform.position) == nextPathNode))
        {
            nextPathNode = path.Dequeue(); // set nextPathNode to be the next node we want the object to move towards in the queue.

            // we want to check if the object is a "person" and if it contains the next pathnode
            if (gameObject.tag == "Person" && GameManager.instance.nodesInAPath.Contains(nextPathNode))
            {
                // remove the nextpatnode from the list of nodes in a path of a person.
                GameManager.instance.nodesInAPath.Remove(nextPathNode);
            }
        }
    }

    // returns a vector2 that represents the next position of the object based on velocity and deltatime
    public Vector2 MoveTowards(Vector3 from, Vector3 to)
    {
        return Vector2.MoveTowards(from, to, velocity * Time.deltaTime);
    }

    // set the goal of the object
    public virtual PathNode SetGoal()
    {
        if(GameObject.FindGameObjectWithTag("Goal") != null) // check if there exists a goal
        {
            // return the patnode in which the goal is on.
            return GameManager.instance.grid.PathNodeFromWorldPoint(GameObject.FindGameObjectWithTag("Goal").transform.position);
        }

        return null;
    }
}
