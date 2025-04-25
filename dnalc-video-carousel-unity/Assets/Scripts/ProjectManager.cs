using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.F1)) { SceneManager.LoadScene("MainMenu"); }
        else if(Input.GetKey(KeyCode.F2)) { SceneManager.LoadScene("VideoSlider"); }
        else if (Input.GetKey(KeyCode.F3)) { }
        else if (Input.GetKey(KeyCode.F4)) { }
    }
}
