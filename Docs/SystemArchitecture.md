# 푸드트럭 클리커 - 시스템 아키텍처

## 1. 프로젝트 개요

| 항목 | 내용 |
|------|------|
| 장르 | 클리커 + 방치형 타이쿤 |
| 엔진 | Unity 3D |
| 플랫폼 | 모바일 (세로 화면) |
| 네임스페이스 | `FoodTruckClicker` |

---

## 2. 폴더 구조

```
Assets/01.Scripts/
├── Core/               # 게임 초기화 및 설정
│   ├── GameManager.cs
│   └── GameConfig.cs
├── Currency/           # 재화 시스템
│   ├── GoldManager.cs
│   ├── ICurrencyProvider.cs
│   └── ICurrencyModifier.cs
├── Click/              # 클릭 수익 시스템
│   ├── ClickController.cs
│   ├── ClickRevenueCalculator.cs
│   └── IRevenueCalculator.cs
├── AutoIncome/         # 자동 수익 시스템
│   ├── AutoIncomeManager.cs
│   └── IAutoIncomeProvider.cs
├── Upgrade/            # 업그레이드 시스템
│   ├── UpgradeManager.cs
│   ├── UpgradeData.cs
│   ├── UpgradeType.cs
│   └── IUpgradeProvider.cs
├── Menu/               # 메뉴 시스템
│   ├── MenuManager.cs
│   ├── MenuData.cs
│   └── IMenuProvider.cs
├── Events/             # 전역 이벤트
│   └── GameEvents.cs
├── Feedback/           # 시각/촉각 피드백
│   ├── ClickFeedbackController.cs
│   ├── FloatingText.cs
│   ├── FoodPopController.cs
│   ├── FoodPopEffect.cs
│   ├── TruckPunchEffect.cs
│   ├── SparkEffect.cs
│   └── HapticFeedback.cs
└── UI/                 # UI 컴포넌트
    ├── GoldDisplayUI.cs
    ├── AutoIncomeDisplayUI.cs
    └── UpgradeButtonUI.cs
```

---

## 3. 핵심 루프

```
┌─────────────────────────────────────────────────────────────┐
│                       GameManager                            │
│                    (시스템 초기화)                            │
└─────────────────────────────────────────────────────────────┘
                              │
           ┌──────────────────┼──────────────────┐
           ▼                  ▼                  ▼
    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
    │ClickController│   │AutoIncome   │    │UpgradeManager│
    │  (수동 클릭)   │   │Manager      │    │  (업그레이드) │
    └──────┬──────┘    │ (자동 수익)  │    └──────┬──────┘
           │           └──────┬──────┘           │
           ▼                  ▼                  ▼
    ┌─────────────────────────────────────────────────┐
    │                   GoldManager                    │
    │                  (재화 관리)                     │
    └─────────────────────────────────────────────────┘
                              │
                              ▼
    ┌─────────────────────────────────────────────────┐
    │                  GameEvents                      │
    │           (OnRevenueEarned, OnGoldChanged)       │
    └─────────────────────────────────────────────────┘
                              │
           ┌──────────────────┼──────────────────┐
           ▼                  ▼                  ▼
    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
    │ClickFeedback│    │GoldDisplayUI│    │UpgradeButton│
    │ Controller  │    │             │    │     UI      │
    └─────────────┘    └─────────────┘    └─────────────┘
```

---

## 4. 시스템별 상세

### 4.1 GameManager (Core)

**역할:** 모든 시스템 초기화 및 의존성 주입

```csharp
private void InitializeSystems()
{
    // 1. MenuManager 초기화
    _menuManager.Initialize();

    // 2. UpgradeManager 초기화
    _upgradeManager.Initialize(_goldManager, _goldManager, _menuManager, OnFoodTruckUpgraded);

    // 3. ClickRevenueCalculator 생성
    _clickRevenueCalculator = new ClickRevenueCalculator(_upgradeManager, _menuManager);

    // 4. ClickController 초기화
    _clickController.Initialize(_clickRevenueCalculator, _goldManager);

    // 5. AutoIncomeManager 초기화
    _autoIncomeManager.Initialize(_upgradeManager, _goldManager, _menuManager);

    // 6. UI 초기화
    InitializeUI();
}
```

