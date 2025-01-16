using UnityEngine;
using TMPro;

public class SerialDebugHandler : MonoBehaviour
{
    public TextMeshProUGUI dataText;          // TextMeshPro para mostrar los datos recibidos
    public Material targetMaterial;          // Material que cambiará de color
    public float minTemperature = 20f;       // Temperatura mínima (frío, azul)
    public float maxTemperature = 37f;       // Temperatura máxima (calor, rojo)

    // Este método se llama cuando llega un mensaje al puerto serial
    public void OnMessageArrived(string message)
    {
        Debug.Log("Mensaje recibido del puerto serial: " + message);

        // Mostrar el mensaje completo en el TextMeshPro
        if (dataText != null)
        {
            dataText.text = "Datos recibidos:\n" + message;
        }

        // Procesar los datos para extraer la primera temperatura
        float firstSensorValue = ExtractFirstSensorValue(message);
        if (firstSensorValue != -1)
        {
            // Cambiar el color del material según la temperatura
            UpdateMaterialColor(firstSensorValue);
        }
    }

    // Este método se llama cuando cambia el estado de la conexión
    public void OnConnectionEvent(bool isConnected)
    {
        if (isConnected)
        {
            Debug.Log("Conexión establecida con el puerto serial.");
            if (dataText != null)
            {
                dataText.text = "Conexión establecida con el puerto serial.";
            }
        }
        else
        {
            Debug.Log("Conexión perdida con el puerto serial.");
            if (dataText != null)
            {
                dataText.text = "Conexión perdida con el puerto serial.";
            }
        }
    }

    // Método para extraer el valor del primer sensor
    private float ExtractFirstSensorValue(string message)
    {
        try
        {
            // Buscar el inicio del valor del primer sensor
            int startIndex = message.IndexOf("Sensor 1:") + "Sensor 1:".Length;
            int endIndex = message.IndexOf("??C", startIndex);

            if (startIndex > -1 && endIndex > startIndex)
            {
                string valueString = message.Substring(startIndex, endIndex - startIndex).Trim();
                return float.Parse(valueString); // Convertir a float
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al extraer el valor del primer sensor: " + e.Message);
        }

        return -1; // Devolver -1 si no se pudo extraer el valor
    }

    // Método para actualizar el color del material según la temperatura
    private void UpdateMaterialColor(float temperature)
    {
        // Normalizar la temperatura en un rango entre 0 y 1
        float normalizedTemperature = Mathf.InverseLerp(minTemperature, maxTemperature, temperature);

        // Calcular el color interpolando entre azul y rojo
        Color newColor = Color.Lerp(Color.blue, Color.red, normalizedTemperature);

        // Aplicar el color al material
        if (targetMaterial != null)
        {
            targetMaterial.color = newColor;
        }
        else
        {
            Debug.LogWarning("El Material no está asignado en el inspector.");
        }
    }
}