
using System.Collections.Generic;
using UnityEngine;



public class FollowWaypoints : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 3f;
    private int currentWaypoint = 0;

    // Diccionario para asociar nombre de waypoint y ángulo de rotación
    private Dictionary<string, float> waypointRotations = new Dictionary<string, float>();

    void Start()
    {
        // Agrega aquí los nombres y ángulos de rotación deseados
        waypointRotations.Add("luisder", 80f);
        waypointRotations.Add("luisizq", 260f);
        waypointRotations.Add("chuyentra", -20f);
        waypointRotations.Add("luisk", 90f);
        waypointRotations.Add("kiel1", 20f);
        waypointRotations.Add("kiel2", 20f);
        waypointRotations.Add("kiel3", 20f);
        waypointRotations.Add("chuyder", 110f);
        waypointRotations.Add("chuyizq", -80f);
        waypointRotations.Add("chuy5", 20f);
        waypointRotations.Add("ferizq", -45f);
        waypointRotations.Add("ferizq1", -35f);
        waypointRotations.Add("ferder", 40f);
        waypointRotations.Add("ferder1", 50f);
        waypointRotations.Add("fer5", -13f);
        waypointRotations.Add("richizq", -40f);
        waypointRotations.Add("richizq1", -40f);
        waypointRotations.Add("ferentra", 7f);
        waypointRotations.Add("richder", 55f);
        waypointRotations.Add("richder1", 35f);


    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            // Buscar si el nombre del waypoint está en el diccionario
            if (waypointRotations.ContainsKey(target.name))
            {
                transform.Rotate(0, waypointRotations[target.name], 0);
            }
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