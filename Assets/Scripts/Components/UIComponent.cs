using TMPro;
using UnityEngine.UI;

namespace Components
{
    public struct UIComponent
    {
        public TMP_Text BusinessNameText;
        public TMP_Text LevelText;
        public TMP_Text IncomeText;
        public Slider ProgressBar;

        public Button LevelUpButton;
        public TMP_Text LevelUpButtonText;           
        public TMP_Text LevelUpButtonPriceText;      
        public bool LevelUpRequested;

        public Button[] UpgradeButtons;
        public TMP_Text[] UpgradeButtonNameTexts;    
        public TMP_Text[] UpgradeButtonIncomeTexts;  
        public TMP_Text[] UpgradeButtonPriceTexts;   
        public bool[] UpgradeRequested;
    }
}
