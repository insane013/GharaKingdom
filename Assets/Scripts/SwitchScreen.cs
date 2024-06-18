using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScreen : MonoBehaviour
{
    [SerializeField] private string sceneName = "GamePlay";
    public void StartGame()
    {
        SceneManager.LoadScene(sceneName);
    }
}
