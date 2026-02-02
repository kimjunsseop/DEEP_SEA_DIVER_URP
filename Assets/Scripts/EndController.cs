using UnityEngine;
using UnityEngine.SceneManagement;

public class EndController : MonoBehaviour
{
        public void Lobby()
        {
            SceneManager.LoadScene(0);
        }
        public void RE()
        {
            SceneManager.LoadScene(1);
        }
}
