using UnityEngine;

public class SemaforoIndicator : MonoBehaviour
{
    [Header("ID del semáforo a monitorear (1-4)")]
    public int semaforoId = 1;

    [Header("Colores para el objeto")]
    public Color colorVerde = Color.green;
    public Color colorRojo = Color.red;

    private SemaforoController semaforoCtrl;
    private Renderer rend;

    void Start()
    {
        semaforoCtrl = FindFirstObjectByType<SemaforoController>();
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning("Este objeto no tiene un Renderer. El color no podrá cambiarse.");
        }
    }

    void Update()
    {
        if (semaforoCtrl == null || rend == null) return;

        // Cambia el color según el estado del semáforo
        if (semaforoCtrl.PuedeAvanzar(semaforoId))
        {
            rend.material.color = colorVerde;
        }
        else
        {
            rend.material.color = colorRojo;
        }
    }
}