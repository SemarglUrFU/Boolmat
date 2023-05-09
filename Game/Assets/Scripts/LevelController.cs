using System.Collections;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Animator sceneTransiton;

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
}
