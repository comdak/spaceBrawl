using UnityEngine;
using UnityEngine.UI;


public class PlayerCanvas : MonoBehaviour {


    public static PlayerCanvas canvas;

    [SerializeField] RectTransform LocalHealthBar;
    [SerializeField] RectTransform LocalHealthForeground;
    [SerializeField] RectTransform PrimaryReloadBar;
    [SerializeField] RectTransform PrimaryReloadForeground;
    [SerializeField] RectTransform SecondaryReloadBar;
    [SerializeField] RectTransform SecondaryReloadForeground;
    [SerializeField] RectTransform DefensiveRechargeBar;
    [SerializeField] RectTransform DefensiveRechargeForeground;
    [SerializeField] Text killsValue;
    [SerializeField] Text gameStatusText;

    private void Awake()
    {
        if (canvas == null)
            canvas = this;
        else if (canvas != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        //LocalHealthForeground = LocalHealthBar.Find("Foreground").GetComponent<RectTransform>();
    }

    public void Initialize()
    {
        
        gameStatusText.text = "";
    }



    private void Reset()
    {
        LocalHealthBar = GameObject.Find("Local Health Bar").GetComponent<RectTransform>();
        LocalHealthForeground = LocalHealthBar.Find("Foreground").GetComponent<RectTransform>();
        PrimaryReloadBar = GameObject.Find("Primary Reload Bar").GetComponent<RectTransform>();
        PrimaryReloadForeground = PrimaryReloadBar.Find("Foreground").GetComponent<RectTransform>();
        SecondaryReloadBar = GameObject.Find("Secondary Reload Bar").GetComponent<RectTransform>();
        SecondaryReloadForeground = SecondaryReloadBar.Find("Foreground").GetComponent<RectTransform>();
        DefensiveRechargeBar = GameObject.Find("Defense Recharge Bar").GetComponent<RectTransform>();
        DefensiveRechargeForeground = DefensiveRechargeBar.Find("Foreground").GetComponent<RectTransform>();
        killsValue = GameObject.Find("KillAmount").GetComponent<Text>();
        gameStatusText = GameObject.Find("GameStatusText").GetComponent<Text>();
    }

    public void SetHealth(float amount, float MaxHealth)
    {
        LocalHealthForeground.sizeDelta = new Vector2(amount / MaxHealth * LocalHealthBar.sizeDelta.x, LocalHealthBar.sizeDelta.y);
    }

    public void SetStatusBar(int key, float amount, float maxAmount)
    {
        if (key > 2 || key < 0)
            return;

        switch(key)
        {
            case 0:
                {
                    PrimaryReloadForeground.sizeDelta = new Vector2(PrimaryReloadBar.sizeDelta.x, (maxAmount - amount) / maxAmount * PrimaryReloadBar.sizeDelta.y);
                    break;
                }
            case 1:
                {
                    SecondaryReloadForeground.sizeDelta = new Vector2(SecondaryReloadBar.sizeDelta.x, (maxAmount - amount) / maxAmount * SecondaryReloadBar.sizeDelta.y);
                    break;
                }
            case 2:
                {
                    DefensiveRechargeForeground.sizeDelta = new Vector2(DefensiveRechargeBar.sizeDelta.x, (maxAmount - amount) / maxAmount * DefensiveRechargeBar.sizeDelta.y);
                    break;
                }
        }
    }

    public void SetKills(int amount)
    {
        killsValue.text = amount.ToString();
    }

    public void SetGameStatusText(string value)
    {
        gameStatusText.text = value;
    }

    public void ResetGameStatusText()
    {
        gameStatusText.text = "";
    }
}
