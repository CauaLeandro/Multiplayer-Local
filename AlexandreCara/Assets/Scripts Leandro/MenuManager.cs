using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void CarregarCena(string nomeCena)
    {
        SceneManager.LoadScene(nomeCena);
    }

    public void SairDoJogo()
    {
        Application.Quit();
        Debug.Log("Saiu do jogo"); // Funciona só no build
    }
}