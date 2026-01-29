# 업그레이드 시스템 리팩토링 계획

## 개요
기존 6개 업그레이드 시스템을 새로운 구조로 변경

---

## 새로운 업그레이드 구조

### 클릭 업그레이드 (3종)

| ID | 이름 | 효과 | 비주얼 |
|----|------|------|--------|
| `click_revenue` | 클릭 수익 | 클릭당 골드 배율 증가 | - |
| `critical_chance` | 크리티컬 확률 | 크리티컬 발생 확률 ↑ | 여러 FoodPop 생성 |
| `critical_damage` | 크리티컬 데미지 | 크리티컬 시 메뉴 개수 ↑ | "x3" 플로팅 텍스트 |

### 자동 업그레이드 (3종)

| ID | 이름 | 효과 | 비주얼 |
|----|------|------|--------|
| `chef_hire` | 요리사 고용 | 자동 클릭 +1 | 요리사 캐릭터 추가 |
| `cooking_speed` | 요리 속도 | 자동 클릭 간격 감소 | - |
| `truck_upgrade` | 트럭 업그레이드 | 새 메뉴 해금 (더 비싼 가격) | 트럭 외관 + 메뉴판 |

---

## 수익 공식

### 클릭 수익
```
일반: 메뉴가격 × 클릭수익배율
크리티컬: 메뉴가격 × 클릭수익배율 × 크리티컬데미지(메뉴개수)
```

### 자동 수익 (초당)
```
요리사수 × 클릭수익 × 요리속도배율
```

### 계산 예시
```
메뉴가격: 10G (버거)
클릭수익배율: x1.5
크리티컬데미지: 3개
요리사: 2명
요리속도: x1.5

일반 클릭: 10 × 1.5 = 15G
크리티컬: 10 × 1.5 × 3 = 45G (FoodPop 3개 생성)
자동(초당): 2 × 15 × 1.5 = 45G/초
```

---

## UpgradeTargetType 변경

### Before
```csharp
public enum UpgradeTargetType
{
    ClickBase,          // 삭제
    ClickMultiplier,    // 삭제
    Critical,           // 분리
    AutoBase,           // 변경
    GlobalMultiplier,   // 삭제
    TruckBonus          // 변경
}
```

### After
```csharp
public enum UpgradeTargetType
{
    ClickRevenue,      // 클릭 수익 배율
    CriticalChance,    // 크리티컬 확률
    CriticalDamage,    // 크리티컬 메뉴 개수
    ChefCount,         // 요리사 수 (자동 클릭 수)
    CookingSpeed,      // 요리 속도 배율
    MenuUnlock         // 메뉴 해금 레벨
}
```

---

## 구현 계획

### Phase 1: 메뉴 시스템 (신규)
| 순서 | 파일 | 설명 |
|------|------|------|
| 1-1 | `Menu/MenuData.cs` | 메뉴 ScriptableObject (이름, 가격, 아이콘, 해금레벨) |
| 1-2 | `Menu/IMenuProvider.cs` | 메뉴 조회 인터페이스 |
| 1-3 | `Menu/MenuManager.cs` | 해금 메뉴 관리, 현재 최고 메뉴 조회 |
| 1-4 | MenuData 에셋 생성 | 핫도그(Lv0), 타코(Lv1), 버거(Lv2), 피자(Lv3) |

### Phase 2: 업그레이드 타입 변경
| 순서 | 파일 | 설명 |
|------|------|------|
| 2-1 | `Upgrade/UpgradeType.cs` | enum 전면 교체 |
| 2-2 | `Upgrade/UpgradeData.cs` | 필요시 수정 |
| 2-3 | `Upgrade/UpgradeManager.cs` | GetValue 로직 수정, 메뉴 해금 연동 |
| 2-4 | UpgradeData 에셋 재생성 | 6개 새 업그레이드 데이터 |

### Phase 3: 클릭 시스템 수정
| 순서 | 파일 | 설명 |
|------|------|------|
| 3-1 | `Click/IRevenueCalculator.cs` | ClickResult에 menuCount 필드 추가 |
| 3-2 | `Click/ClickRevenueCalculator.cs` | 새 공식 적용, 메뉴 가격 기반 계산 |
| 3-3 | `Click/ClickController.cs` | 크리티컬 시 다중 FoodPop 트리거 |

