using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BlendControl : MonoBehaviour
{
    private SerialPort serialPort;
    public string portName = "COM3"; // Set your port name
    public int baudRate = 9600;
    private float indexValue = 0f;
    private float ringValue = 0f;

    [SerializeField] GameObject forceBar;
    UnityEngine.UI.Image forceBarImage;
    private Animator animator;

    void Start()
    {
        // Initialize serial port
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
        forceBarImage = forceBar.GetComponent<UnityEngine.UI.Image>();  
        // Get the Animator component attached to the GameObject
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (serialPort.IsOpen)
        {
            try
            {
                // Read data from the serial port
                string data = serialPort.ReadLine();
                string[] values = data.Split(',');

                if (values.Length == 2)
                {
                    // Parse the sensor values
                    float.TryParse(values[0], out ringValue);
                    float.TryParse(values[1], out indexValue);

                    // Clamp values to ensure they are within the expected range
                    ringValue = Mathf.Clamp01(ringValue);
                    indexValue = Mathf.Clamp01(indexValue);
                    //Debug.Log(ringValue + ", "+ middleValue);
                    // Update the blend tree parameters
                    animator.SetFloat("Ring", ringValue);
                    animator.SetFloat("Index", indexValue);
                    forceBarImage.fillAmount = ringValue;



                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error reading from serial port: " + e.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        // Close the serial port when the application quits
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
