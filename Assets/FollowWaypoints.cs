using System.Collections.Generic;
using UnityEngine;

public class FollowWaypoints : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 3f;
    private int currentWaypoint = 0;

    // Nueva bandera pública para detener el carro
    public bool detenido = false;

    // Semáforo asignado a este carro
    public int semaforoId;

    // Referencia al controlador de semáforos
    private SemaforoController semaforoCtrl;

    public float distanciaSegura = 3f;

    // Diccionario para asociar nombre de waypoint y ángulo de rotación
    private Dictionary<string, float> waypointRotations = new Dictionary<string, float>();

    private Dictionary<int, string> waypointAltoPorSemaforo = new Dictionary<int, string>()
    {
        {1, "chuy4"},
        {2, "fer4"},
        {3, "luis4"},
        {4, "rich4"}
    };

    // Nuevo: referencia al Rigidbody
    private Rigidbody rb;

    void Awake()
    {
        // Añade Rigidbody kinemático y BoxCollider si no existen
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true; // importante para moverlo manualmente
        }

        BoxCollider bc = GetComponent<BoxCollider>();
        if (bc == null)
        {
            bc = gameObject.AddComponent<BoxCollider>();
            bc.center = Vector3.up * 0.5f; // ajusta según altura del auto
            bc.size = new Vector3(1f, 1f, 2f); // ajusta según tamaño del auto
        }
    }

    void Start()
    {
        semaforoCtrl = FindFirstObjectByType<SemaforoController>();

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
        
        // --- LÓGICA DE SEMÁFORO ---
        bool puedeAvanzar = semaforoCtrl != null && semaforoCtrl.PuedeAvanzar(semaforoId);
        bool cercaAlto = CercaDeLineaAlto();

        if (semaforoCtrl != null && !puedeAvanzar && cercaAlto)
        {
            detenido = true;
        }
        else
        {
            detenido = false;
        }

        if (waypoints == null || waypoints.Length == 0) return;

        // --- Nuevo: OverlapSphere para detectar autos delante ---
        Vector3 puntoDelante = transform.position + transform.forward * distanciaSegura;
        Collider[] hits = Physics.OverlapSphere(puntoDelante, 1f);
        bool hayCarroAdelante = false;
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Car") && hit.gameObject != gameObject)
            {
                hayCarroAdelante = true;
                break;
            }
        }

        // SOLO DETENIDO por carro si el semáforo está en rojo o si aún no puede avanzar
        if (hayCarroAdelante && !puedeAvanzar)
        {
            detenido = true;
        }

        if (detenido) return;

        // Movimiento usando Rigidbody kinemático
        Transform target = waypoints[currentWaypoint];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Giro al llegar al waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            if (waypointRotations.ContainsKey(target.name))
            {
                transform.Rotate(0, waypointRotations[target.name], 0);
            }
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
            {
                Destroy(gameObject);
            }
        
    }
    }

    bool CercaDeLineaAlto()
    {
        if (waypoints == null || waypoints.Length == 0) return false;
        if (!waypointAltoPorSemaforo.ContainsKey(semaforoId)) return false;
        string nombreAlto = waypointAltoPorSemaforo[semaforoId];

        Transform altoTarget = System.Array.Find(waypoints, w => w.name == nombreAlto);
        if (altoTarget == null) return false;

        float distancia = Vector3.Distance(transform.position, altoTarget.position);
        return distancia < 1.0f;
    }
}
