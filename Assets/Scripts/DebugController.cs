using UnityEngine;
using TMPro;
using System.Collections.Generic;


public class SerialDebugHandler : MonoBehaviour
{
    // Componentes de la gráfica
    public LineRenderer lineRenderer;
    public int maxPoints = 50;             // Máximo de puntos visibles
    public float updateInterval = 0.5f;    // Intervalo de actualización en segundos
    public float xSpacing = 1f;            // Espaciado entre puntos
    public float minY = 20f;               // Mínimo del eje Y
    public float maxY = 37f;               // Máximo del eje Y

    private List<Vector3> points = new List<Vector3>();

    // Componentes para el serial
    public TextMeshProUGUI dataText;       // Mostrar datos recibidos
    private float latestTemperature = 25f; // Última temperatura procesada (valor inicial por defecto)

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

    // Método llamado desde el sistema de serial para recibir datos del sensor
    public void OnMessageArrived(string message)
    {
        Debug.Log("Mensaje recibido del puerto serial: " + message);

        // Mostrar mensaje completo
        if (dataText != null)
        {
            dataText.text = "Datos recibidos:\n" + message;
        }

        // Extraer la temperatura del primer sensor
        float extractedTemperature = ExtractFirstSensorValue(message);
        if (extractedTemperature != -1)
        {
            latestTemperature = Mathf.Clamp(extractedTemperature, minY, maxY); // Limitar al rango válido
        }
    }

    // Método para extraer el valor del primer sensor
    private float ExtractFirstSensorValue(string message)
    {
        try
        {
            int startIndex = message.IndexOf("Sensor 1:") + "Sensor 1:".Length;
            int endIndex = message.IndexOf("??C", startIndex);

            if (startIndex > -1 && endIndex > startIndex)
            {
                string valueString = message.Substring(startIndex, endIndex - startIndex).Trim();
                return float.Parse(valueString);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al extraer el valor del primer sensor: " + e.Message);
        }

        return -1; // Retorna -1 si falla
    }

    // Actualización de la gráfica
    void UpdateGraph()
    {
        // Usar la última temperatura recibida en lugar de un valor aleatorio
        float newY = latestTemperature;

        // Agregar un nuevo punto en el extremo derecho
        points.Add(new Vector3((points.Count > 0 ? points[^1].x + xSpacing : 0), newY, 0));

        // Desplazar todos los puntos hacia la izquierda
        for (int i = 0; i < points.Count; i++)
        {
            points[i] = new Vector3(points[i].x - xSpacing, points[i].y, 0);
        }

        // Eliminar puntos que salen del rango visible
        if (points.Count > 0 && points[0].x < -maxPoints * xSpacing)
        {
            points.RemoveAt(0);
        }

        // Actualizar el LineRenderer
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}