using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControlAction : EventActionComponent
{
    public string sceneName = "Game";

    public override IEnumerator Execute(EventContext ctx)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
        yield return null;
    }

}
