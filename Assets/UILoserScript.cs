using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UILoserScript : MonoBehaviour
{
    [SerializeField]
    private GameObject YesButton;

    [SerializeField]
    private GameObject SpinCountGameObject;

    public static event UnityAction yesButtonClicked;

    void Awake()
    {
        YesButton.GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    void OnEnable()
    {
        // changed to current spincount
        this.SpinCountGameObject.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Spin Count:" + " " + GameManager.Spins.ToString();
    }

    private void OnButtonClick()
    {
        yesButtonClicked?.Invoke();
    }

    void OnDisable()
    {
        GameManager.Spins = 0;
    }
}
