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

    [SerializeField]
    PressableButton[] roomButtonsForHallway;
    [SerializeField]
    PressableButton roomButtonsForDoorway;

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
        if(SceneName == "Hallway")
        {
            StartHallwayScene();
        }
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

    // Hallway Scene
    public void StartHallwayScene()
    {
        dialogueRunner.Stop();
        var characterInfos = GameManager.Instance.CharacterManager.GetCharacterInfos();
        for (int i = 0; i < characterInfos.Length; i++)
        {
            roomButtonsForHallway[i].DoAlphaTween(characterInfos[i].stay?1:0, 0);
        }

        //roomButtonsForDoorway.gameObject.SetActive(GameManager.Instance.GameDataManager.UnusedCharacters() > 0);
        roomButtonsForDoorway.DoAlphaTween(GameManager.Instance.GameDataManager.UnusedCharacters() > 0 ? 1: 0, 0);
    }
    public void GotoGuestRoom(int id)
    {
        dialogueRunner.StartDialogue("Character_" + id + "_Chat");
    }
    public void GotoDoorway()
    {
        var characterInfos = GameManager.Instance.CharacterManager.GetCharacterInfos();
        for (int i = 0; i < characterInfos.Length; i++)
        {
            if (characterInfos[i].used == false)
            {
                dialogueRunner.StartDialogue("Character_" + i + "_Question");
                break;
            }
        }
    }
    public void GotoSleep()
    {
        dialogueRunner.StartDialogue("Sleep");
    }

    public void GameEnd()
    {
        dialogueRunner.Stop();
        dialogueRunner.StartDialogue("GameEnd");
    }


}
