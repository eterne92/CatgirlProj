using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager
{
    public class CharacterInfo
    {
        public string name;
        public string room;
        public bool infected;
        public bool alive;
    }

    int totalDays;
    int totalCharacters;
    int currentDay;
    public bool someoneDiedYesterday { get; set; }

    List<CharacterInfo> characterList;

    public void Init()
    {
        characterList = new List<CharacterInfo>();
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
        for (int i = 0; i < characterList.Count; i++)
        {
            if (characterList[i].alive && characterList[i].infected && Random.Range(0, 100) < 10)
            {
                characterList[i].alive = false;
                someoneDiedYesterday = true;
            }
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
        for (int i = 0; i < characterList.Count; i++)
        {
            if (characterList[i].alive)
            {
                count++;
            }
        }
        return count;
    }

    // how many characters are left
    public int UnusedCharacters()
    {
        return totalCharacters - characterList.Count;
    }

    public void AddCharacter(int id)
    {
        characterList.Add(GameManager.Instance.CharacterManager.GetCharacterInfo(id));
    }
}
