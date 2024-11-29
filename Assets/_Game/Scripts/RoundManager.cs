using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Awake()
    {
        uiMan = FindObjectOfType<UIManager>();
        board = FindObjectOfType<Board>();
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
        uiMan.roundOver.SetActive(true);

        uiMan.winScore.text = roundScore.ToString();

        if (roundScore >= scoreTarget3)
        {
            uiMan.winText.text = "Congratulations! You earned 3 stars!";
            uiMan.winStars3.SetActive(true);
        }
        else if (roundScore >= scoreTarget2)
        {
            uiMan.winText.text = "Congratulations! You earned 2 stars!";
            uiMan.winStars2.SetActive(true);
        }
        else if (roundScore >= scoreTarget1)
        {
            uiMan.winText.text = "Congratulations! You earned 1 stars!";
            uiMan.winStars1.SetActive(true);
        }
        else
        {
            uiMan.winText.text = "Oh no! No stars for you! Try again?";
        }
    }
}