**의존성 그래프:**
```
GameManager
├── GoldManager
├── MenuManager
├── UpgradeManager ← (GoldManager, MenuManager)
├── ClickRevenueCalculator ← (UpgradeManager, MenuManager)
├── ClickController ← (ClickRevenueCalculator, GoldManager)
├── AutoIncomeManager ← (UpgradeManager, GoldManager, MenuManager)
└── UpgradeButtonUI[] ← (UpgradeManager)
```

---

### 4.2 Currency 시스템

**GoldManager** - 골드 재화 관리

| 인터페이스 | 메서드 |
|-----------|--------|
| `ICurrencyProvider` | `CurrentGold`, `HasEnough(amount)` |
| `ICurrencyModifier` | `AddGold(amount)`, `SpendGold(amount)` |

---

### 4.3 Click 시스템

**ClickController** - 클릭 입력 처리
- `IPointerClickHandler` 구현
- 클릭 시 `ClickRevenueCalculator.Calculate()` 호출
- 골드 추가 후 `GameEvents.RaiseRevenueEarned()` 발생

**ClickRevenueCalculator** - 클릭 수익 계산

```
[클릭 수익 공식]
일반:    메뉴가격 × 클릭수익배율
크리티컬: 메뉴가격 × 클릭수익배율 × 메뉴개수

메뉴가격 = 기본가격 × 메뉴가격배율(트럭 레벨)
```

**ClickResult 구조체:**
```csharp
public struct ClickResult
{
    public float Revenue;       // 최종 수익
    public bool IsCritical;     // 크리티컬 여부
    public int MenuCount;       // 메뉴 개수 (크리티컬 시)
    public MenuData SelectedMenu; // 선택된 메뉴
}
```

---

### 4.4 AutoIncome 시스템

**AutoIncomeManager** - 자동 수익 관리

```
[자동 수익 공식]
초당 수익 = 요리사 수 × (평균메뉴가격 × 클릭수익배율) × 요리속도배율
```

- `_incomeInterval` (기본 1초) 마다 수익 발생
- 수익 발생 시 `GameEvents.RaiseRevenueEarned(..., isAuto: true)` 호출

---

### 4.5 Menu 시스템

**MenuData** (ScriptableObject)
| 필드 | 설명 |
|------|------|
| `MenuId` | 고유 ID |
| `DisplayName` | 표시 이름 |
| `BasePrice` | 기본 가격 |
| `UnlockLevel` | 해금 레벨 |
| `Icon` | 아이콘 스프라이트 |

**MenuManager** - 메뉴 관리
| 메서드 | 설명 |
|--------|------|
| `GetRandomMenu()` | 해금된 메뉴 중 랜덤 선택 |
| `GetFinalPrice(menu)` | 기본가격 × 가격배율 |
| `SetUnlockLevel(level)` | 트럭 업그레이드 시 호출 |
| `SetPriceMultiplier(mult)` | 트럭 업그레이드 시 호출 |

**메뉴 목록:**
| 메뉴 | 해금 레벨 | 기본 가격 |
|------|----------|----------|
| 핫도그 | 0 | 10G |
| 타코 | 1 | 15G |
| 버거 | 2 | 30G |
| 피자 | 3 | 50G |

---

### 4.6 Upgrade 시스템

**EUpgradeTargetType** (enum)
```csharp
ClickRevenue,    // 클릭 수익 배율 (Multiplicative)
CriticalChance,  // 크리티컬 확률 (Additive)
CriticalProfit,  // 크리티컬 메뉴 개수 (Additive)
ChefCount,       // 요리사 수 (Additive)
CookingSpeed,    // 요리 속도 배율 (Multiplicative)
FoodTruck        // 메뉴 해금 + 가격 배율 (Multiplicative)
```

**UpgradeData** (ScriptableObject)
| 필드 | 설명 |
|------|------|
| `UpgradeId` | 고유 ID |
| `DisplayName` | 표시 이름 |
| `MaxLevel` | 최대 레벨 (기본 3) |
| `CostsPerLevel` | 레벨별 비용 배열 |
| `ValuesPerLevel` | 레벨별 효과 값 배열 |
| `EModifierType` | Additive / Multiplicative |
| `TargetType` | EUpgradeTargetType |

