using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    // events
    // GameManager is a Singleton that controls the whole game
    private static GameManager m_GameManager;
    // dummy gameobj to hold every sub component
    private static GameObject gameObject;
    public static GameManager Instance
    {
        get
        {
            if (m_GameManager == null)
            {
                m_GameManager = new GameManager();
                gameObject = new GameObject("_gameObj");
                // add to not destroy
                Object.DontDestroyOnLoad(gameObject);
            }
            return m_GameManager;
        }
    }

    // Input Controller
    private static CharacterManager m_CharacterManager;
    public CharacterManager CharacterManager
    {
        get
        {
            if (m_CharacterManager == null)
            {
                Object characterManagerPrefab = Resources.Load("Prefabs/CharacterManagerPrefab");

                GameObject.Instantiate(characterManagerPrefab, gameObject.transform);

                m_CharacterManager = gameObject.GetComponentInChildren<CharacterManager>();
                m_CharacterManager.enabled = true;
            }
            return m_CharacterManager;
        }
    }

    private static SceneManager m_SceneManager;
    public SceneManager SceneManager
    {
        get
        {
            if (m_SceneManager == null)
            {
                Object characterManagerPrefab = Resources.Load("Prefabs/SceneManagerPrefab");

                GameObject.Instantiate(characterManagerPrefab, gameObject.transform);

                m_SceneManager = gameObject.GetComponentInChildren<SceneManager>();
                m_SceneManager.enabled = true;
            }
            return m_SceneManager;
        }
    }

    private static GameDataManager m_GameData;
    public GameDataManager GameDataManager
    {
        get
        {
            if (m_GameData == null)
            {
                m_GameData = new GameDataManager();
            }
            return m_GameData;
        }
    }
}
