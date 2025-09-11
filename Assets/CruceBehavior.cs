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
    public SemaforoController semaforoController; // Asignar en el inspector o en Start()

    public List<GameObject> carPrefabs;
    public List<PathConfig> pathConfigs;

    private Dictionary<int, string> carTypes = new Dictionary<int, string>() {
        {1, "chuygarcia"}, {2, "chuyelizondo"}, {3, "chuycovarrubias"},
        {4, "fercantu"}, {5, "fercovarrubias"}, {6, "ferelizondo"},
        {7, "luisgarcia"}, {8, "luiscantu"}, {9, "luiscovarrubias"}, {10, "luiskiel"},
        {11, "richelizondo"}, {12, "richiegarcia"}, {13, "richcantu"}
    };

    private Dictionary<int, int> carStoplights = new Dictionary<int, int>() {
        {1, 1}, {2, 1}, {3, 1},
        {4, 2}, {5, 2}, {6, 2},
        {7, 3}, {8, 3}, {9, 3}, {10, 3},
        {11, 4}, {12, 4}, {13, 4}
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
        if (semaforoController == null)
        {
            semaforoController = FindFirstObjectByType<SemaforoController>();
        }
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
                    if (semaforoController != null)
                        semaforoController.CambiarAVerde(ev.semaforo);
                    break;
                case CruceEventType.EarlySwitch:
                    Debug.Log($"[EVENT] Early switch a semáforo: {ev.semaforo}");
                    if (semaforoController != null)
                        semaforoController.CambiarAVerde(ev.semaforo);
                    // Si quieres hacer algo especial para early switch, agrégalo aquí
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
        int carSL = carStoplights[pathId];
        GameObject prefab = carPrefabs.Find(p => p != null && p.name == carType);

        if (prefab == null)
        {
            Debug.LogWarning($"No se encontró prefab con nombre {carType}");
            return;
        }

        GameObject clon = Instantiate(prefab, spawnPosition, Quaternion.Euler(0, yRotation, 0));
        clon.transform.localScale = new Vector3(300, 300, 300);
        clon.tag = "Car";

        // si el clon o sus hijos ya tienen colliders, no agregar otro (evita duplicados)
        Collider[] existing = clon.GetComponentsInChildren<Collider>();
        if (existing == null || existing.Length == 0)
        {
            // agregar BoxCollider al root y ajustarlo automáticamente al tamaño del modelo
            BoxCollider bc = clon.GetComponent<BoxCollider>();
            if (bc == null) bc = clon.AddComponent<BoxCollider>();

            // calcular bounds combinados de todos los renderers (world space)
            Renderer[] rends = clon.GetComponentsInChildren<Renderer>();
            if (rends != null && rends.Length > 0)
            {
                Bounds combined = rends[0].bounds;
                for (int i = 1; i < rends.Length; i++)
                    combined.Encapsulate(rends[i].bounds);

                // convertir tamaño world -> local (tener en cuenta la escala del objeto)
                Vector3 lossy = clon.transform.lossyScale;
                Vector3 localSize = new Vector3(
                    combined.size.x / (Mathf.Approximately(lossy.x, 0f) ? 1f : lossy.x),
                    combined.size.y / (Mathf.Approximately(lossy.y, 0f) ? 1f : lossy.y),
                    combined.size.z / (Mathf.Approximately(lossy.z, 0f) ? 1f : lossy.z)
                );

                bc.size = localSize;
                bc.center = clon.transform.InverseTransformPoint(combined.center);
            }
            else
            {
                // fallback si no hay Renderers (ajusta a lo que necesites)
                bc.size = new Vector3(1f, 1f, 2f);
                bc.center = Vector3.zero;
            }

            bc.isTrigger = false; // normalmente false para raycasts/colisiones físicas
        }

        // (opcional) asegurar un Rigidbody kinemático para que las queries físicas funcionen bien
        Rigidbody rb = clon.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = clon.AddComponent<Rigidbody>();
            rb.isKinematic = true; // movemos con transform, no con física
            rb.useGravity = false;
        }


        // Asegura que el clon tenga CarBehaviour1
        CarBehaviour1 carScript = clon.GetComponent<CarBehaviour1>();
        if (carScript == null)
        {
            carScript = clon.AddComponent<CarBehaviour1>();
        }
        carScript.pathId = pathId;
        carScript.nombre = carType;
        carScript.semaforoId = carSL;

        // Buscar el PathConfig correspondiente
        PathConfig config = pathConfigs.Find(c => c.pathId == pathId);
        if (config != null)
        {
            FollowWaypoints fw = clon.GetComponent<FollowWaypoints>();
            if (fw != null)
            {
                fw.waypoints = config.waypoints;
                fw.speed = 1000;
                fw.detenido = false;
                fw.semaforoId = carSL; // ¡IMPORTANTE!
            }
        }
        else
        {
            Debug.LogWarning($"No se encontró configuración de waypoints para pathId {pathId}");
        }

        //Debug.Log($"Carro {carType} instanciado con pathId={pathId} y semaforo {carSL}");
    }
}