**UpgradeManager** - 업그레이드 관리
| 메서드 | 설명 |
|--------|------|
| `GetValue(targetType)` | 효과 값 반환 (float) |
| `GetIntValue(targetType)` | 효과 값 반환 (int) |
| `TryPurchase(upgradeId)` | 업그레이드 구매 |
| `CalculateAutoIncome()` | 자동 수익 계산 |

**업그레이드 목록:**

| ID | 이름 | 비용 (Lv1/2/3) | 효과 | 타입 |
|----|------|----------------|------|------|
| `click_revenue` | 클릭 수익 | 100/500/2000 | x1.5/x2/x3 | Multiplicative |
| `critical_chance` | 크리티컬 확률 | 150/600/2500 | 10%/20%/30% | Additive |
| `critical_damage` | 크리티컬 데미지 | 200/800/3000 | 2/3/5개 | Additive |
| `chef_hire` | 요리사 고용 | 100/500/2000 | 1/2/3명 | Additive |
| `cooking_speed` | 요리 속도 | 200/800/3000 | x1.5/x2/x3 | Multiplicative |
| `truck_upgrade` | 트럭 업그레이드 | 300/1200/5000 | x1.5/x2/x3 | Multiplicative |

---

### 4.7 Events 시스템

**GameEvents** - 전역 이벤트 버스

| 이벤트 | 파라미터 | 발생 시점 |
|--------|----------|----------|
| `OnGoldChanged` | `int newGold` | 골드 변경 시 |
| `OnRevenueEarned` | `float revenue, bool isCritical, int menuCount, bool isAuto` | 수익 발생 시 |
| `OnUpgradePurchased` | `string upgradeId, int newLevel` | 업그레이드 구매 시 |
| `OnAutoIncomeChanged` | `float incomePerSecond` | 자동 수익률 변경 시 |

---

### 4.8 Feedback 시스템

**ClickFeedbackController** - 피드백 통합 관리

```csharp
private void HandleRevenueEarned(float revenue, bool isCritical, int menuCount, bool isAuto)
{
    // 공통 피드백
    SpawnFloatingText(revenue, isCritical, menuCount);
    SpawnFoodPop(menuCount);

    // 클릭 전용 피드백
    if (!isAuto)
    {
        TriggerTruckPunch();
        TriggerSpark(isCritical);
        TriggerHaptic(isCritical);
    }
}
```

| 피드백 | 클릭 | 자동 | 설명 |
|--------|:----:|:----:|------|
| FloatingText | O | O | "+{gold}G" 텍스트 |
| FoodPop | O | O | 음식 아이콘 팝업 |
| TruckPunch | O | X | 트럭 흔들림 |
| Spark | O | X | 불꽃 파티클 |
| Haptic | O | X | 진동 피드백 |

**FloatingText** - 골드 표시
- 오브젝트 풀링 사용 (기본 10개)
- 크리티컬 시 "x{menuCount} CRITICAL!\n+{gold}G" 표시
- DOTween으로 위로 이동 + 페이드아웃

**FoodPopController** - 음식 팝업
- `menuCount`만큼 FoodPop 생성
- 랜덤 메뉴 아이콘 표시

---

### 4.9 UI 시스템

**GoldDisplayUI** - 골드 표시
- `OnGoldChanged` 이벤트 구독
- 현재 골드 표시

**AutoIncomeDisplayUI** - 자동 수익 표시
- `OnAutoIncomeChanged` 이벤트 구독
- "+{income}/sec" 표시

**UpgradeButtonUI** - 업그레이드 버튼
- `UpgradeData` 참조
- 레벨, 비용, 효과 표시
- 구매 가능 여부에 따른 색상 변경

---

## 5. 인터페이스 요약

| 인터페이스 | 구현체 | 역할 |
|-----------|--------|------|
| `ICurrencyProvider` | GoldManager | 골드 조회 |
| `ICurrencyModifier` | GoldManager | 골드 변경 |
| `IRevenueCalculator` | ClickRevenueCalculator | 클릭 수익 계산 |
| `IUpgradeProvider` | UpgradeManager | 업그레이드 정보 조회 |
| `IMenuProvider` | MenuManager | 메뉴 정보 조회 |
| `IAutoIncomeProvider` | AutoIncomeManager | 자동 수익 정보 조회 |

