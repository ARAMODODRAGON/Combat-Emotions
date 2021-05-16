using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public void SwitchScene(int sceneIndex_)
    {
        SceneManager.LoadSceneAsync(sceneIndex_, LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
