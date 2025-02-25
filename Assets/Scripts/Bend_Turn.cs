using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class Bend_Turn : MonoBehaviour
{
    private SerialPort stream;
    public string portName = "COM3"; // Set your port name
    public int baudRate = 9600;
    //SerialPort stream = new SerialPort("COM3", 19200);

    public string[] strData = new string[7];
    public float thumb,index,middle, qw, qx, qy, qz;

    private Animator animator;

    void Awake()
    {
        //stream.Open(); //Open the Serial Stream.
        // Initialize serial port
        stream = new SerialPort(portName, baudRate);
        stream.Open();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stream.IsOpen)
        {
            try
            {
                strData = stream.ReadLine().Split(',');//Read the information  

                thumb = float.Parse(strData[0]);
                index = float.Parse(strData[1]);
                middle = float.Parse(strData[2]);
                qw = float.Parse(strData[3]);
                qx = float.Parse(strData[4]);
                qy = float.Parse(strData[5]);
                qz = float.Parse(strData[6]);

                animator.SetFloat("thumb", thumb);
                animator.SetFloat("index", index);
                animator.SetFloat("middle", middle);
                transform.rotation = new Quaternion(-qy, -qz, qx, -qw);

            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error reading from serial port: " + e.Message);
            }
        }
    }
    private void OnDestroy()
    {
        stream.Close();
    }
    void OnApplicationQuit()
    {
        // Close the serial port when the application quits
        if (stream != null && stream.IsOpen)
        {
            stream.Close();
        }
    }
}

