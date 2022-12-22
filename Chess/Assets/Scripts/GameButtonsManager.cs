using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameButtonsManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private List<Button> replayButton;
    [SerializeField] private List<Button> mainMenuButton;
    [SerializeField] private Button exitMenuButton;
    SceneLoader sceneLoader = new SceneLoader();

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        pauseButton.GetComponent<Button>().onClick.AddListener(PauseButtonOnClick);
        exitMenuButton.onClick.AddListener(ExitMenuButtonOnClick);

        foreach(Button b in replayButton)
        {
            b.onClick.AddListener(() => sceneLoader.LoadScene(SceneManager.GetActiveScene().name));
        }

        foreach(Button b in mainMenuButton)
        {
            b.onClick.AddListener(() => sceneLoader.LoadScene(SceneLoader.START_MENU_SCENE));
        }
    }

    private void PauseButtonOnClick()
    {
        gameManager.gameIsActive = false;
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
    }

    private void ExitMenuButtonOnClick()
    {
        gameManager.gameIsActive = true;
        pauseButton.SetActive(true);
        pauseMenu.SetActive(false);
    }

}
