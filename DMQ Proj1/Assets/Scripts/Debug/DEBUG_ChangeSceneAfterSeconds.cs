using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class DEBUG_ChangeSceneAfterSeconds : MonoBehaviour
{
    public float Time = 5f;

    public int SceneBuildIndex;

    void Start()
    {
        StartCoroutine(ChangeSceneAfterSeconds(Time));
    }

    protected IEnumerator ChangeSceneAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);

        SceneManager.LoadScene(SceneBuildIndex);
    }
}
