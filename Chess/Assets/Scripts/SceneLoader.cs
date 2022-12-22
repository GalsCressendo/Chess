using UnityEngine.SceneManagement;

public class SceneLoader
{
    public const string START_MENU_SCENE = "StartMenu";
    public const string TWO_PLAYER_SCENE = "2Player";
    public const string VS_AI_SCENE = "VS_AI";

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
