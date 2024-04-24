using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Grid grid;

    public float pauseSeconds;

    public bool isPaused;

    public List<PathNode> nodesInAPath;

    public int numOfAttemptsReachGoal;
    public int numOfTimesReachedGoal;
    public float timeItTakesToReachGoal;
    public float qty;



    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        grid = gameObject.GetComponent<Grid>();
        isPaused = false;
        nodesInAPath = new List<PathNode>();
        numOfTimesReachedGoal = 0;
        numOfAttemptsReachGoal = 0;
        qty = 0;
    }

    // Update is called once per frame
    void Update()
    {
        grid.UpdateWalkability(); // update the walkability of each cell in grid
        /*
        ++qty;

        Debug.Log("FPS: " + (1.0f / Time.smoothDeltaTime) / qty);
        Debug.Log("Number Of Times Reached Goal: " + numOfTimesReachedGoal);
        Debug.Log("Number Of Times Attempt Goal: " + numOfAttemptsReachGoal);
        if(numOfTimesReachedGoal > 0)
        {
            Debug.Log("Average Time to reach goal: " + ((timeItTakesToReachGoal / numOfTimesReachedGoal) % 60) );
        }
        */
        
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
