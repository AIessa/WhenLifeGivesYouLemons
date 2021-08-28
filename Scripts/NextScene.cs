using UnityEngine;
using UnityEngine.SceneManagement;
 
public class NextScene : MonoBehaviour
{
    [SerializeField] string scene_name;
    public void GoToNextScene()
    {
        SceneManager.LoadScene(scene_name);
    }
}