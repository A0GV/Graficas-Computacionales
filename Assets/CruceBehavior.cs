using System.Collections.Generic;
using UnityEngine;

public class CruceBehavior : MonoBehaviour
{
    public TCPIPServerAsync server;

    // Lista de prefabs en el mismo orden que los tipos de carro
    public List<GameObject> carPrefabs; 

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
        // SpawnCarFromPathId(1, new Vector3(0, 0, 0));
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
        if (carPrefabs == null || carPrefabs.Count == 0)
        {
            Debug.LogWarning($"Lista de prefabs vacía o no asignada");
            return;
        }
        GameObject prefab = carPrefabs.Find(p => p != null && p.name == carType);
        if (prefab == null)
        {
            Debug.LogWarning($"No se encontró prefab con nombre {carType}");
            return;
        }
        Instantiate(prefab, debugPosition, Quaternion.identity);
        Debug.Log($"Carro {carType} instanciado en {debugPosition} (vector original: {position})");
    }
}
