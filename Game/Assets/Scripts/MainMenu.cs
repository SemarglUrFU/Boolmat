using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Animator animator;

    private void Awake()
    {
        animator.SetTrigger("SceneOpening");
    }

    public void LoadLevel(int id) => StartCoroutine(LoadLevelCoroutine(id));
    public IEnumerator LoadLevelCoroutine(int id)
    {
        animator.SetTrigger("SceneClosing");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Level" + id);
    }

    public void ExitGame()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
