using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(GUITexture))]
public class ForcedReset : MonoBehaviour {

    void Update () {
        
        // if we have forced a reset ...
        if (CrossPlatformInput.GetButtonDown ("ResetObject")) {
            
            //... reload the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

}
