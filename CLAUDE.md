# FoodTruckCliker - Code Standards

## Project Structure
```
Assets/
├── 00.Scenes/       # Unity 씬 파일
├── 01.Scripts/      # C# 스크립트
├── Polyeler/        # Kitchen Car Pack 에셋
└── Settings/        # 프로젝트 설정
```

## SOLID Principles (Strictly Enforced)

- **S (Single Responsibility)**: 클래스는 단일 책임만 가짐
  - 예: `EnemyGroup`은 그룹 관리, `MonsterController`는 개별 행동 담당
- **O (Open-Closed)**: 수정 없이 새로운 상태/전략으로 확장
- **L (Liskov Substitution)**: 상속된 상태는 `IMonsterState` 계약 준수
- **I (Interface Segregation)**: 작은 인터페이스 사용 (`IAttackable`, `IDamageable`, `IHealthProvider`)
- **D (Dependency Inversion)**: 구체 클래스가 아닌 추상화(인터페이스)에 의존

## Law of Demeter

- **규칙**: 직접적인 친구와만 대화
- **Bad**: `controller.GetGroup().GetSlotManager().RequestSlot()`
- **Good**: `controller.RequestAttackSlot()` (체이닝 캡슐화)

## C# Naming Conventions

```csharp
// PascalCase: 클래스, 메서드, 프로퍼티, public 필드
public class MonsterController
{
    public void RequestAttackSlot() { }
    public Transform PlayerTransform { get; private set; }
}

// camelCase: 지역 변수, 매개변수
void Attack(float damageAmount, Vector3 hitPosition)
{
    float distanceToTarget = Vector3.Distance(transform.position, hitPosition);
}

// _camelCase: private 인스턴스 필드
private NavMeshAgent _navAgent;
private MonsterStateMachine _stateMachine;

// s_camelCase: private static 필드
private static int s_instanceCount = 0;
```

## Code Style

- **Indentation**: 4 spaces
- **Braces**: Allman style (여는 브레이스는 새 줄에)
- **One statement per line**

```csharp
// Good - Allman style
public void DoSomething()
{
    if (condition)
    {
        Execute();
    }
}

// Bad - K&R style
public void DoSomething() {
    if (condition) {
        Execute();
    }
}
```

## Refactoring Requirements

- **Pre-PR**: Gemini 코드 리뷰 피드백 적용
- **Boy Scout Rule**: 코드를 발견했을 때보다 깨끗하게 유지
- 유지보수성에 집중, 한 줄짜리 트릭 지양
