using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "BusinessConfig", menuName = "Configs/BusinessConfig")]
    public class BusinessConfig : ScriptableObject
    {
        [Header("�������� ��������� �������")]
        public float IncomeDelay;  
        public float BaseCost;     
        public float BaseIncome;   

        [Header("���������")]
        public UpgradeData[] Upgrades; 
    }
}
