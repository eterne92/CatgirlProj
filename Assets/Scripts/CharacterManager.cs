using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    [SerializeField]
    GameDataManager.CharacterInfo[] characters;
    [SerializeField]
    Sprite[] sprites;

    [SerializeField]
    Image avatar;

    int currentCharacter;
    public void Init()
    {
    }

    public void LoadCharacter(int id)
    {
        Debug.Log("Loading Character: " + id);
        currentCharacter = id;
        characters[id].used = true;
        avatar.sprite = sprites[currentCharacter];
    }

    public string GetCurrentCharacterName()
    {
        return GetCharacterInfo(currentCharacter).name;
    }

    public string GetCharacterName(int id)
    {
        return GetCharacterInfo(id).name;
    }

    public GameDataManager.CharacterInfo GetCharacterInfo(int id)
    {
        return characters[id];
    }

    public GameDataManager.CharacterInfo[] GetCharacterInfos()
    {
        return characters;
    }
}
