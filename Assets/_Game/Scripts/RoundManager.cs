using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG;
using DG.Tweening;

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

    private bool checkIntroduce;

    // Start is called before the first frame update
    void Awake()
    {
        checkIntroduce = false;
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

        StartCoroutine(IntroduceLevel());
    }

    // Update is called once per frame
    void Update()
    {
        if (roundTime > 0 && checkIntroduce == true)
        {
            roundTime -= Time.deltaTime;

            if (roundTime <= 0)
            {
                roundTime = 0;
            }
        }

        if (roundTime == 0 && board.currentState == Board.BoardState.move && checkIntroduce == true)
        {
            checkIntroduce = false;

            if (roundScore >= scoreTarget3)
            {
                StartCoroutine(Animation3Star());
            }
            else
            {
                StartCoroutine(fadeGems());
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

    private IEnumerator IntroduceLevel()
    {
        uiMan.introduceLevel.SetActive(true);
        board.currentState = Board.BoardState.wait;

        yield return new WaitForSeconds(2f);

        uiMan.introduceLevel.SetActive(false);
        board.currentState = Board.BoardState.move;
        checkIntroduce = true;
    }

    private IEnumerator fadeGems()
    {
        yield return new WaitForSeconds(1f);

        for (int y = board.height - 1; y >= 0; y--)
        {
            for (int x = board.width - 1; x >= 0; x--)
            {
                GameObject gems = board.allGems[x, y].gameObject;
                SpriteRenderer spriteRenderer = gems.GetComponent<SpriteRenderer>();
                spriteRenderer.DOColor(new Color(0.5f, 0.5f, 0.5f), 0.5f);

            }
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);

        endingRound = true;
    }

    private IEnumerator Animation3Star()
    {
        float waveDuration = 0.5f; // Thời gian cho mỗi gợn sóng
        float delayBetweenWaves = 0.05f; // Thời gian chờ giữa mỗi ô được xử lý
        int centerRow = board.width / 2;
        int centerCol = board.height / 2;

        // Hiệu ứng thu nhỏ (scale xuống)
        for (int distance = 0; distance <= Mathf.Max(board.width, board.height); distance++)
        {
            for (int row = 0; row < board.width; row++)
            {
                for (int col = 0; col < board.height; col++)
                {
                    if (Mathf.Abs(row - centerRow) + Mathf.Abs(col - centerCol) == distance)
                    {
                        board.allGems[row, col].gameObject.transform.DOScale(Vector3.zero, waveDuration).SetEase(Ease.OutSine);
                    }
                }
            }

            yield return new WaitForSeconds(delayBetweenWaves);
        }

        yield return new WaitForSeconds(0.1f); // Thời gian chờ sau hiệu ứng thu nhỏ

        // Hiệu ứng phóng to (scale lên)
        for (int distance = 0; distance <= Mathf.Max(board.width, board.height); distance++)
        {
            for (int row = 0; row < board.width; row++)
            {
                for (int col = 0; col < board.height; col++)
                {
                    if (Mathf.Abs(row - centerRow) + Mathf.Abs(col - centerCol) == distance)
                    {
                        board.allGems[row, col].gameObject.transform.DOScale(Vector3.one, waveDuration).SetEase(Ease.OutSine);
                    }
                }
            }

            yield return new WaitForSeconds(delayBetweenWaves);
        }

        yield return new WaitForSeconds(1f);
        endingRound = true;
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
