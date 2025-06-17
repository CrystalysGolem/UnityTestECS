using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "BusinessConfig", menuName = "Configs/BusinessConfig")]
    public class BusinessConfig : ScriptableObject
    {
        [Header("Основные параметры бизнеса")]
        public float IncomeDelay;  
        public float BaseCost;     
        public float BaseIncome;   

        [Header("Улучшения")]
        public UpgradeData[] Upgrades; 
    }
}
