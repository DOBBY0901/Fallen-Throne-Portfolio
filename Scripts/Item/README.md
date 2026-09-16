# Item

ScriptableObject 기반 아이템 데이터, 사용 효과, 드롭 구조를 보여주는 코드 샘플입니다.

## Use Effect Flow

```text
ItemDataSO
  ├─ Category / Grade / Stack
  ├─ Equipment Data
  └─ OnUseEffects
          ↓
    ItemEffectRunner
      1. CanApply 전체 검증
      2. 모든 검증 성공
          ↓
      ItemEffectSO.Apply()
          ↓
HealHpPercentEffectSO
```

## Drop Flow

```text
EnemyHealth
  ↓ Death
EnemyDropToInventory
  ↓
DropTableSO.Roll()
  ↓
Inventory.AddItems()
```

## Files

### Core
- **ItemDataSO.cs** — 아이템 ID, 분류, 등급, 스택, 장비 정보, 사용 효과 데이터
- **ItemDatabaseSO.cs** — ID 기반 ItemData 조회를 위한 Dictionary 캐시
- **ItemEffectSO.cs** — CanApply / Apply로 분리한 사용 효과 확장 계약
- **ItemEffectRunner.cs** — 모든 효과를 먼저 검증한 뒤 실제 효과를 실행
- **HealHpPercentEffectSO.cs** — 최대 HP 비율 기반 회복 효과 구현 예시

### Drop
- **DropTableSO.cs** — 확률과 수량 범위를 사용한 ScriptableObject 드롭 테이블
- **EnemyDropToInventory.cs** — 적 사망 시 Roll 결과를 Inventory Batch로 전달

## Review Points

- 데이터와 런타임 동작을 ScriptableObject로 분리
- 문자열 ID 기반 ItemDatabase와 런타임 Dictionary 캐시
- 추상 ItemEffectSO를 통한 사용 효과 확장
- 여러 효과를 적용하기 전에 전체 CanApply를 검사해 일부 효과만 적용되는 상태 방지
- ItemData 하나에 여러 사용 효과를 조합할 수 있는 구조
- DropTable 데이터를 여러 적/상자에서 재사용
- Auto-loot은 전체 Batch를 수용할 수 있을 때만 지급해 부분 드롭 상태를 만들지 않음
