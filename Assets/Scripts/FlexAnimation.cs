using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class FlexAnimation : MonoBehaviour
{
    public Animator handAnimator;
    private static SerialPort sp = new SerialPort("COM3", 9600);
    private static string incomingMsg = "0.0";
    public float sensitivity = 1.0f;

    private bool isDelaying = false;

    void Start()
    {
        sp.Open();
    }

    private void OnDestroy()
    {
        sp.Close();
    }

    void Update()
    {
        if (sp.IsOpen)
        {
            Debug.Log(incomingMsg);
            incomingMsg = sp.ReadLine();

            float flexValue = float.Parse((incomingMsg.Trim()));
            Debug.Log(flexValue);
            handAnimator.SetFloat("Trigger", flexValue * sensitivity);

            if (!isDelaying)
            {
                // Start a new thread for sleeping
                Thread delayThread = new Thread(() =>
                {
                    isDelaying = true;
                    Thread.Sleep(50); // Sleep for 50 milliseconds
                    isDelaying = false;
                });

                delayThread.Start();
            }
        }
        else
        {
            sp.Open();
        }
    }
}
