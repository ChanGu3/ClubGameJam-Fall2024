using UnityEngine;
using UnityEngine.Events;

public class Slime : MonoBehaviour
{
    /// <summary>
    /// color of slime.
    /// </summary>
    public string color
    {
        get;
        private set;
    }

    /// <summary>
    /// marked as looked at for finding winnings
    /// </summary>
    public bool marked
    {
        get;
        set;
    }

    /// <summary>
    /// targetLocation for slime.
    /// </summary>
    ///
    public Vector3 targetLocation
    {
        get;
        set;
    }

    public bool isOnRemoveRow = false;

    public event UnityAction<Slime> RemovalFinished;
    public event UnityAction CreationFinished;

    private void Start()
    {
        SetColor();
        SetSprite();
        marked = false;
    }

    // Update is called once per frame
    void Update()
    {
        GoToTargetLerp();
    }

    private RuntimeAnimatorController animatorController;
    public void AnimateExplode()
    {
        this.transform.Find("Sprite").GetComponent<Animator>().runtimeAnimatorController = animatorController;
    }

    private void SetColor()
    {
        float colorPickNumber = Random.Range(0f, 1f);
        if (colorPickNumber >= 0.75f)
        {
            this.color = "green";
        }
        else if (colorPickNumber < 0.75 && colorPickNumber >= 0.50)
        {
            this.color = "red";
        }
        else if (colorPickNumber < 0.50 && colorPickNumber >= 0.30)
        {
            this.color = "blue";
        }
        else if (colorPickNumber < 0.30 && colorPickNumber >= 0.15)
        {
            this.color = "yellow";
        }
        else if (colorPickNumber < 0.15 && colorPickNumber >= 0.05)
        {
            this.color = "orange";
        }
        else if (colorPickNumber < 0.05 && colorPickNumber >= 0)
        {
            this.color = "purple";
        }
    }

    private void SetSprite()
    {
        SpriteRenderer spriteRenderer = this.transform.Find("Sprite").GetComponent<SpriteRenderer>(); ;
        if (this.color == "red")
        {
            spriteRenderer.sprite = GameManager.slimeScriptableObject.RedSprite;
            animatorController = GameManager.slimeScriptableObject.RedController;
        }
        else if (this.color == "blue")
        {
            spriteRenderer.sprite = GameManager.slimeScriptableObject.BlueSprite;
            animatorController = GameManager.slimeScriptableObject.BlueController;
        }
        else if (this.color == "yellow")
        {
            spriteRenderer.sprite = GameManager.slimeScriptableObject.YellowSprite;
            animatorController = GameManager.slimeScriptableObject.YellowController;
        }
        else if (this.color == "purple")
        {
            spriteRenderer.sprite = GameManager.slimeScriptableObject.PurpleSprite;
            animatorController = GameManager.slimeScriptableObject.PurpleController;
        }
        else if (this.color == "orange")
        {
            spriteRenderer.sprite = GameManager.slimeScriptableObject.OrangeSprite;
            animatorController = GameManager.slimeScriptableObject.OrangeController;
        }
        else
        {
            spriteRenderer.sprite = GameManager.slimeScriptableObject.GreenSprite;
            animatorController = GameManager.slimeScriptableObject.GreenController;
        }
    }

    private void GoToTargetLerp()
    {
        if (targetLocation != this.transform.position)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, targetLocation, 1.5f * Time.deltaTime);

            if (Vector3.Distance(this.transform.position, targetLocation) < 0.1f)
            {
                this.transform.position = targetLocation;

                if (isOnRemoveRow)
                {
                    RemovalFinished?.Invoke(this);
                    Destroy(this.gameObject);
                }
                else
                {
                    CreationFinished?.Invoke();
                }
            }
        }
    }
}
