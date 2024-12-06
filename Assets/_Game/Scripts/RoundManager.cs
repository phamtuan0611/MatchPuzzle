using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
    public float roundTime = 60f;
    private UIManager uiMan;

    private bool endingRound = false;
    private Board board;

    public float roundScore;
    public float displayScore;
    public float scoreSpeed;

    public int scoreTarget1, scoreTarget2, scoreTarget3;

    public string levelName;

    // Start is called before the first frame update
    void Awake()
    {
        //uiMan.bestScoretext.text = PlayerPrefs.GetString("BestScore");
        uiMan = FindObjectOfType<UIManager>();
        board = FindObjectOfType<Board>();
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("BestScore" + levelName))
        {
            uiMan.bestScoretext.text = PlayerPrefs.GetFloat("BestScore" + levelName).ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (roundTime > 0)
        {
            roundTime -= Time.deltaTime;

            if (roundTime <= 0)
            {
                roundTime = 0;

                endingRound = true;
            }
        }

        if (endingRound == true && board.currentState == Board.BoardState.move)
        {
            WinCheck();
            endingRound = false;
        }

        uiMan.timeText.text = roundTime.ToString("0.0") + "s";

        displayScore = Mathf.Lerp(displayScore, roundScore, scoreSpeed * Time.deltaTime);
        uiMan.scoreText.text = displayScore.ToString("0");
    }

    private void WinCheck()
    {
        //uiMan.roundOver.SetActive(true);
        uiMan.winGame.SetActive(true);

        uiMan.winScore.text = roundScore.ToString();

        if (!PlayerPrefs.HasKey("BestScore" + levelName))
        {
            PlayerPrefs.SetFloat("BestScore" + levelName, roundScore);

        }
        else
        {
            if (roundScore > PlayerPrefs.GetFloat("BestScore" + levelName))
            {
                PlayerPrefs.SetFloat("BestScore" + levelName, roundScore);
            }
        }

        Debug.Log("Best score in Level: " + PlayerPrefs.GetFloat("BestScore" + levelName));

        if (roundScore >= scoreTarget3)
        {
            uiMan.winText.text = "Congratulations! You earned 3 stars!";
            uiMan.winStars3.SetActive(true);

            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star1", 1);
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star2", 1);
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star3", 1);
        }
        else if (roundScore >= scoreTarget2)
        {
            uiMan.winText.text = "Congratulations! You earned 2 stars!";
            uiMan.winStars2.SetActive(true);

            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star1", 1);
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star2", 1);
        }
        else if (roundScore >= scoreTarget1)
        {
            uiMan.winText.text = "Congratulations! You earned 1 stars!";
            uiMan.winStars1.SetActive(true);

            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_Star1", 1);
        }
        else
        {
            uiMan.winText.text = "Oh no! No stars for you! Try again?";
        }

        SFXManager.instance.PlayRoundOver();
    }
}
