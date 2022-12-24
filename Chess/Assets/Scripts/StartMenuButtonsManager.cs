using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuButtonsManager : MonoBehaviour
{
    [SerializeField] private Button twoPlayerButton;
    [SerializeField] private Button vsAIButton;
    [SerializeField] private Button quitButton;
    SceneLoader sceneLoader = new SceneLoader();

    private void Start()
    {
        twoPlayerButton.onClick.AddListener(() => sceneLoader.LoadScene(SceneLoader.TWO_PLAYER_SCENE));
        vsAIButton.onClick.AddListener(() => sceneLoader.LoadScene(SceneLoader.VS_AI_SCENE));
        quitButton.onClick.AddListener(QuitButtonOnClick);
    }

    private void QuitButtonOnClick()
    {
        FindObjectOfType<AudioManager>().PlayAudio(AudioManager.BOARD_START_AUDIO);
        Application.Quit();
    }
}
