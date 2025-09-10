using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
public enum CruceEventType
{
    CarSpawn,
    SemaforoVerde,
    EarlySwitch
}

public class CruceEvent
{
    public CruceEventType type;
    public int pathId;     // Para CarSpawn
    public int semaforo;   // Para SemaforoVerde y EarlySwitch
}

public class CruceBehavior : MonoBehaviour
{
    public TCPIPServerAsync server;

    public List<GameObject> carPrefabs;

    public List<PathConfig> pathConfigs;

    private Dictionary<int, string> carTypes = new Dictionary<int, string>() {
        {1, "chuygarcia"}, {2, "chuyelizondo"}, {3, "chuycovarrubias"},
        {4, "fercantu"}, {5, "fercovarrubias"}, {6, "ferelizondo"},
        {7, "luisgarcia"}, {8, "luiscantu"}, {9, "luiscovarrubias"},
        {10, "luiskiel"}, {11, "richelizondo"}, {12, "richiegarcia"}, {13, "richcantu"}
    };

    private ConcurrentQueue<CruceEvent> eventos = new ConcurrentQueue<CruceEvent>();

    public void EnqueueEvent(CruceEvent ev)
    {
        eventos.Enqueue(ev);
    }

    void Start()
    {
        if (server == null)
        {
            server = FindFirstObjectByType<TCPIPServerAsync>();
        }
        // Prueba: crear un cubo que siga el path 1 desde el origen
        //SpawnCubeFromPathId(1, new Vector3(0, 0, 0));
        //SpawnCarFromPathId(12, new Vector3(5829f, 0.0001460083f, 1038f));
    }

    void Update()
    {
        CruceEvent ev;
        while (eventos.TryDequeue(out ev))
        {
            switch (ev.type)
            {
                case CruceEventType.CarSpawn:
                    SpawnCarFromPathId(ev.pathId, Vector3.zero);
                    break;
                case CruceEventType.SemaforoVerde:
                    Debug.Log($"[EVENT] Cambiar semáforo a VERDE: {ev.semaforo}");
                    // TODO: Lógica real para cambiar el semáforo en Unity
                    break;
                case CruceEventType.EarlySwitch:
                    Debug.Log($"[EVENT] Early switch a semáforo: {ev.semaforo}");
                    // TODO: Lógica real para early switch
                    break;
            }
        }
    }

    public void SpawnCarFromPathId(int pathId, Vector3 position)
    {

        Vector3 spawnPosition;
        float yRotation;
        if (pathId >= 1 && pathId <= 3)
        {
            spawnPosition = new Vector3(9725f, 0.0001460083f, -3921f);
            yRotation = 0;
        }
        else if (pathId >= 4 && pathId <= 6)
        {
            spawnPosition = new Vector3(13231f, 0.0001460083f, -106f);
            yRotation = -70;
        }
        else if (pathId >= 7 && pathId <= 10)
        {
            spawnPosition = new Vector3(10886f, 0.0001460083f, 5598f);
            yRotation = -153.78f;
        }
        else if (pathId >= 11 && pathId <= 13)
        {
            spawnPosition = new Vector3(5829f, 0.0001460083f, 1038f);
            yRotation = 103.469f;
        }
        else
        {
            spawnPosition = position;
            yRotation = 0f;
        }

        Debug.Log($"[DEBUG] Punto de spawn calculado para pathId {pathId}: {spawnPosition}, rotación Y: {yRotation}");

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


    GameObject clon = Instantiate(prefab, spawnPosition, Quaternion.Euler(0, yRotation, 0));
    clon.transform.localScale = new Vector3(300, 300, 300);

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