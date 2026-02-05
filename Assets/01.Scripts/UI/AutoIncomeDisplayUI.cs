using Events;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// 자동 수익 표시 UI
    /// </summary>
    public class AutoIncomeDisplayUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _incomeText;

        [SerializeField]
        private string _format = "+{0:F1}G/초";

        [SerializeField]
        private GameObject _displayRoot;

        private void OnEnable()
        {
            IncomeEvents.OnAutoIncomeChanged += HandleAutoIncomeChanged;
        }

        private void OnDisable()
        {
            IncomeEvents.OnAutoIncomeChanged -= HandleAutoIncomeChanged;
        }

        private void HandleAutoIncomeChanged(float incomePerSecond)
        {
            UpdateDisplay(incomePerSecond);
        }

        private void UpdateDisplay(float income)
        {
            // 자동 수익이 0이면 숨김
            if (_displayRoot != null)
            {
                _displayRoot.SetActive(income > 0);
            }

            if (_incomeText != null)
            {
                _incomeText.text = string.Format(_format, income);
            }
        }
    }
}
