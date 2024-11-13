using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    [SerializeField]
    private GameObject Wallet;

    [SerializeField]
    private GameObject SpinWinnings;

    [SerializeField]
    private GameObject GoAgainButton;

    [SerializeField]
    private GameObject SpinCount;

    [SerializeField]
    private Sprite GoAgain;

    [SerializeField]
    private Sprite CantGo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameManager.walletChanged += UpdateWallet;
        GameManager.spinWinningsChanged += UpdateSpinWinnings;
        GameManager.spinsChanged += UpdateSpins;
        GoAgainButton.GetComponent<Button>().onClick.AddListener(FindAnyObjectByType<ReelStrip>().NewSpin);
    }

    private void OnDestroy()
    {
        // GameManager.walletChanged -= UpdateWallet;
        // GameManager.spinWinningsChanged -= UpdateSpinWinnings;
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateButton();
    }

    private void UpdateButton()
    {
        if (GameManager.currentReelState == ReelState.WAITING && GoAgainButton.GetComponent<Image>().sprite != GoAgain)
        {
            GoAgainButton.GetComponent<Image>().sprite = GoAgain;
            GoAgainButton.GetComponent<Button>().interactable = true;
        }
        else if (GameManager.currentReelState != ReelState.WAITING && GoAgainButton.GetComponent<Image>().sprite != CantGo)
        {
            GoAgainButton.GetComponent<Button>().interactable = false;
            GoAgainButton.GetComponent<Image>().sprite = CantGo;
        }
    }

    /// <summary>
    /// Observer
    /// </summary>
    private void UpdateWallet()
    {
        StringBuilder walletAmount = new StringBuilder(GameManager.WalletAmount.ToString());
        if (walletAmount.ToString().Contains('.'))
        {
            walletAmount[walletAmount.ToString().IndexOf('.') + 3] = '\0';
        }
        this.Wallet.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Wallet\r\n$" + " " + walletAmount;
    }

    /// <summary>
    /// Observer
    /// </summary>
    private void UpdateSpinWinnings()
    {
        StringBuilder spinWinningAmount = new StringBuilder(GameManager.SpinWinnings.ToString());
        if (spinWinningAmount.ToString().Contains('.'))
        {
            spinWinningAmount[spinWinningAmount.ToString().IndexOf('.') + 3] = '\0';
        }
        this.SpinWinnings.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Spin Winnings\r\n$" + " " + spinWinningAmount;
    }

    /// <summary>
    /// Observer
    /// </summary>
    private void UpdateSpins()
    {
        this.SpinCount.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Spin Count:" + " " + GameManager.Spins.ToString();
    }
}
