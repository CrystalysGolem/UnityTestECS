using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "BusinessNamesSO", menuName = "Configs/BusinessNamesSO", order = 2)]
    public class BusinessNamesSO : ScriptableObject
    {
        public string[] BusinessNames;
    }
}
