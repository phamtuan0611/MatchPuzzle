using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG;
using DG.Tweening;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    public GameObject bgTilePrefab;

    public Gem[] gems;

    public Gem[,] allGems;

    public float gemSpeed;

    [HideInInspector]
    public MatchFinder matchFind;

    public enum BoardState { wait, move }
    public BoardState currentState = BoardState.move;

    public Gem bomb;
    public float bombChance = 2f;

    public RoundManager roundManager;

    private float bonusMulti;
    public float bonusAmount = 0.5f;

    private BoardLayOut boardLayOut;
    private Gem[,] layoutStore;

    public int showScore;
    public int bonusScore;

    private UIManager uiMan;

    private float timeFade;

    private void Awake()
    {
        matchFind = FindObjectOfType<MatchFinder>();
        roundManager = FindObjectOfType<RoundManager>();
        boardLayOut = GetComponent<BoardLayOut>();
        uiMan = FindObjectOfType<UIManager>();
        showScore = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        allGems = new Gem[width, height];

        layoutStore = new Gem[width, height];

        Setup();

        timeFade = 0f;
    }

    private void Update()
    {
        //matchFind.FindAllMatches();

        if (Input.GetKeyDown(KeyCode.S))
        {
            ShuffleBoard();
        }

        if (timeFade > 0f)
        {
            timeFade -= Time.deltaTime;
        }
    }

    private void Setup()
    {
        if (boardLayOut != null)
        {
            layoutStore = boardLayOut.GetLayOut();
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Sinh BGBoard cua Gem tai vi tri (x, y)
                //Lan luot tu 0,0 - 0,1 - 0,2 ...
                Vector2 pos = new Vector2(x, y);
                GameObject bgTile = Instantiate(bgTilePrefab, pos, Quaternion.identity);
                bgTile.transform.parent = transform; //Dat Parent la GameObject Board trong Hierarchy
                bgTile.name = "BG Tile - " + x + ", " + y;

                if (layoutStore[x, y] != null)
                {
                    SpawnGem(new Vector2Int(x, y), layoutStore[x, y]);
                    //DestroyMatches();
                }
                else
                {
                    int gemToUse = Random.Range(0, gems.Length); //Lay ngau nhien Gem tu mang gems

                    int iterations = 0;
                    while (MatchesAt(new Vector2Int(x, y), gems[gemToUse]) && iterations < 100)
                    {
                        //Kiem tra xem khi bat dau co cai nao bi AN khong, neu co thi sinh lai gem
                        gemToUse = Random.Range(0, gems.Length);
                        iterations++;
                    }

                    SpawnGem(new Vector2Int(x, y), gems[gemToUse]);
                }
            }
        }
    }

    private void SpawnGem(Vector2Int pos, Gem gemToSpawn)
    {
        if (Random.Range(0f, 100f) < bombChance)
        {
            gemToSpawn = bomb;
        }

        //Sinh Gem tai vi tri (x, y + height)
        Gem gem = Instantiate(gemToSpawn, new Vector3(pos.x, pos.y + height, 0f), Quaternion.identity);
        gem.transform.parent = transform;
        gem.name = "Gem - " + pos.x + ", " + pos.y;
        allGems[pos.x, pos.y] = gem; //Luu gem duoc sinh ra vao mang 2 chieu

        gem.SetupGem(pos, this); //Luu vi tri cua Gem phai o
    }

    bool MatchesAt(Vector2Int posToCheck, Gem gemToCheck)
    {
        if (posToCheck.x > 1)
        {
            //Kiem tra tu cot 2
            if (allGems[posToCheck.x - 1, posToCheck.y].type == gemToCheck.type && allGems[posToCheck.x - 2, posToCheck.y].type == gemToCheck.type)
            {
                return true;
            }
        }

        if (posToCheck.y > 1)
        {
            //Kiem tra tu hang 2
            if (allGems[posToCheck.x, posToCheck.y - 1].type == gemToCheck.type && allGems[posToCheck.x, posToCheck.y - 2].type == gemToCheck.type)
            {
                return true;
            }
        }

        return false;
    }

    private void DestroyMatchedGemAt(Vector2Int pos)
    {
        if (allGems[pos.x, pos.y] != null)
        {
            if (allGems[pos.x, pos.y].isMatched)
            {
                if (allGems[pos.x, pos.y].type == Gem.GemType.bomb)
                {
                    SFXManager.instance.PlayExplode();
                }
                else if (allGems[pos.x, pos.y].type == Gem.GemType.stone)
                {
                    SFXManager.instance.PlayStoneBreak();
                }
                else
                {
                    SFXManager.instance.PlayGemBreak();
                }

                Instantiate(allGems[pos.x, pos.y].destroyEffect, new Vector2(pos.x, pos.y), Quaternion.identity);

                Destroy(allGems[pos.x, pos.y].gameObject);
                allGems[pos.x, pos.y] = null;
            }
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < matchFind.currentMatches.Count; i++)
        {
            if (matchFind.currentMatches[i] != null)
            {
                ScoreCheck(matchFind.currentMatches[i]);

                DestroyMatchedGemAt(matchFind.currentMatches[i].posIndex);
            }
        }

        Debug.Log("Show score: " + showScore);

        StartCoroutine(ShowScore());

        Debug.Log("Bonus score: " + bonusScore);

        StartCoroutine(DecreaseRowCo());
    }
    private IEnumerator ShowScore()
    {
        GameObject showScoreInstance = Instantiate(uiMan.showScore, uiMan.transform.position, Quaternion.identity);
        showScoreInstance.transform.SetParent(uiMan.transform);
        //showScoreInstance.transform.position = new Vector3(960f, 540f, 0);

        TextMeshProUGUI scoreTextComponent = showScoreInstance.transform.Find("Score").GetComponent<TextMeshProUGUI>();
        if (scoreTextComponent != null)
        {
            scoreTextComponent.text = showScore.ToString();
        }
        showScoreInstance.transform.localScale = Vector3.zero;
        showScoreInstance.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        showScore = 0;

        yield return new WaitForSeconds(0.75f);

        showScoreInstance.transform.DOScale(Vector3.zero, 0.25f);

        yield return new WaitForSeconds(0.25f);

        Destroy(showScoreInstance);

        yield return new WaitForSeconds(0.25f);

        if (bonusScore != 0)
        {
            GameObject showBonusScoreInstance = Instantiate(uiMan.showBonusScore, uiMan.transform.position, Quaternion.identity);
            showBonusScoreInstance.transform.SetParent(uiMan.transform);
            //showBonusScoreInstance.transform.position = new Vector3(960f, 540f, 0);

            TextMeshProUGUI scoreTextBonusComponent = showBonusScoreInstance.transform.Find("BonusScore").GetComponent<TextMeshProUGUI>();
            if (scoreTextBonusComponent != null)
            {
                scoreTextBonusComponent.text = bonusScore.ToString();
            }

            showBonusScoreInstance.transform.localScale = Vector3.zero;
            showBonusScoreInstance.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            bonusScore = 0;

            yield return new WaitForSeconds(0.75f);

            showBonusScoreInstance.transform.DOScale(Vector3.zero, 0.25f);

            yield return new WaitForSeconds(0.25f);
            Destroy(showBonusScoreInstance);
        }
    }

    private IEnumerator DecreaseRowCo()
    {
        yield return new WaitForSeconds(0.2f);

        int nullCounter = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null)
                {
                    nullCounter++;
                }
                else if (nullCounter > 0)
                {
                    allGems[x, y].posIndex.y -= nullCounter;
                    allGems[x, y - nullCounter] = allGems[x, y];
                    allGems[x, y] = null;
                }
            }

            nullCounter = 0;
        }

        StartCoroutine(FillBoardCo());
    }

    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(0.5f);
        RefillBoard();

        yield return new WaitForSeconds(0.5f);
        matchFind.FindAllMatches();

        if (matchFind.currentMatches.Count > 0)
        {
            bonusMulti++;

            yield return new WaitForSeconds(0.5f);
            DestroyMatches();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            currentState = BoardState.move;

            bonusMulti = 0f;
        }
    }

    private void RefillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null)
                {
                    int gemToUse = Random.Range(0, gems.Length);

                    SpawnGem(new Vector2Int(x, y), gems[gemToUse]);
                }
            }
        }

        CheckMisplacedGems();
    }

    //Duoc dung de loai bo nhung Gem bi trung lap
    //Dam bao so luong Gem trong allGems luon bang so luong Gems co tren man hinh
    private void CheckMisplacedGems()
    {
        List<Gem> foundGems = new List<Gem>();

        foundGems.AddRange(FindObjectsOfType<Gem>());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (foundGems.Contains(allGems[x, y]))
                {
                    foundGems.Remove(allGems[x, y]);
                }
            }
        }

        foreach (Gem g in foundGems)
        {
            Destroy(g.gameObject);
        }
    }

    public void ShuffleBoard()
    {
        if (currentState != BoardState.wait)
        {
            currentState = BoardState.wait;

            List<Gem> gemsFromBoard = new List<Gem>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gemsFromBoard.Add(allGems[x, y]);
                    allGems[x, y] = null;
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int gemToUse = Random.Range(0, gemsFromBoard.Count);

                    int iterations = 0;
                    while (MatchesAt(new Vector2Int(x, y), gemsFromBoard[gemToUse]) && iterations < 100 && gemsFromBoard.Count > 1)
                    {
                        gemToUse = Random.Range(0, gemsFromBoard.Count);
                        iterations++;
                    }

                    gemsFromBoard[gemToUse].SetupGem(new Vector2Int(x, y), this);
                    allGems[x, y] = gemsFromBoard[gemToUse];
                    gemsFromBoard.RemoveAt(gemToUse);
                }
            }

            StartCoroutine(FillBoardCo());
        }
    }

    public void ScoreCheck(Gem gemToCheck)
    {
        roundManager.roundScore += gemToCheck.scoreValue;

        showScore += gemToCheck.scoreValue;


        if (bonusMulti > 0)
        {
            Debug.Log("Bonus Multi: " + bonusMulti);
            float bonusToAdd = gemToCheck.scoreValue * bonusMulti * 0.75f + bonusMulti;
            roundManager.roundScore += Mathf.RoundToInt(bonusToAdd);
            bonusScore += Mathf.RoundToInt(bonusToAdd);
        }
    }


}
