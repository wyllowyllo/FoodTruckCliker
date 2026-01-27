# P0 구현 계획 - 푸드트럭 클리커

## 목표
GDD P0 기능 구현: 클릭 → 골드 획득 + 6개 업그레이드 시스템

---

## 폴더 구조
```
Assets/01.Scripts/
├── Core/
│   └── GameManager.cs
├── Currency/
│   ├── ICurrencyProvider.cs
│   ├── ICurrencyModifier.cs
│   └── GoldManager.cs
├── Click/
│   ├── IRevenueCalculator.cs
│   ├── ClickController.cs
│   └── ClickRevenueCalculator.cs
├── AutoIncome/
│   ├── IAutoIncomeProvider.cs
│   └── AutoIncomeManager.cs
├── Upgrade/
│   ├── UpgradeType.cs
│   ├── UpgradeData.cs (ScriptableObject)
│   ├── IUpgradeProvider.cs
│   └── UpgradeManager.cs
├── Events/
│   └── GameEvents.cs
└── UI/
    ├── GoldDisplayUI.cs
    ├── AutoIncomeDisplayUI.cs
    └── UpgradeButtonUI.cs

Assets/02.ScriptableObjects/
├── GameConfig.asset
└── Upgrades/ (6개 업그레이드 데이터)
```

---

## 구현 순서

### Phase 1: 기반 시스템
1. `UpgradeType.cs` - enum 정의
2. `GameEvents.cs` - 이벤트 시스템
3. `ICurrencyProvider.cs`, `ICurrencyModifier.cs` - 재화 인터페이스
4. `GoldManager.cs` - 골드 관리

### Phase 2: 클릭 시스템
5. `IRevenueCalculator.cs` - 수익 계산 인터페이스
6. `ClickController.cs` - 입력 처리
7. `ClickRevenueCalculator.cs` - 수익 계산

### Phase 3: 업그레이드 시스템
8. `UpgradeData.cs` - ScriptableObject
9. `IUpgradeProvider.cs` - 업그레이드 조회 인터페이스
10. `UpgradeManager.cs` - 업그레이드 관리

### Phase 4: 자동 수익
11. `IAutoIncomeProvider.cs` - 자동 수익 인터페이스
12. `AutoIncomeManager.cs` - 초당 자동 수익

### Phase 5: UI
13. `GoldDisplayUI.cs` - 골드 표시
14. `AutoIncomeDisplayUI.cs` - 초당 수익 표시
15. `UpgradeButtonUI.cs` - 업그레이드 버튼

### Phase 6: 통합
16. `GameConfig.cs` - 게임 설정 ScriptableObject
17. `GameManager.cs` - 시스템 초기화/연결
18. 6개 업그레이드 데이터 에셋 생성

---

## 핵심 수익 공식

```csharp
// 클릭 수익
(1 + 메뉴추가) × 고급재료 × 크리티컬 × 마케팅 × 트럭

// 자동 수익 (초당)
요리사 × 마케팅 × 트럭
```

---

## 업그레이드 데이터

| ID | 이름 | 비용 | 효과 | 타입 |
|----|------|------|------|------|
| menu_add | 메뉴 추가 | 100/500/2000 | +2/+5/+15 | ClickBase |
| premium_ingredient | 고급 재료 | 150/600/2500 | x1.5/x2/x3 | ClickMultiplier |
| golden_hand | 황금 손 | 200/800/3000 | 10%/20%/30% | Critical |
| chef_hire | 요리사 고용 | 100/500/2000 | +1/+3/+10 | AutoBase |
| marketing | 마케팅 | 200/800/3000 | x1.2/x1.5/x2 | GlobalMultiplier |
| truck_upgrade | 트럭 업그레이드 | 300/1200/5000 | x1.1/x1.25/x1.5 | TruckBonus |

---

## 검증 방법

1. **클릭 테스트**: 화면 터치 → 골드 증가 확인
2. **업그레이드 테스트**: 골드 차감 + 레벨업 + 수익 증가 확인
3. **자동 수익 테스트**: 요리사 고용 후 매초 골드 증가 확인
4. **수익 공식 테스트**: 모든 업그레이드 Lv2 → 클릭 22.5G, 크리티컬 45G, 자동 5.6G/초
