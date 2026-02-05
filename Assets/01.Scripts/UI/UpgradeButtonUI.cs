using Events;
using OutGame.Upgrades.Domain;
using OutGame.Upgrades.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UpgradeButtonUI : MonoBehaviour
    {
        [Header("업그레이드 타입")]
        [SerializeField] private EUpgradeType _upgradeType;

        [Header("UI 요소")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private TextMeshProUGUI _effectText;
        [SerializeField] private GameObject _maxLevelIndicator;

        [Header("클릭 피드백 색상")]
        [SerializeField] private Color _affordableColor = Color.white;
        [SerializeField] private Color _unaffordableColor = Color.gray;

        // 참조
        private UpgradeManager _upgradeManager;
        private Upgrade _upgrade;

        public void Initialize(UpgradeManager upgradeManager)
        {
            _upgradeManager = upgradeManager;
            _upgrade = _upgradeManager.GetUpgradeData(_upgradeType);

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

        private void HandleGoldChanged(long newGold)
        {
            RefreshUI();
        }

        private void HandleUpgradePurchased(EUpgradeType type, int newLevel)
        {
            if (type == _upgradeType)
            {
                RefreshUI();
            }
        }

        private void OnButtonClicked()
        {
            if (_upgradeManager == null || _upgrade == null)
            {
                return;
            }

            _upgradeManager.TryPurchase(_upgradeType);
        }

        private void SetupStaticUI()
        {
            if (_upgrade == null)
            {
                return;
            }

            if (_iconImage != null && _upgrade.Icon != null)
            {
                _iconImage.sprite = _upgrade.Icon;
            }

            if (_nameText != null)
            {
                _nameText.text = _upgrade.DisplayName;
            }
        }

        private void RefreshUI()
        {
            if (_upgradeManager == null || _upgrade == null)
            {
                return;
            }

            int currentLevel = _upgrade.Level;
            bool isMaxLevel = _upgrade.IsMaxLevel;

            if (_levelText != null)
            {
                _levelText.text = $"Lv.{currentLevel}";
            }

            if (_maxLevelIndicator != null)
            {
                _maxLevelIndicator.SetActive(isMaxLevel);
            }

            if (_costText != null)
            {
                if (isMaxLevel)
                {
                    _costText.text = "MAX";
                }
                else
                {
                    long cost = _upgrade.NextLevelCost;
                    _costText.text = $"{cost:N0}G";
                }
            }

            if (_effectText != null)
            {
                _effectText.text = GetEffectDescription(currentLevel, isMaxLevel);
            }

            if (_button != null)
            {
                bool canUpgrade = _upgradeManager.CanUpgrade(_upgradeType);
                _button.interactable = canUpgrade;

                var colors = _button.colors;
                colors.normalColor = canUpgrade ? _affordableColor : _unaffordableColor;
                _button.colors = colors;
            }
        }

        private string GetEffectDescription(int currentLevel, bool isMaxLevel)
        {
            if (_upgrade == null)
            {
                return "";
            }
            
            float value = _upgrade.Effect;

            switch (_upgradeType)
            {
                case EUpgradeType.ClickRevenue:
                    return $"x{value:F1}";

                case EUpgradeType.CriticalChance:
                    return $"{value * 100:F0}%";

                case EUpgradeType.CriticalProfit:
                    return $"{value:F0}개";

                case EUpgradeType.ChefCount:
                    return $"{value:F0}명";

                case EUpgradeType.CookingSpeed:
                    return $"x{value:F1}";

                case EUpgradeType.FoodTruck:
                    return $"Lv.{value:F0}";

                default:
                    return "";
            }
        }
    }
}
