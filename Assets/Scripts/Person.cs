using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MovableObject
{

    // when the person collides with the goal, this function is called.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Goal") // check if object collided with is a goal
        {
            GameManager.instance.isPaused = true;   // pause the game
            GameManager.instance.nodesInAPath.Clear();  // clear the list of cells in a person's path
            GameManager.instance.numOfTimesReachedGoal += 1;
        }
    }

}
