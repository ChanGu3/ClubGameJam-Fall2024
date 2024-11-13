using UnityEngine;
using UnityEngine.Events;

public enum ReelState
{
    NEWSPIN,
    SPINNING,
    EMPTYSPIN,
    EMPTYING,
    CONNECTSLIMES,
    CONNECTING,
    ADDSLIMES,
    WAITING,
    GAMESTARTED,
    GAMEOVER,
    GAMEOVERSCREEN,
}


public class GameManager : MonoBehaviour
{
    [SerializeField]
    public static float startingAmount = 150f;

    [SerializeField]
    private static float winningAmount;

    public static event UnityAction walletChanged;
    public static event UnityAction spinWinningsChanged;
    public static event UnityAction spinsChanged;

    private static float walletAmount;
    public static float WalletAmount
    {
        get => walletAmount;
        set
        {
            walletAmount = value;

            walletChanged?.Invoke();
        }

    }

    private static float spinWinnings;
    public static float SpinWinnings
    {
        get => spinWinnings;
        set
        {
            spinWinnings = value;

            spinWinningsChanged?.Invoke();
        }

    }

    private static int spins;
    public static int Spins
    {
        get => spins;
        set
        {
            spins = value;

            spinsChanged?.Invoke();
        }

    }

    /// <summary>
    /// Only One Instance Of GameManager.
    /// </summary>
    public static GameManager gameManager = null;

    /// <summary>
    /// Only One Instance Of SlimeScriptableObject.
    /// </summary>
    public static SlimeScriptableObject slimeScriptableObject = null;

    public static ReelState currentReelState = ReelState.WAITING;

    public static GameObject UICanvas;

    public static GameObject UILoserScript;

    void Awake()
    {
        Singleton();
    }

    private void Singleton()
    {
        if (GameManager.gameManager == null)
        {
            GameManager.gameManager = this;
            GameManager.slimeScriptableObject = Resources.Load("Slimes/SlimeScriptableObject") as SlimeScriptableObject;
            GameManager.UICanvas = FindAnyObjectByType<UIScript>().gameObject;
            GameManager.UILoserScript = FindAnyObjectByType<UILoserScript>().gameObject;
            GameManager.UILoserScript.SetActive(false);
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Update()
    {
    }
}
