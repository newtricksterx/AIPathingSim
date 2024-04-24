using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject person;
    public GameObject chair;
    public GameObject goal;

    public GameObject ground;

    public LayerMask objToDetect;

    public int numberOfPersonsToSpawn;
    public int numberOfChairsToSpawn;

    public float timeToReplaceGoal = 10f;

    public float respawnDelay = 0.5f;

    float max_X;
    float max_Z;
    float min_X;
    float min_Z;

    float currentTime;

    bool replaceGoalStarted; // this is just a flag for if a Coroutine is called


    // Start is called before the first frame update
    void Start()
    {
        currentTime = Time.time; // set current time

        for (int i = 0; i < numberOfChairsToSpawn; i++) // spawn chairs based on the number of chairs to spawn
        {
            SpawnObject(chair); // spawn chair
        }

        for (int i = 0; i < numberOfPersonsToSpawn; i++) // spawn person based on the number of person to spawn
        {
            SpawnObject(person); // spawn person
        }

        SpawnObject(goal); // spawn goals

        replaceGoalStarted = false;
    }

    void Update()
    {
        if (GameManager.instance.isPaused) // check if game is paused.
        {
            GameManager.instance.timeItTakesToReachGoal += (Time.time - currentTime);

            if(!replaceGoalStarted) // after 1/2s pause, the game is unpaused and a new goal is created
            {
                replaceGoalStarted = true;
                ReplaceGoal();
            }

            return;
        }

        if (Time.time >= currentTime + timeToReplaceGoal) // check if the time exceeded time limit for goal
        {
            ReplaceGoal(); // move goal
            currentTime = Time.time;
        }
    }

    GameObject SpawnObject(GameObject obj)
    {
        ExtractBoundaries(); // get boundaries of platform
        
        Vector3 spawnPos = PickRandomVector(obj); // pick a random spawn position

        while (!CheckIfObjIsClear(obj, spawnPos)) // loop until an unobstructed spawn position
        {
            spawnPos = PickRandomVector(obj); 
        }

        GameObject spawnedObj = Instantiate(obj, spawnPos, Quaternion.identity); // instantiate gameobject based on spawn position

        return spawnedObj;
    }

    void ExtractBoundaries() //  extract boundaries of platform
    {
        BoxCollider collider = ground.GetComponent<BoxCollider>(); // get boxcollider of platform
        Bounds bounds = collider.bounds;

        // get the bounds (max, min) of platform
        max_X = bounds.max.x;
        max_Z = bounds.max.z;
        min_X = bounds.min.x;
        min_Z = bounds.min.z;
    }

    // pick random float number
    float PickRandomNumber(float lowerRange, float upperRange)
    {
        return Random.Range(lowerRange, upperRange);
    }

    // get a random vector based on gameobject
    Vector3 PickRandomVector(GameObject obj)
    {
        return new Vector3(PickRandomNumber(min_X, max_X), obj.transform.localScale.y * (obj.GetComponent<MeshFilter>().sharedMesh.bounds.max.y / 2 + 
            ground.GetComponent<MeshFilter>().sharedMesh.bounds.max.y), PickRandomNumber(min_Z, max_Z));
    }

    // check if the spawn position is unobstructed (i.e. can place object without overlapping)
    bool CheckIfObjIsClear(GameObject objToSpawn, Vector3 spawnPoint)
    {

        // check for any object that collide with raycast
        Collider[] objCollider = Physics.OverlapBox(spawnPoint, objToSpawn.GetComponent<MeshFilter>().sharedMesh.bounds.size / 2, Quaternion.identity, objToDetect);

        if(objCollider.Length > 0)
        {
            return false;
        }

        return true;
    }

    // move the goal
    void ReplaceGoal()
    {
        GameObject activeGoal = GameObject.FindGameObjectWithTag("Goal");

        if (activeGoal != null) //  check if a goal exists
        {
            Destroy(activeGoal); // destory active goal
        }
        GameManager.instance.numOfAttemptsReachGoal += 1;
        StartCoroutine(WaitAndReplace(respawnDelay)); // spawn a new goal
        //currentTime = Time.time; // set current time
    }

    IEnumerator WaitAndReplace(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // wait for a few seconds
        SpawnObject(goal);  // spawn a new goal object
        currentTime = Time.time; // set current time
        GameManager.instance.isPaused = false;  // unpause game
        replaceGoalStarted = false;
    }
}
