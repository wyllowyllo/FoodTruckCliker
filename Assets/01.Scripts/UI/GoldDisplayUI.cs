using Events;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// 골드 표시 UI
    /// </summary>
    public class GoldDisplayUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _goldText;
        private string _format = "{0:N0} $";

        private void OnEnable()
        {
            GameEvents.OnGoldChanged += HandleGoldChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnGoldChanged -= HandleGoldChanged;
        }

        private void HandleGoldChanged(long newGold)
        {
            UpdateDisplay(newGold);
        }

        private void UpdateDisplay(long gold)
        {
            if (_goldText != null)
            {
                _goldText.text = string.Format(_format, gold);
            }
        }
    }
}
