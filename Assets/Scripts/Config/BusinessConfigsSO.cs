using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "BusinessConfigsSO", menuName = "Configs/BusinessConfigsSO", order = 1)]
    public class BusinessConfigsSO : ScriptableObject
    {
        public BusinessConfig[] Businesses;
    }
}
