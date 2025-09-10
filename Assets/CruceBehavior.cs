using System.Collections.Generic;
using UnityEngine;

public class CruceBehavior : MonoBehaviour
{
    public TCPIPServerAsync server;

    // Lista de prefabs en el mismo orden que los tipos de carro
    public List<GameObject> carPrefabs;

    // Lista configurable desde el Inspector
    public List<PathConfig> pathConfigs;

    // Diccionario de tipos de carro por path_id
    private Dictionary<int, string> carTypes = new Dictionary<int, string>() {
        {1, "chuygarcia"}, {2, "chuyelizondo"}, {3, "chuycovarrubias"},
        {4, "fercantu"}, {5, "fercovarrubias"}, {6, "ferelizondo"},
        {7, "luisgarcia"}, {8, "luiscantu"}, {9, "luiscovarrubias"},
        {10, "luiskiel"}, {11, "richelizondo"}, {12, "richiegarcia"}, {13, "richcantu"}
    };

    void Start()
    {
        if (server == null)
        {
            server = FindFirstObjectByType<TCPIPServerAsync>();
        }
        // // Prueba manual (no sirve)
        SpawnCarFromPathId(1, new Vector3(-20, 0, -0.42f));
    }

    // Llama a este método para crear un carro según el path_id y la posición
    public void SpawnCarFromPathId(int pathId, Vector3 position)
    {
        Debug.Log($"[DEBUG] Vector recibido para spawn: {position}");
        Vector3 debugPosition = new Vector3(0, 0, 0); // Fuerza la posición al centro para depuración
        if (!carTypes.ContainsKey(pathId))
        {
            Debug.LogWarning($"PathId {pathId} no tiene tipo de carro asociado");
            return;
        }
        string carType = carTypes[pathId];
        GameObject prefab = carPrefabs.Find(p => p != null && p.name == carType);

        if (prefab == null)
        {
            Debug.LogWarning($"No se encontró prefab con nombre {carType}");
            return;
        }

        GameObject clon = Instantiate(prefab, position, Quaternion.identity);

        // CarBehavior opcional (para datos extra)
        CarBehaviour1 carScript = clon.GetComponent<CarBehaviour1>();
        if (carScript != null)
        {
            carScript.pathId = pathId;
            carScript.nombre = carType;
        }

        // Buscar el PathConfig correspondiente
        PathConfig config = pathConfigs.Find(c => c.pathId == pathId);
        if (config != null)
        {
            FollowWaypoints fw = clon.GetComponent<FollowWaypoints>();
            if (fw != null)
            {
                fw.waypoints = config.waypoints;   // asigna directamente desde el inspector
                fw.speed = 1000;   // ejemplo dinámico
            }
        }
        else
        {
            Debug.LogWarning($"No se encontró configuración de waypoints para pathId {pathId}");
        }

        Debug.Log($"Carro {carType} instanciado con pathId={pathId}");
    }

}
