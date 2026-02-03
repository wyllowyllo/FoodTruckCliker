using Events;
using Goods.Manager;
using Menu;
using Upgrade.Domain;
using Upgrade.Manager;
using UnityEngine;

namespace AutoIncome
{
    /// <summary>
    /// 자동 수익 관리자
    /// 공식: 요리사 수 × 클릭 수익 × 요리 속도 배율
    /// </summary>
    public class AutoIncomeManager : MonoBehaviour
    {
        [SerializeField]
        private float _incomeInterval = 1f;

        private UpgradeManager _upgradeProvider;
        private GoldManager _goldManager;
        private MenuManager _menuProvider;

        private float _timer;
        private float _cachedIncomePerSecond;

        public float IncomePerSecond => _cachedIncomePerSecond;

        public void Initialize(
            UpgradeManager upgradeProvider,
            GoldManager goldManager,
            MenuManager menuProvider)
        {
            _upgradeProvider = upgradeProvider;
            _goldManager = goldManager;
            _menuProvider = menuProvider;

            RecalculateIncome();
        }

        private void OnEnable()
        {
            GameEvents.OnUpgradePurchased += HandleUpgradePurchased;
        }

        private void OnDisable()
        {
            GameEvents.OnUpgradePurchased -= HandleUpgradePurchased;
        }

        private void Update()
        {
            if (_upgradeProvider == null || _goldManager == null)
            {
                return;
            }

            if (_cachedIncomePerSecond <= 0)
            {
                return;
            }

            _timer += Time.deltaTime;

            if (_timer >= _incomeInterval)
            {
                _timer -= _incomeInterval;
                ProcessAutoIncome();
            }
        }

        private void ProcessAutoIncome()
        {
            long goldToAdd = (long)(_cachedIncomePerSecond * _incomeInterval);

            if (goldToAdd > 0)
            {
                _goldManager.AddGold(goldToAdd);
                GameEvents.RaiseRevenueEarned(goldToAdd, false, 1, true);
            }
        }

        private void HandleUpgradePurchased(string upgradeId, int newLevel)
        {
            RecalculateIncome();
        }

        private void RecalculateIncome()
        {
            if (_upgradeProvider == null)
            {
                _cachedIncomePerSecond = 0f;
                return;
            }

            // 요리사 수
            int chefCount = _upgradeProvider.GetIntValue(EUpgradeType.ChefCount);

            if (chefCount <= 0)
            {
                _cachedIncomePerSecond = 0f;
                GameEvents.RaiseAutoIncomeChanged(_cachedIncomePerSecond);
                return;
            }

            // 평균 메뉴 가격
            float menuPrice = _menuProvider?.AveragePrice ?? 10f;

            // 클릭 수익 배율
            float clickRevenue = _upgradeProvider.GetValue(EUpgradeType.ClickRevenue);

            // 요리 속도 배율
            float cookingSpeed = _upgradeProvider.GetValue(EUpgradeType.CookingSpeed);

            // 기본 클릭 수익 = 메뉴 가격 × 클릭 수익 배율
            float baseClickIncome = menuPrice * clickRevenue;

            // 공식: 요리사 수 × 클릭 수익 × 요리 속도
            _cachedIncomePerSecond = chefCount * baseClickIncome * cookingSpeed;

            GameEvents.RaiseAutoIncomeChanged(_cachedIncomePerSecond);
        }
    }
}
