using UnityEngine;
using System.Collections;
using Pathfinding;

public class PetAI : MonoBehaviour {
    public Transform target;

    private Seeker seeker;
    private CharacterController controller;

    public Path path;
    public float speed = 5;
    // The max distance from the AI to a waypoint for it to continue.
    public float nextWaypointDistance = 3;

    private int currentWaypoint = 0;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
        Debug.Log("controller enabled: " + controller.enabled);

        seeker.pathCallback += OnPathComplete;
        seeker.StartPath(transform.position, target.position);
    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    public void Update()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            Debug.Log("End of path reached.");
            return;
        }

        // Direction for the next waypoint.
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed * Time.deltaTime;

        controller.SimpleMove(dir);

        // Check if we are close enough to advance to the next waypoint.
        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) <
            nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }

    public void OnDisable()
    {
        seeker.pathCallback -= OnPathComplete;
    }
}
