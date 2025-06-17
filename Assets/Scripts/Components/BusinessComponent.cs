using Configs;

namespace Components
{
    public struct BusinessComponent
    {
        public int Id;
        public int Level;
        public BusinessConfig Config;
        public string Name;

        public bool[] UpgradesPurchased;
    }
}
