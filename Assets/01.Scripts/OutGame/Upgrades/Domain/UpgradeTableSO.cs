using System.Collections.Generic;
using UnityEngine;

namespace OutGame.Upgrades.Domain
{
    [CreateAssetMenu(fileName = "UpgradeTable", menuName = "FoodTruck/Upgrade Table")]
    public class UpgradeTableSO : ScriptableObject
    {
        [SerializeField] private UpgradeSpec[] _specs;

        public IReadOnlyList<UpgradeSpec> AllSpecs => _specs;

        public UpgradeSpec GetSpec(EUpgradeType type)
        {
            if (_specs == null)   return null;
            

            foreach (var spec in _specs)
            {
                if (spec.Type == type)
                {
                    return spec;
                }
            }

            return null;
        }
    }
}
