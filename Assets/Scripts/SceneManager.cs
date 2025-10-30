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

    private bool doorOpenedToday = false;

    public bool DoorOpenedToday { get => doorOpenedToday; set => doorOpenedToday = value; }

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
    bool ShouldOpenDoor()
    {
        bool characterUnused = GameManager.Instance.GameDataManager.UnusedCharacters() > 0;
        return characterUnused && !doorOpenedToday;
    }

    bool ShouldVisitGuest(int id)
    {
        var characterInfos = GameManager.Instance.CharacterManager.GetCharacterInfos();
        bool justDied = id == GameManager.Instance.GameDataManager.diedId;
        bool alive = characterInfos[id].alive;
        return characterInfos[id].stay && !characterInfos[id].visitedToday && (alive || justDied);
    }
    public void StartHallwayScene()
    {
        dialogueRunner.Stop();
        var characterInfos = GameManager.Instance.CharacterManager.GetCharacterInfos();
        for (int i = 0; i < characterInfos.Length; i++)
        {
            roomButtonsForHallway[i].DoAlphaTween(ShouldVisitGuest(i) ? 1 : 0, 0);
        }

        roomButtonsForDoorway.DoAlphaTween(ShouldOpenDoor() ? 1 : 0, 0);
    }
    public void GotoGuestRoom(int id)
    {
        var characterInfos = GameManager.Instance.CharacterManager.GetCharacterInfos();
        characterInfos[id].visitedToday = true;
        if (!characterInfos[id].alive)
        {
            dialogueRunner.StartDialogue("Died");
        }
        else if (characterInfos[id].firstGuestChat)
        {
            characterInfos[id].firstGuestChat = false;
            dialogueRunner.StartDialogue("Character_" + id + "_Chat");
        }
        else
        {
            dialogueRunner.StartDialogue("Character_" + id + "_NormalChat");
        }
    }
    public void GotoDoorway()
    {
        var characterInfos = GameManager.Instance.CharacterManager.GetCharacterInfos();
        for (int i = 0; i < characterInfos.Length; i++)
        {
            if (characterInfos[i].used == false)
            {
                doorOpenedToday = true;
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
