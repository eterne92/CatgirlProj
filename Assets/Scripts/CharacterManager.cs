using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public int currentCharacter;
    public string currentCharacterName;

    public void Init()
    {
    }

    public void LoadCharacter(int id)
    {
        Debug.Log("Loading Character: " + id);
        currentCharacter = id;
        currentCharacterName = id.ToString();
    }

    public string GetCurrentCharacterName()
    {
        return currentCharacterName;
    }

    public GameDataManager.CharacterInfo GetCharacterInfo(int id)
    {
        GameDataManager.CharacterInfo info = new GameDataManager.CharacterInfo();
        info.name = currentCharacterName;
        info.room = "Room 1";
        info.infected = false;
        return info;
    }

    public void SetEmotion(string Emotion)
    {
        Debug.Log("Setting Emotion: " + Emotion);
    }

    public void ShowAvatar()
    {
        Debug.Log("Showing Avatar for " + currentCharacterName);
    }
}
