using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager
{
    [Serializable]
    public class CharacterInfo
    {
        public string name;
        public string room;
        public bool infected;
        public bool alive = true;
        public bool stay = false;
        public bool used = false;
    }

    int totalDays;
    int totalCharacters;
    int currentDay;
    public bool someoneDiedYesterday { get; set; }


    public void Init()
    {
        totalDays = 3;
        totalCharacters = totalDays;    // as for now, we have the same amount of characterss as days
        currentDay = 0;
        someoneDiedYesterday = false;
    }

    public void DayEnd()
    {
        currentDay++;
        // check if someone is dead
        someoneDiedYesterday = false;
        CharacterInfo[] characterList = GameManager.Instance.CharacterManager.GetCharacterInfos();
        for (int i = 0; i < characterList.Length; i++)
        {
            if (characterList[i].alive && characterList[i].stay && characterList[i].infected && UnityEngine.Random.Range(0, 100) < 10)
            {
                characterList[i].alive = false;
                someoneDiedYesterday = true;
            }
        }

        if(currentDay >= totalDays)
        {
            GameManager.Instance.SceneManager.GameEnd();
        }
    }

    public int DaysRemain()
    {
        return totalDays - currentDay;
    }

    // how many characters are alive
    public int CharacterAlive()
    {
        int count = 0;
        CharacterInfo[] characterList = GameManager.Instance.CharacterManager.GetCharacterInfos();
        for (int i = 0; i < characterList.Length; i++)
        {
            if (characterList[i].alive && characterList[i].stay)
            {
                count++;
            }
        }
        return count;
    }

    // how many characters are left
    public int UnusedCharacters()
    {
        int count = 0;
        CharacterInfo[] characterList = GameManager.Instance.CharacterManager.GetCharacterInfos();
        for (int i = 0; i < characterList.Length; i++)
        {
            if (characterList[i].used == false)
            {
                count++;
            }
        }
        return count;
    }

    public void AddCharacter(int id)
    {
        GameManager.Instance.CharacterManager.GetCharacterInfo(id).stay = true;
    }
    public void SendawayCharacter(int id)
    {
        GameManager.Instance.CharacterManager.GetCharacterInfo(id).stay = false;
    }
}
