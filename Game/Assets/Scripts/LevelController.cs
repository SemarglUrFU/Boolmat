using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using static UnityEngine.InputSystem.InputAction;

public class LevelController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Animator sceneTransiton;
    [SerializeField] GameObject winMenu;
    [SerializeField] GameObject menu;
    [SerializeField] string mainMenuSceneName;
    InputActions input;

    private void Awake() => StartCoroutine(AwakeCoroutine());
    private IEnumerator AwakeCoroutine()
    {
        sceneTransiton.SetTrigger("SceneOpening");
        playerController.transform.position = spawnPoint.position;
        yield return new WaitForSeconds(0.5f);
        playerController.enabled = true;
        input = new();
        input.Enable();
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
        yield return new WaitForSeconds(0.5f);
        playerController.enabled = false;
        winMenu.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        input.UI.Any.started += WinMenuExit;
    }
    private void WinMenuExit(CallbackContext ctx)
    {
        input.UI.Any.started -= WinMenuExit;
        ExitToMainMenu();
    }

    public void ExitToMainMenu() => StartCoroutine(ExitToMainMenuCoroutine());
    private IEnumerator ExitToMainMenuCoroutine()
    {
        input.Disable();
        sceneTransiton.SetTrigger("SceneClosing");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OpenMenu()
    {
        playerController.enabled = false;
        menu.SetActive(true);
        input.UI.Close.started += CloseMenuAction;
    }

    public void CloseMenu()
    {
        input.UI.Close.started -= CloseMenuAction;
        menu.SetActive(false);
        playerController.enabled = true;
    }
    private void CloseMenuAction(CallbackContext obj) => CloseMenu();
}
