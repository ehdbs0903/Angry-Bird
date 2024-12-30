using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpriteUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadMainSceneAfterDelay(2f));
    }

    private IEnumerator LoadMainSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainScene");
    }
}
