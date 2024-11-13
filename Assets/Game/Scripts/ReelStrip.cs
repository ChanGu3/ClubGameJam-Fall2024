using System.Collections.Generic;
using UnityEngine;

public class ReelStrip : MonoBehaviour
{
    /// <summary>
    /// slimes in the reel.
    /// </summary>
    private int totalSlimes = 25;

    /// <summary>
    /// slimes being counted
    /// </summary>
    private int currentSlimes = 0;

    /// <summary>
    /// Prefab To A Slime Object.
    /// </summary>
    [SerializeField]
    private GameObject SlimePrefab;

    /// <summary>
    /// child of ReelStrip.
    /// </summary>
    private GameObject slimeArrayPosition;

    /// <summary>
    /// child of slimeArrayPosition.
    /// </summary>
    private GameObject rowSpawn;

    private List<GameObject> rowSpawnSlimes = new List<GameObject>(5);

    private List<List<GameObject>> slimeSlotReel = new List<List<GameObject>>(5)
    {
        new List<GameObject>() { (null), (null), (null), (null), (null), },
        new List<GameObject>() { (null), (null), (null), (null), (null), },
        new List<GameObject>() { (null), (null), (null), (null), (null), },
        new List<GameObject>() { (null), (null), (null), (null), (null), },
        new List<GameObject>() { (null), (null), (null), (null), (null), },
    };

    /// <summary>
    /// Column 0 in the ReelStrip
    /// </summary>
    private List<GameObject> columnZero = new List<GameObject>() { (null), (null), (null), (null), (null), };

    /// <summary>
    /// Column 0 in the ReelStrip
    /// </summary>
    private List<GameObject> columnOne = new List<GameObject>() { (null), (null), (null), (null), (null), };

    /// <summary>
    /// Column 0 in the ReelStrip
    /// </summary>
    private List<GameObject> columnTwo = new List<GameObject>() { (null), (null), (null), (null), (null), };

    /// <summary>
    /// Column 0 in the ReelStrip
    /// </summary>
    private List<GameObject> columnThree = new List<GameObject>() { (null), (null), (null), (null), (null), };

    /// <summary>
    /// Column 0 in the ReelStrip
    /// </summary>
    private List<GameObject> columnFour = new List<GameObject>() { (null), (null), (null), (null), (null), };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        UILoserScript.yesButtonClicked += GoBackToGame;

        this.slimeArrayPosition = this.transform.Find("SlimeArrayPosition").gameObject;
        this.rowSpawn = this.slimeArrayPosition.transform.Find("RowSpawn").gameObject;

