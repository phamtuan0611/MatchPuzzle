using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text scoreText;
    public TMP_Text shuffleText;
    public TMP_Text bestScoretext;

    public TMP_Text winScore;
    public TMP_Text winText;
    public GameObject winStars1, winStars2, winStars3;

    public GameObject roundOver;
    public GameObject winGame;

    public GameObject winPopup;

    private Board theBoard;

    public string levelSelect;

    public GameObject pauseScreen;

    private int count = 0;

    public string nextLevel;

    public GameObject introduceLevel;

    //public TMP_Text score, bonusScore;

    public GameObject showScore, showBonusScore;

    private void Awake()
    {
        theBoard = FindObjectOfType<Board>();
    }

    // Start is called before the first frame update
    void Start()
    {


        count = 3;
        winStars1.SetActive(false);
        winStars2.SetActive(false);
        winStars3.SetActive(false);

        TextMeshProUGUI scoreText = showScore.transform.Find("Score").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }

    }

    public void PauseUnpause()
    {
        if (!pauseScreen.activeInHierarchy)
        {
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void ShuffleBoard()
    {
        if (theBoard.currentState == Board.BoardState.move)
        {
            if (count > 0)
            {
                count--;
                Debug.Log(count);
                theBoard.ShuffleBoard();
                shuffleText.text = count.ToString();
            }
            else
            {
                return;
            }
        }
    }

    public void QuitGaem()
    {
        Application.Quit();
    }

    public void GoToSelectLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelSelect);
    }

    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }

    public void WinPopup()
    {
        winPopup.SetActive(true);
    }

}
