using FoodTruckClicker.Events;
using FoodTruckClicker.Upgrade;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodTruckClicker.UI
{
    /// <summary>
    /// 업그레이드 버튼 UI
    /// </summary>
    public class UpgradeButtonUI : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField]
        private UpgradeData _upgradeData;

        [Header("UI 요소")]
        [SerializeField]
        private Button _button;

        [SerializeField]
        private Image _iconImage;

        [SerializeField]
        private TextMeshProUGUI _nameText;

        [SerializeField]
        private TextMeshProUGUI _levelText;

        [SerializeField]
        private TextMeshProUGUI _costText;

        [SerializeField]
        private TextMeshProUGUI _effectText;

        [SerializeField]
        private GameObject _maxLevelIndicator;

        [Header("색상")]
        [SerializeField]
        private Color _affordableColor = Color.white;

        [SerializeField]
        private Color _unaffordableColor = Color.gray;

        private UpgradeManager _upgradeManager;

        public void Initialize(UpgradeManager upgradeManager)
        {
            _upgradeManager = upgradeManager;

            if (_button != null)
            {
                _button.onClick.AddListener(OnButtonClicked);
            }

            SetupStaticUI();
            RefreshUI();
        }

        private void OnEnable()
        {
            GameEvents.OnGoldChanged += HandleGoldChanged;
            GameEvents.OnUpgradePurchased += HandleUpgradePurchased;
        }

        private void OnDisable()
        {
            GameEvents.OnGoldChanged -= HandleGoldChanged;
            GameEvents.OnUpgradePurchased -= HandleUpgradePurchased;
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnButtonClicked);
            }
        }

        private void HandleGoldChanged(int newGold)
        {
            RefreshUI();
        }

        private void HandleUpgradePurchased(string upgradeId, int newLevel)
        {
            if (_upgradeData != null && upgradeId == _upgradeData.UpgradeId)
            {
                RefreshUI();
            }
        }

        private void OnButtonClicked()
        {
            if (_upgradeManager == null || _upgradeData == null)
            {
                return;
            }

            _upgradeManager.TryPurchase(_upgradeData.UpgradeId);
        }

        private void SetupStaticUI()
        {
            if (_upgradeData == null)
            {
                return;
            }

            if (_iconImage != null && _upgradeData.Icon != null)
            {
                _iconImage.sprite = _upgradeData.Icon;
            }

            if (_nameText != null)
            {
                _nameText.text = _upgradeData.DisplayName;
            }
        }

        private void RefreshUI()
        {
            if (_upgradeManager == null || _upgradeData == null)
            {
                return;
            }

            string upgradeId = _upgradeData.UpgradeId;
            int currentLevel = _upgradeManager.GetLevel(upgradeId);
            int maxLevel = _upgradeData.MaxLevel;
            bool isMaxLevel = currentLevel >= maxLevel;

            // 레벨 표시
            if (_levelText != null)
            {
                _levelText.text = $"Lv.{currentLevel}";
            }

            // 최대 레벨 표시
            if (_maxLevelIndicator != null)
            {
                _maxLevelIndicator.SetActive(isMaxLevel);
            }

            // 비용 표시
            if (_costText != null)
            {
                if (isMaxLevel)
                {
                    _costText.text = "MAX";
                }
                else
                {
                    int cost = _upgradeManager.GetNextLevelCost(upgradeId);
                    _costText.text = $"{cost:N0}G";
                }
            }

            // 효과 표시
            if (_effectText != null)
            {
                _effectText.text = GetEffectDescription(currentLevel, isMaxLevel);
            }

            // 버튼 상태
            if (_button != null)
            {
                bool canUpgrade = _upgradeManager.CanUpgrade(upgradeId);
                _button.interactable = canUpgrade;

                // 색상 변경
                var colors = _button.colors;
                colors.normalColor = canUpgrade ? _affordableColor : _unaffordableColor;
                _button.colors = colors;
            }
        }

        private string GetEffectDescription(int currentLevel, bool isMaxLevel)
        {
            if (_upgradeData == null)
            {
                return "";
            }

            int displayLevel = isMaxLevel ? currentLevel : currentLevel + 1;
            if (displayLevel <= 0)
            {
                displayLevel = 1;
            }

            float value = _upgradeData.GetValue(displayLevel);

            switch (_upgradeData.TargetType)
            {
                case UpgradeTargetType.ClickRevenue:
                    return $"x{value:F1}";

                case UpgradeTargetType.CriticalChance:
                    return $"{value * 100:F0}%";

                case UpgradeTargetType.CriticalDamage:
                    return $"{value:F0}개";

                case UpgradeTargetType.ChefCount:
                    return $"{value:F0}명";

                case UpgradeTargetType.CookingSpeed:
                    return $"x{value:F1}";

                case UpgradeTargetType.MenuUnlock:
                    return $"Lv.{value:F0}";

                default:
                    return "";
            }
        }
    }
}
