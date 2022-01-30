using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!IsSceneLoaded("Auth"))
        {
            StartCoroutine(LoadScene("Auth", LoadSceneMode.Additive));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal IEnumerator LoadScene(string sceneName, LoadSceneMode mode)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, mode);
    }

    internal IEnumerator UnloadScene(string sceneName)
    {
        yield return SceneManager.UnloadSceneAsync(sceneName);
    }

    internal bool IsSceneLoaded(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);

        if (scene.name == null)
        {
            return false;
        }

        return true;
    }
}
