using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTimeGraph : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int maxPoints = 50; // Máximo de puntos visibles
    public float updateInterval = 0.5f; // Intervalo de actualización en segundos
    public float xSpacing = 1f; // Espaciado entre puntos
    public float yRange = 50f; // Rango del eje Y (0 a 50)

    private List<Vector3> points = new List<Vector3>();

    void Start()
    {   
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.positionCount = 0;

        // Comenzar la actualización de datos
        InvokeRepeating(nameof(UpdateGraph), 0f, updateInterval);
    }

    void UpdateGraph()
    {
        // Generar un nuevo valor aleatorio
        float randomY = Random.Range(25, yRange);

        // Agregar un nuevo punto en el extremo derecho
        points.Add(new Vector3((points.Count > 0 ? points[^1].x + xSpacing : 0), randomY, 0));

        // Desplazar todos los puntos hacia la izquierda
        for (int i = 0; i < points.Count; i++)
        {
            points[i] = new Vector3(points[i].x - xSpacing, points[i].y, 0);
        }

        // Eliminar puntos que salen del rango visible (es decir, fuera del borde izquierdo)
        if (points.Count > 0 && points[0].x < -maxPoints * xSpacing)
        {
            points.RemoveAt(0);
        }

        // Actualizar el LineRenderer
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}