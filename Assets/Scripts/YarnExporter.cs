using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class YarnExporter
{
    [YarnCommand("Echo")]
    public static void YarnEcho(string s)
    {
        Debug.Log(s);
    }

    [YarnCommand("GameStart")]
    public static void GameStart()
    {
        GameManager.Instance.SceneManager.SwitchScene("MainScene");
        GameManager.Instance.GameDataManager.Init();
        GameManager.Instance.CharacterManager.Init();
    }

    [YarnCommand("DayEnd")]
    public static void DayEnd()
    {
        GameManager.Instance.GameDataManager.DayEnd();
    }


    [YarnCommand("SwitchScene")]
    public static void SwitchScene(string SceneName)
    {
        GameManager.Instance.SceneManager.SwitchScene(SceneName);
    }

    [YarnCommand("LoadCharacter")]
    public static void LoadCharacter(int id)
    {
        GameManager.Instance.CharacterManager.LoadCharacter(id);
    }

    [YarnCommand("AddCharacter")]
    public static void AddCharacter(int id)
    {
        GameManager.Instance.GameDataManager.AddCharacter(id);
    }

    [YarnCommand("SendawayCharacter")]
    public static void SendawayCharacter(int id)
    {
        GameManager.Instance.GameDataManager.SendawayCharacter(id);
    }



    [YarnFunction("SomeoneDiedYesterday")]
    public static bool SomeoneDied()
    {
        return GameManager.Instance.GameDataManager.someoneDiedYesterday;
    }

    [YarnFunction("UnusedCharacterCount")]
    public static int UnusedCharacterCount()
    {
        return GameManager.Instance.GameDataManager.UnusedCharacters();
    }

    [YarnFunction("DaysRemain")]
    public static int DaysRemain()
    {
        return GameManager.Instance.GameDataManager.DaysRemain();
    }

    [YarnFunction("GetCharacterName")]
    public static string GetCharacterName(int id)
    {
        return GameManager.Instance.CharacterManager.GetCharacterName(id);
    }


}