### Phase 4: 자동 수익 시스템 수정
| 순서 | 파일 | 설명 |
|------|------|------|
| 4-1 | `AutoIncome/AutoIncomeManager.cs` | 요리사 수 × 클릭 수익 × 속도 공식 |
| 4-2 | `AutoIncome/IAutoIncomeProvider.cs` | 필요시 인터페이스 수정 |

### Phase 5: 피드백 시스템 수정
| 순서 | 파일 | 설명 |
|------|------|------|
| 5-1 | `Feedback/FoodPopController.cs` | 다중 FoodPop 생성 지원 |
| 5-2 | `Feedback/FloatingText.cs` | 크리티컬 개수 표시 ("x3") |
| 5-3 | `Feedback/ClickFeedbackController.cs` | 크리티컬 시 menuCount 전달 |

### Phase 6: UI 수정
| 순서 | 파일 | 설명 |
|------|------|------|
| 6-1 | `UI/UpgradeButtonUI.cs` | 효과 설명 텍스트 변경 |
| 6-2 | `UI/MenuDisplayUI.cs` (신규) | 현재 메뉴 표시 (선택) |

### Phase 7: 비주얼 (선택)
| 순서 | 파일 | 설명 |
|------|------|------|
| 7-1 | `Visual/ChefVisualController.cs` | 요리사 캐릭터 표시/숨김 |
| 7-2 | `Visual/TruckVisualController.cs` | 트럭 외관 변경 |

---

## 폴더 구조 변경

```
Assets/01.Scripts/
├── Core/
├── Currency/
├── Click/
├── AutoIncome/
├── Upgrade/
├── Menu/                 ← 신규
│   ├── MenuData.cs
│   ├── IMenuProvider.cs
│   └── MenuManager.cs
├── Events/
├── Feedback/
├── UI/
└── Visual/               ← 신규 (선택)
    ├── ChefVisualController.cs
    └── TruckVisualController.cs

Assets/03.ScriptableObjects/
├── Upgrades/
│   ├── ClickRevenue.asset
│   ├── CriticalChance.asset
│   ├── CriticalDamage.asset
│   ├── ChefHire.asset
│   ├── CookingSpeed.asset
│   └── TruckUpgrade.asset
└── Menus/
    ├── Hotdog.asset
    ├── Taco.asset
    ├── Burger.asset
    └── Pizza.asset
```

---

## 업그레이드 데이터 (예시)

### 클릭 수익
| Lv | 비용 | 효과 |
|----|------|------|
| 1 | 100 | x1.5 |
| 2 | 500 | x2.0 |
| 3 | 2000 | x3.0 |

### 크리티컬 확률
| Lv | 비용 | 효과 |
|----|------|------|
| 1 | 150 | 10% |
| 2 | 600 | 20% |
| 3 | 2500 | 30% |

### 크리티컬 데미지
| Lv | 비용 | 효과 |
|----|------|------|
| 1 | 200 | 2개 |
| 2 | 800 | 3개 |
| 3 | 3000 | 5개 |

### 요리사 고용
| Lv | 비용 | 효과 |
|----|------|------|
| 1 | 100 | 1명 |
| 2 | 500 | 2명 |
| 3 | 2000 | 3명 |

### 요리 속도
| Lv | 비용 | 효과 |
|----|------|------|
| 1 | 200 | x1.5 |
| 2 | 800 | x2.0 |
| 3 | 3000 | x3.0 |

### 트럭 업그레이드 (메뉴 해금)
| Lv | 비용 | 효과 | 해금 메뉴 |
|----|------|------|----------|
| 1 | 300 | Lv1 | 타코 (15G) |
| 2 | 1200 | Lv2 | 버거 (30G) |
| 3 | 5000 | Lv3 | 피자 (50G) |

### 메뉴 가격
| 메뉴 | 해금 레벨 | 가격 |
|------|----------|------|
| 핫도그 | 0 (기본) | 10G |
| 타코 | 1 | 15G |
| 버거 | 2 | 30G |
| 피자 | 3 | 50G |

---

## 검증 체크리스트

- [ ] 클릭 시 현재 최고 메뉴 가격 기준 수익 발생
- [ ] 크리티컬 시 FoodPop이 크리티컬데미지 개수만큼 생성
- [ ] 크리티컬 시 "x3" 등 개수 플로팅 텍스트 표시
- [ ] 요리사 고용 시 자동 클릭 발생
- [ ] 요리 속도 업그레이드 시 자동 클릭 간격 감소
- [ ] 트럭 업그레이드 시 새 메뉴 해금 및 가격 상승
- [ ] 자동 수익 = 요리사 수 × 클릭 수익 × 속도 배율