---

## 6. 데이터 흐름

### 6.1 클릭 수익 흐름
```
1. ClickController.OnPointerClick()
2. ClickRevenueCalculator.Calculate()
   ├── MenuManager.GetRandomMenu()
   ├── MenuManager.GetFinalPrice()
   ├── UpgradeManager.GetValue(ClickRevenue)
   ├── UpgradeManager.GetValue(CriticalChance)
   └── UpgradeManager.GetIntValue(CriticalProfit)
3. GoldManager.AddGold()
4. GameEvents.RaiseRevenueEarned(revenue, isCritical, menuCount, false)
5. ClickFeedbackController.HandleRevenueEarned()
   ├── SpawnFloatingText()
   ├── SpawnFoodPop()
   ├── TriggerTruckPunch()
   ├── TriggerSpark()
   └── TriggerHaptic()
```

### 6.2 자동 수익 흐름
```
1. AutoIncomeManager.Update() [매 프레임]
2. ProcessAutoIncome() [incomeInterval마다]
3. GoldManager.AddGold()
4. GameEvents.RaiseRevenueEarned(gold, false, 1, true)
5. ClickFeedbackController.HandleRevenueEarned()
   ├── SpawnFloatingText()
   └── SpawnFoodPop()
```

### 6.3 업그레이드 흐름
```
1. UpgradeButtonUI.OnButtonClicked()
2. UpgradeManager.TryPurchase(upgradeId)
3. GoldManager.SpendGold()
4. GameEvents.RaiseUpgradePurchased()
5. 효과 적용:
   ├── ChefCount/CookingSpeed → AutoIncomeManager 재계산
   └── FoodTruck → MenuManager 메뉴 해금 + 가격배율 적용
```

---

## 7. ScriptableObject 에셋

### 7.1 업그레이드 에셋 (6개)
```
Assets/03.ScriptableObjects/
├── ClickRevenue.asset      (신규 필요)
├── CriticalUpgrade.asset   (critical_chance)
├── CriticalDamage.asset    (신규 필요)
├── Hire.asset              (chef_hire)
├── CookingSpeed.asset      (신규 필요)
└── Truck.asset             (truck_upgrade)
```

### 7.2 메뉴 에셋 (4개)
```
Assets/03.ScriptableObjects/Menus/
├── Hotdog.asset    (UnlockLevel: 0, BasePrice: 10)
├── Taco.asset      (UnlockLevel: 1, BasePrice: 15)
├── Burger.asset    (UnlockLevel: 2, BasePrice: 30)
└── Pizza.asset     (UnlockLevel: 3, BasePrice: 50)
```

---

## 8. 씬 계층 구조

```
SampleScene
├── Managers
│   ├── GameManager
│   ├── GoldManager
│   ├── UpgradeManager
│   ├── AutoIncomeManager
│   └── MenuManager
├── Gameplay
│   ├── ClickController (트럭 영역)
│   └── FeedbackController
├── UI
│   ├── Canvas
│   │   ├── GoldDisplay
│   │   ├── AutoIncomeDisplay
│   │   ├── UpgradePanel
│   │   │   ├── ClickRevenueButton
│   │   │   ├── CriticalChanceButton
│   │   │   ├── CriticalDamageButton
│   │   │   ├── ChefHireButton
│   │   │   ├── CookingSpeedButton
│   │   │   └── TruckUpgradeButton
│   │   └── FloatingTextPool
│   └── EventSystem
└── Environment
    └── FoodTruck
```

---

## 9. 확장 가이드

### 새 업그레이드 추가
1. `EUpgradeTargetType`에 새 타입 추가
2. `UpgradeData` 에셋 생성
3. `UpgradeManager.GetDefaultValueForType()` 기본값 설정
4. 필요시 관련 시스템에서 `GetValue()` 호출

### 새 메뉴 추가
1. `MenuData` 에셋 생성
2. `MenuManager._allMenus`에 추가
3. `UnlockLevel` 설정

### 새 피드백 추가
1. 피드백 컴포넌트 생성 (예: `NewEffect.cs`)
2. `ClickFeedbackController`에 참조 추가
3. `HandleRevenueEarned()`에서 호출