        for (int index = 0; index < rowSpawnSlimes.Capacity; index++)
        {
            this.rowSpawnSlimes.Add(this.rowSpawn.transform.Find("slime" + index).gameObject);
        }
    }

    private void Start()
    {
        GameManager.currentReelState = ReelState.GAMESTARTED;
        AudioManager.PlayMusic(MusicAudio.SlotMachineGame);
        GameManager.WalletAmount = GameManager.startingAmount;
        GameManager.SpinWinnings = 0f;
        GameManager.Spins = 0;
        CreateNewReel();
    }

    // Update is called once per frame
    void Update()
    {
        // check if wallet is gg
        if (GameManager.WalletAmount < 50f && GameManager.currentReelState == ReelState.WAITING)
        {
            GameManager.currentReelState = ReelState.GAMEOVER;
        }

        switch (GameManager.currentReelState)
        {
            case ReelState.NEWSPIN:
                GameManager.currentReelState = ReelState.SPINNING;
                GameManager.Spins++;
                GameManager.WalletAmount -= 50f;
                CreateNewReel();
                break;
            case ReelState.EMPTYSPIN:
                AudioManager.PlaySFX(SFXAudio.ButtonPress);
                GameManager.currentReelState = ReelState.EMPTYING;
                GameManager.SpinWinnings = 0f;
                RemoveOldReel();
                break;
            case ReelState.CONNECTSLIMES:
                GameManager.currentReelState = ReelState.CONNECTING;
                List<List<GameObject>> tempHolder = FindWinnings();
                ProccessPossibleWins(tempHolder);
                if (tempHolder != null)
                {
                    if (GameManager.SpinWinnings > 50)
                    {
                        AudioManager.PlaySFX(SFXAudio.Over50);
                    }
                    else if (GameManager.SpinWinnings < 50)
                    {
                        AudioManager.PlaySFX(SFXAudio.NotOver50);
                    }

                    GameManager.currentReelState = ReelState.WAITING;
                }
                else
                {
                    AudioManager.PlaySFX(SFXAudio.NotOver50);
                    GameManager.currentReelState = ReelState.ADDSLIMES;
                }
                break;
            case ReelState.ADDSLIMES:
                GameManager.currentReelState = ReelState.WAITING;
                break;
            case ReelState.GAMEOVER:
                GameManager.currentReelState = ReelState.GAMEOVERSCREEN;
                GameManager.UICanvas.SetActive(false);
                GameManager.UILoserScript.SetActive(true);
                break;
        }

        if (Input.GetKeyDown(KeyCode.Space) && GameManager.currentReelState == ReelState.WAITING)
        {
            GameManager.currentReelState = ReelState.EMPTYSPIN;
        }
    }

    private void CreateNewReel()
    {
        for (int columnIndex = 0; columnIndex < slimeSlotReel.Capacity; columnIndex++)
        {
            for (int rowIndex = 0; rowIndex < 5; rowIndex++)
            {
                slimeSlotReel[rowIndex][columnIndex] = Instantiate(SlimePrefab, rowSpawnSlimes[columnIndex].transform.position, Quaternion.identity);
                slimeSlotReel[rowIndex][columnIndex].GetComponent<Slime>().CreationFinished += IncrementSlimeCount;
                slimeSlotReel[rowIndex][columnIndex].GetComponent<Slime>().RemovalFinished += DecrementSlimeCount;
                slimeSlotReel[rowIndex][columnIndex].GetComponent<Slime>().targetLocation = this.slimeArrayPosition.transform.Find("Row" + rowIndex).Find("slime" + columnIndex).position;
            }
        }
    }

    private void RemoveOldReel()
    {
        for (int columnIndex = 0; columnIndex < slimeSlotReel.Capacity; columnIndex++)
        {
            for (int rowIndex = 0; rowIndex < 5; rowIndex++)
            {
                slimeSlotReel[rowIndex][columnIndex].GetComponent<Slime>().targetLocation = this.slimeArrayPosition.transform.Find("RowRemoval").Find("slime" + columnIndex).position;
                slimeSlotReel[rowIndex][columnIndex].GetComponent<Slime>().isOnRemoveRow = true;
            }
        }
    }

    /// <summary>
    /// Gets a List Of All Winnings
    /// </summary>
    /// <returns></returns>
    private List<List<GameObject>> FindWinnings()
    {
        List<List<GameObject>> Matches = new List<List<GameObject>>();

        for (int rowIndex = 0; rowIndex < slimeSlotReel.Capacity; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < 5; columnIndex++)
            {
                if (slimeSlotReel[rowIndex][columnIndex].GetComponent<Slime>().marked != true)
                {
                    List<GameObject> MatchGroup = new List<GameObject>();
                    DFS(rowIndex, columnIndex, ref MatchGroup, slimeSlotReel[rowIndex][columnIndex].GetComponent<Slime>().color);
                    if (MatchGroup.Count > 3)
                    {
                        Matches.Add(MatchGroup);
                    }
                }
            }
        }

        for (int rowIndex = 0; rowIndex < slimeSlotReel.Capacity; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < 5; columnIndex++)
            {
                slimeSlotReel[rowIndex][columnIndex].GetComponent<Slime>().marked = false;
            }
        }

        if (Matches.Count == 0)
        {
            return null;
        }
        else
        {
            return Matches;
        }
    }

    private static readonly int[] RowOffsets = { -1, 1, 0, 0 }; // Up, Down, Left, Right // ChatGpt
    private static readonly int[] ColOffsets = { 0, 0, -1, 1 }; // Up, Down, Left, Right // ChatGpt

    /// <summary>
    /// Chatgpt helped alot here... 
    /// </summary>
    private void DFS(int row, int col, ref List<GameObject> matchGroup, string targetValue)
    {
        // Check bounds and whether the cell has been visited or if it does not match the target value
        if (row < 0 || col < 0 || row >= 5 || col >= 5 || slimeSlotReel[row][col].GetComponent<Slime>().marked || slimeSlotReel[row][col].GetComponent<Slime>().color != targetValue)
            return;

        // Mark the cell as visited
        slimeSlotReel[row][col].GetComponent<Slime>().marked = true;

        // Add this cell to the match group
        matchGroup.Add(slimeSlotReel[row][col]);

        // Explore neighboring cells (up, down, left, right)
        for (int i = 0; i < 4; i++)
        {
            int newRow = row + RowOffsets[i];
            int newCol = col + ColOffsets[i];
            DFS(newRow, newCol, ref matchGroup, targetValue);
        }
    }

    private void ProccessPossibleWins(List<List<GameObject>> MatchGroups)
    {
        if (MatchGroups != null)
        {
            foreach (List<GameObject> matches in MatchGroups)
            {
                for (int index = 0; index < matches.Count; index++)
                {
                    matches[index].GetComponent<Slime>().AnimateExplode();
                }

                float multiplier = 1f;
                if (matches.Count > 4)
                {
                    multiplier = 1f + (0.2f * (matches.Count - 4));
                }


                GameManager.SpinWinnings += WinningData.Data[matches[0].GetComponent<Slime>().color] * multiplier;
                GameManager.WalletAmount += WinningData.Data[matches[0].GetComponent<Slime>().color] * multiplier;
            }
            AudioManager.PlaySFX(SFXAudio.WinMoney);
        }
    }

    /// <summary>
    /// Observer whenever A slime has finished its animation.
    /// </summary>
    private void IncrementSlimeCount()
    {
        AudioManager.PlaySFX(SFXAudio.SlimeInReel);
        this.currentSlimes++;
        if (this.currentSlimes == this.totalSlimes && GameManager.currentReelState == ReelState.SPINNING && GameManager.currentReelState != ReelState.GAMESTARTED)
        {
            GameManager.currentReelState = ReelState.CONNECTSLIMES;
        }
        else if (this.currentSlimes == this.totalSlimes && GameManager.currentReelState == ReelState.GAMESTARTED)
        {
            GameManager.currentReelState = ReelState.WAITING;
        }
    }

    /// <summary>
    /// Observer whenever A slime has been deleted.
    /// </summary>
    private void DecrementSlimeCount(Slime slime)
    {
        slime.GetComponent<Slime>().CreationFinished -= IncrementSlimeCount;
        slime.GetComponent<Slime>().RemovalFinished -= DecrementSlimeCount;

        this.currentSlimes--;
        if (this.currentSlimes == 0 && GameManager.currentReelState == ReelState.EMPTYING)
        {
            GameManager.currentReelState = ReelState.NEWSPIN;
        }
    }

    /// <summary>
    /// Observer new spins from the UI.
    /// </summary>
    public void NewSpin()
    {
        GameManager.currentReelState = ReelState.EMPTYSPIN;
    }


    public void GoBackToGame()
    {
        GameManager.UILoserScript.SetActive(false);
        GameManager.UICanvas.SetActive(true);
        GameManager.WalletAmount = 150f;
        GameManager.currentReelState = ReelState.WAITING;
    }

    private void CreateColumn(List<GameObject> column, int columnNum)
    {
        for (int rowIndex = 0; rowIndex < column.Count; rowIndex++)
        {
            column[rowIndex] = Instantiate(SlimePrefab, rowSpawnSlimes[columnNum].transform.position, Quaternion.identity);
            column[rowIndex].GetComponent<Slime>().targetLocation = this.slimeArrayPosition.transform.Find("Row" + rowIndex).Find("slime" + columnNum).position;
        }
    }
    private void RemoveColumn(List<GameObject> column, int columnNum)
    {
        for (int rowIndex = 0; rowIndex < column.Count; rowIndex++)
        {
            column[rowIndex].GetComponent<Slime>().targetLocation = this.slimeArrayPosition.transform.Find("RowRemoval").Find("slime" + columnNum).position;
            column[rowIndex].GetComponent<Slime>().isOnRemoveRow = true;
        }
    }
}
