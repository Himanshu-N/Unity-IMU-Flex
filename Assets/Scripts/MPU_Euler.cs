using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class MPU_Euler : MonoBehaviour
{

    SerialPort stream = new SerialPort("COM3", 9600);
    public string strReceived;

    public string[] strData = new string[3];
    public string[] strData_received = new string[3];
    public float qx, qy, qz;
    void Start()
    {
        stream.Open(); //Open the Serial Stream.
    }

    // Update is called once per frame
    void Update()
    {
        strReceived = stream.ReadLine(); //Read the information  

        strData = strReceived.Split(',');
        if (strData[0] != "" && strData[1] != "" && strData[2] != "") //make sure data are readable
        {
            strData_received[0] = strData[0];
            strData_received[1] = strData[1];
            strData_received[2] = strData[2];

            qx = float.Parse(strData_received[1]);
            qy = float.Parse(strData_received[2]);
            qz = float.Parse(strData_received[0]);

            transform.localEulerAngles = new Vector3 (qx, -qy, -qz);

        }
    }

    private void OnDestroy()
    {
        stream.Close();
    }
}