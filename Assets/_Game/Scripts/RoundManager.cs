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
    }
}
