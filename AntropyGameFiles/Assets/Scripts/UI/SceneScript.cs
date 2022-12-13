using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class SceneScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGame(){
        // generate map if no map was loaded
        SceneManager.LoadScene("Prototype_v1");
    }

    public void QuitGame(){
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit(); // original code to quit Unity player
        #endif 
    }

    public void OpenAntHill(){
        SceneManager.LoadScene("AntHill");
    }
    public void OpenMap(){
        SceneManager.LoadScene("Prototype_v1");
    }

    public void OpenMenu(){
        SceneManager.LoadScene("Start");
    }

    public void OpenSettings(){
        SceneManager.LoadScene("Settings");
    }

    public void SaveGame(){
        SceneManager.LoadScene("Menu");
    }

    public void LoadGame(){
        SceneManager.LoadScene("Menu");
    }

}
