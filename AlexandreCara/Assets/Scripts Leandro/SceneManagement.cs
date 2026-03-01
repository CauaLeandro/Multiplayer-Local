using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("morte"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            
        }

        if (collision.gameObject.CompareTag("Portal"))
        {
            SceneManager.LoadScene("Scene 1");
        }

        if (collision.gameObject.CompareTag("Portal2"))
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
