using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Animator sceneTransiton;
    [SerializeField] Image winImage;
    [SerializeField] int nextLevelId;
    [SerializeField] string mainMenuSceneName;

    private void Awake()
    {
        StartCoroutine(AwakeCoroutine());
    }

    private IEnumerator AwakeCoroutine()
    {
        sceneTransiton.SetTrigger("SceneOpening");
        playerController.transform.position = spawnPoint.position;
        yield return new WaitForSeconds(0.5f);
        playerController.enabled = true;
    }

    public void RespawnPlayer() => StartCoroutine(RespawnPlayerCoroutine());
    private IEnumerator RespawnPlayerCoroutine()
    {
        playerController.enabled = false;
        yield return new WaitForSeconds(1.0f);
        sceneTransiton.SetTrigger("SceneFade");
        yield return new WaitForSeconds(0.5f);
        playerController.transform.position = spawnPoint.position;
        playerController.ResetAnimations();
        yield return new WaitForSeconds(0.5f);
        playerController.enabled = true;
    }

    public void Win() => StartCoroutine(WinCoroutine());
    private IEnumerator WinCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        playerController.enabled = false;
        winImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        sceneTransiton.SetTrigger("SceneClosing");
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
