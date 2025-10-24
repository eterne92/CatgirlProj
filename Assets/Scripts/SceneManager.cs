using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    Canvas[] SceneCanvas;
    [SerializeField]
    string[] SceneNames;
    [SerializeField]
    DialogueRunner dialogueRunner;

    int currentScene = -1;

  
    public void SwitchScene(string SceneName)
    {
        Debug.Log("Switching to " + SceneName);
        for (int i = 0; i < SceneCanvas.Length; i++)
        {
            if (SceneNames[i] == SceneName)
            {
                // do not switch if we are already there
                if (currentScene != i)
                {
                    ShowCanvas(i);
                }
                break;
            }
        }

        // Special treatment
    }

    void ShowCanvas(int scene)
    {
        foreach (var canvas in SceneCanvas)
        {
            canvas.GetComponent<CanvasAlphaController>().Hide(0);
        }

        SceneCanvas[scene].GetComponent<CanvasAlphaController>().Show(0.3f);
        currentScene = scene;
    }


    // Main Scene
    public void MainSeceneGameStart()
    {
        dialogueRunner.StartDialogue("Main");
    }
}
