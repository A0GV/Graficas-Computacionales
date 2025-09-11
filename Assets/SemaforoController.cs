using System.Collections.Generic;
using UnityEngine;

public class SemaforoController : MonoBehaviour
{
    // 1: verde, 2: rojo, etc. (puedes agregar otros estados)
    private Dictionary<int, string> estados = new Dictionary<int, string>()
    {
        {1, "verde"},
        {2, "rojo"},
        {3, "rojo"},
        {4, "rojo"}
    };

    public void CambiarAVerde(int semaforoId)
    {
        // var keys = new List<int>(estados.Keys);
        // foreach (var key in keys)
        // {
        //     estados[key] = "rojo";
        // }
        // estados[semaforoId] = "verde";
        if (semaforoId == 1)
        {
            estados[1] = "verde";
            estados[2] = "rojo";
            estados[3] = "rojo";
            estados[4] = "rojo";
            Debug.Log($"[Semaforo] Semáforo {semaforoId} ahora en {estados[1]}");
        }
        if (semaforoId == 2)
        {
            estados[1] = "rojo";
            estados[2] = "verde";
            estados[3] = "rojo";
            estados[4] = "rojo";
            Debug.Log($"[Semaforo] Semáforo {semaforoId} ahora en {estados[2]}");
        }
        if (semaforoId == 3)
        {
            estados[1] = "rojo";
            estados[2] = "rojo";
            estados[3] = "verde";
            estados[4] = "rojo";
            Debug.Log($"[Semaforo] Semáforo {semaforoId} ahora en {estados[3]}");
        }
        if (semaforoId == 4)
        {
            estados[1] = "rojo";
            estados[2] = "rojo";
            estados[3] = "rojo";
            estados[4] = "verde";
            Debug.Log($"[Semaforo] Semáforo {semaforoId} ahora en {estados[4]}");
        }
        
    }

    public bool PuedeAvanzar(int semaforoId)
    {
        return estados.ContainsKey(semaforoId) && estados[semaforoId] == "verde";
    }

    public string EstadoSemaforo(int semaforoId)
    {
        return estados.ContainsKey(semaforoId) ? estados[semaforoId] : "desconocido";
    }
}