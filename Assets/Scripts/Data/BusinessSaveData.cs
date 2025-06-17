using System;

namespace Data
{
    [Serializable]
    public class BusinessSaveData
    {
        public int Level;
        public float Progress;
        public UpgradeSaveData[] UpgradesPurchased;
    }
}
