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

    // Diccionario para asociar nombre de waypoint y ángulo de rotación
    private Dictionary<string, float> waypointRotations = new Dictionary<string, float>();

    private Dictionary<int, string> waypointAltoPorSemaforo = new Dictionary<int, string>()
{
    {1, "chuy4"},
    {2, "fer4"},
    {3, "luis4"},
    {4, "rich4"}
};

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
        if (semaforoCtrl != null && !semaforoCtrl.PuedeAvanzar(semaforoId) && CercaDeLineaAlto())
        {
            detenido = true;
        }
        else
        {
            detenido = false;
        }

        if (detenido) return;

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
                // Destruye el objeto al llegar al último waypoint
                Destroy(gameObject);
            }
        }
    }

    // Debes ajustar esta función para tu escena específica:
    // Por ejemplo, si el waypoint 2 es el punto de alto para el semáforo de este path
    bool CercaDeLineaAlto()
{
    if (waypoints == null || waypoints.Length == 0) return false;
    if (!waypointAltoPorSemaforo.ContainsKey(semaforoId)) return false;
    string nombreAlto = waypointAltoPorSemaforo[semaforoId];

    Transform altoTarget = System.Array.Find(waypoints, w => w.name == nombreAlto);
    if (altoTarget == null) return false;

    float distancia = Vector3.Distance(transform.position, altoTarget.position);
    return distancia < 1.0f; // puedes ajustar el rango (ej: 0.5f o 1.5f)
}
}