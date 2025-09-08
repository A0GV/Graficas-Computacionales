using UnityEngine;

public class FollowWaypoints : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 3f;
    private int currentWaypoint = 0;

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
            {
                // Opcional: repetir desde el inicio o detenerse
                //currentWaypoint = 0; // Para hacer un loop
                enabled = false;   // Para detenerse
            }
        }
    }
}