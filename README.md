# Fallen Throne

> Unity 기반 3인칭 액션 RPG 개인 포트폴리오 프로젝트

**Fallen Throne**은 설원과 동굴, 보스 전투를 중심으로 구성한 3인칭 액션 RPG 프로젝트입니다.  
이 저장소는 **채용 및 포트폴리오 코드 검토를 위한 공개 저장소**이며, 게임 전체 프로젝트 대신 제가 직접 구현한 핵심 C# 코드와 시스템 구조를 선별해 정리합니다.

## Project Overview

| 항목 | 내용 |
| --- | --- |
| Engine | Unity 6.3 |
| Language | C# |
| Role | Client / Gameplay Programmer |
| Project Type | Personal Project |
| Genre | Third-Person Action RPG |

## Start Here

코드 전체를 순서대로 볼 필요 없이 아래 영역부터 확인할 수 있습니다.

- [**Player Combat**](Scripts/Player/README.md) — 3단 콤보, 공격 판정, 체력/사망/리스폰
- [**Enemy AI**](Scripts/Enemy/README.md) — NavMesh 순찰, 시야 탐지, Chase/Attack 상태 전환
- [**Boss System**](Scripts/Boss/README.md) — HP Threshold 패턴, Slam/Rockfall, Flame Phase
- [**Item System**](Scripts/Item/README.md) — ScriptableObject 데이터, ID Database, 사용 효과, Drop Table
- [**Inventory System**](Scripts/Inventory/README.md) — 슬롯, 스택/수량 관리, 이벤트 기반 UI
- [**Equipment System**](Scripts/Equipment/README.md) — 장착/해제, 슬롯별 장비, 스탯 합산
- [**UI System**](Scripts/UI/README.md) — 메뉴 상태 관리, QuickSlot 등록/사용/HUD/쿨타임

## Core Implementation

### Player Combat
- 입력 버퍼와 Animation Event를 사용한 3단 콤보
- `OverlapSphereNonAlloc` 기반 근접 공격 판정
- 장비 스탯을 반영한 데미지/방어 계산
- 사망 → 입력 차단 → 체크포인트 Respawn 흐름

### Enemy AI
- NavMesh 기반 랜덤 순찰
- 거리, 시야각, Raycast 장애물 검사를 조합한 플레이어 탐지
- 피격 시 공격자를 추적 대상으로 전환
- Spawn/Encounter 연출과 전투 AI 활성 시점 분리

### Boss System
- 기본 공격과 특수 패턴 상태 분리
- HP 구간별 일회성 패턴 실행
- Root Motion 공격 전후 NavMeshAgent 동기화
- Slam / Rockfall / Flame Phase 전환

### Item / Inventory / Equipment

```text
ItemDataSO
   ↓
ItemDatabaseSO
   ↓
Inventory ─────→ EquipmentManager
   ↓ OnChanged        ↓ OnChanged
InventoryUI       PlayerStats / UI
```

- ScriptableObject 기반 아이템 데이터와 사용 효과
- ID 기반 Dictionary 캐시로 ItemData 조회
- 스택/슬롯 수용량 검사 후 원자적으로 아이템 추가·제거
- 장착 장비의 EquipmentStat 합산
- 장착 교체 실패 시 Inventory 롤백 처리

### Quick Slot / Menu UI

```text
InventoryUI
   ↓
QuickSlotAssignPopup
   ↓
QuickSlotManager
   ├─ QuickSlotHUD
   └─ QuickSlotGameInput
          ↓
    ItemEffectRunner
          ↓
      Inventory
```

- QuickSlot에는 ItemData 전체가 아닌 Item ID만 저장
- Inventory / QuickSlot 이벤트 기반 HUD 갱신
- 실제 효과 적용 성공 시에만 소비 아이템 차감
- 아이템별 쿨타임 관리 및 Radial UI 표시
- 메뉴 오픈 시 플레이어 입력/커서/HUD/TimeScale 상태 제어

