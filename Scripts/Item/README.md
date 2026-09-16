# Item

ScriptableObject 기반 아이템 데이터, 효과, 드롭 구조를 보여주는 코드 샘플입니다.

## System Flow

```text
ItemDataSO
  ├─ Category / Grade / Stack
  ├─ Equipment Data
  └─ OnUseEffects
          ↓
    ItemEffectRunner
          ↓
      ItemEffectSO
          ↓
HealHpPercentEffectSO
```

드롭 흐름은 다음과 같이 구성했습니다.

```text
EnemyHealth
  ↓ Death
EnemyDropToInventory
  ↓
DropTableSO.Roll()
  ↓
ItemDatabaseSO
  ↓
Inventory.AddItem()
```

## Files

### Core
- **ItemDataSO.cs** — 아이템 ID, 분류, 등급, 스택, 장비 정보, 사용 효과 데이터
- **ItemDatabaseSO.cs** — ID 기반 ItemData 조회를 위한 Dictionary 캐시
- **ItemEffectSO.cs** — 아이템 사용 효과를 확장하기 위한 추상 ScriptableObject
- **ItemEffectRunner.cs** — ItemData에 등록된 효과 목록 실행
- **HealHpPercentEffectSO.cs** — 최대 HP 비율 기반 회복 효과 구현 예시

### Drop
- **DropTableSO.cs** — 확률과 수량 범위를 사용한 ScriptableObject 드롭 테이블
- **EnemyDropToInventory.cs** — 적 사망 시 드롭 결과를 Inventory에 전달

## Review Points

- 데이터와 런타임 동작을 ScriptableObject로 분리
- 문자열 ID 기반 ItemDatabase와 런타임 Dictionary 캐시
- 추상 ItemEffectSO를 통한 사용 효과 확장 구조
- ItemData 하나에 여러 사용 효과를 조합할 수 있는 구성
- DropTable 데이터를 적마다 재사용할 수 있는 구조
- 드롭 결과와 Inventory 저장 로직의 역할 분리
