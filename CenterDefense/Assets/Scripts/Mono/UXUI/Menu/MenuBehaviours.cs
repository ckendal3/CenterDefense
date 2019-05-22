using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehaviours : MonoBehaviour
{
    [SerializeField]
    public string mapToLoad = "MainMenu";

    public void LoadMap()
    {
        SceneManager.LoadScene(mapToLoad);
    }
}
