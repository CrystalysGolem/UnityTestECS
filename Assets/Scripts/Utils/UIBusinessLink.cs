using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBusinessLink : MonoBehaviour
{
    public int BusinessId;
    public GameObject Root;
    public TMP_Text BusinessNameText;
    public TMP_Text LevelText;
    public TMP_Text IncomeText;
    public Slider ProgressBar;

    public Button LevelUpButton;
    public TMP_Text LevelUpButtonText;
    public TMP_Text LevelUpButtonPriceText;

    public Button[] UpgradeButtons;
    public TMP_Text[] UpgradeButtonNameTexts;
    public TMP_Text[] UpgradeButtonIncomeTexts;
    public TMP_Text[] UpgradeButtonPriceTexts;
}