## Selected Code

### Player
- [PlayerCombat.cs](Scripts/Player/PlayerCombat.cs)
- [PlayerAttackHit.cs](Scripts/Player/PlayerAttackHit.cs)
- [PlayerHealth.cs](Scripts/Player/PlayerHealth.cs)
- [PlayerStats.cs](Scripts/Player/PlayerStats.cs)

### Enemy
- [EnemyCombatAI.cs](Scripts/Enemy/EnemyCombatAI.cs)
- [EnemyMove.cs](Scripts/Enemy/EnemyMove.cs)
- [EnemyHealth.cs](Scripts/Enemy/EnemyHealth.cs)
- [WolfEncounterSequence.cs](Scripts/Enemy/WolfEncounterSequence.cs)

### Boss
- [BossAI.cs](Scripts/Boss/BossAI.cs)
- [BossHealth.cs](Scripts/Boss/BossHealth.cs)
- [BossPatternController.cs](Scripts/Boss/BossPatternController.cs)
- [BossPhaseController.cs](Scripts/Boss/BossPhaseController.cs)
- [BossRockFallPattern.cs](Scripts/Boss/BossRockFallPattern.cs)

### Item / Inventory
- [ItemDataSO.cs](Scripts/Item/ItemDataSO.cs)
- [ItemDatabaseSO.cs](Scripts/Item/ItemDatabaseSO.cs)
- [ItemEffectSO.cs](Scripts/Item/ItemEffectSO.cs)
- [DropTableSO.cs](Scripts/Item/DropTableSO.cs)
- [Inventory.cs](Scripts/Inventory/Inventory.cs)
- [InventoryUI.cs](Scripts/Inventory/InventoryUI.cs)

### Equipment / UI
- [EquipmentManager.cs](Scripts/Equipment/EquipmentManager.cs)
- [EquipmentSelectPopup.cs](Scripts/Equipment/EquipmentSelectPopup.cs)
- [MenuManager.cs](Scripts/UI/MenuManager.cs)
- [QuickSlotManager.cs](Scripts/UI/QuickSlotManager.cs)
- [QuickSlotGameInput.cs](Scripts/UI/QuickSlotGameInput.cs)
- [QuickSlotHUD.cs](Scripts/UI/QuickSlotHUD.cs)

## Repository Structure

```text
Fallen-Throne-Portfolio/
├─ README.md
└─ Scripts/
   ├─ Player/       Combat / Health / Stats
   ├─ Enemy/        AI / Spawn / Encounter
   ├─ Boss/         AI / Phase / Pattern / Attack
   ├─ Item/         Data / Database / Effects / Drop
   ├─ Inventory/    Slot / Stack / Inventory UI
   ├─ Equipment/    Equip / Stats / Select UI
   └─ UI/           Menu / QuickSlot / HUD
```

각 폴더의 `README.md`에 시스템 흐름과 코드 리뷰 포인트를 별도로 정리했습니다.

## Repository Notice

이 저장소는 실행 가능한 전체 Unity 프로젝트를 배포하기 위한 저장소가 아닙니다.

대용량 모델, 텍스처, 애니메이션, 오디오 및 외부 에셋은  
**용량 및 라이선스 문제로 포함하지 않습니다.**

또한 포트폴리오 코드 검토와 직접적인 관련이 낮은 단순 Trigger, Hotkey Wrapper, 표시 전용 UI, 설정 화면 등은 일부 제외했습니다.  
핵심 구현 흐름과 코드 설계를 빠르게 확인할 수 있도록 선별한 저장소입니다.

## Status

현재 **Player / Enemy / Boss / Item / Inventory / Equipment / UI** 핵심 코드 정리를 완료했습니다.  
이후 Interaction, Environment 영역도 같은 기준으로 필요한 코드만 선별해 추가할 예정입니다.
