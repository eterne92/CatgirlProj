using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    Canvas[] SceneCanvas;
    [SerializeField]
    string[] SceneNames;

  
    public void SwitchScene(string SceneName)
    {
        Debug.Log("Switching to " + SceneName);
        for (int i = 0; i < SceneCanvas.Length; i++)
        {
            if (SceneNames[i] == SceneName)
            {
                ShowCanvas(i);
            }
        }

        // Special treatment
    }

    void ShowCanvas(int scene)
    {
        foreach (var canvas in SceneCanvas)
        {
            canvas.enabled = false;
        }

        SceneCanvas[scene].enabled = true;
    }
}
