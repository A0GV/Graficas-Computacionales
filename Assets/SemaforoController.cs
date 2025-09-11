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
        var keys = new List<int>(estados.Keys);
        foreach (var key in keys)
        {
            estados[key] = "rojo";
        }
        estados[semaforoId] = "verde";
        Debug.Log($"[Semaforo] Semáforo {semaforoId} cambiado a VERDE");
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