using UnityEngine;

[System.Serializable]
public class PathConfig
{
    public int pathId;              // El ID del camino
    public Transform[] waypoints;   // Los waypoints asociados a ese camino
}
