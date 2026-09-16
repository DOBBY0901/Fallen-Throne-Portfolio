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
- [**UI System**](Scripts/UI/README.md) — 메뉴, QuickSlot, 상호작용 프롬프트, 보스 UI, 비동기 로딩
- [**Map / Minimap**](Scripts/Map/README.md) — 적 위치 추적, 월드→UI 좌표 변환, 동적 아이콘 등록/해제
- [**Interaction System**](Scripts/Interaction/README.md) — IInteractable 기반 월드 상호작용, 상자/체크포인트 연계
- [**Environment System**](Scripts/Environment/README.md) — 지역 상태에 따른 파티클/Fog/조명/환경음/이동속도 전환
- [**Trap System**](Scripts/Trap/README.md) — Emission 경고, 반복 화염 패턴, Burn 상태이상
- [**Respawn System**](Scripts/Respawn/README.md) — 체크포인트 위치/회전/환경 상태 저장 및 사망 후 복구

## Core Implementation

### Player Combat
- 입력 버퍼와 Animation Event를 사용한 3단 콤보
- `OverlapSphereNonAlloc` 기반 근접 공격 판정
- 장비 스탯을 반영한 데미지/방어 계산
- 사망 → 입력 차단 → 체크포인트 Respawn 흐름
- 보스/함정에서 전달되는 Burn 상태이상을 PlayerStatusEffect에서 공통 처리

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

### UI / Quick Slot / Loading

```text
InventoryUI → QuickSlotManager → QuickSlotHUD
                    ↓
             QuickSlotGameInput
                    ↓
              ItemEffectRunner

IInteractable → InteractionUIManager
LoadingSceneController → LoadingSceneUI → Async Scene Load
```

- QuickSlot에는 ItemData 전체가 아닌 Item ID만 저장
- Inventory / QuickSlot 이벤트 기반 HUD 갱신
- 실제 효과 적용 성공 시에만 소비 아이템 차감
- 메뉴 오픈 시 플레이어 입력/커서/HUD/TimeScale 상태 제어
- 상호작용 요청자를 owner로 관리하는 World/Screen Prompt UI
- `WorldToScreenPoint` 기반 월드 상호작용 키 표시
- AsyncOperation 실제 진행률과 표시 진행률을 분리한 로딩 화면

### Interaction / Trap

```text
PlayerInteraction
      ↓
 IInteractable
   ├───────────────┐
   ↓               ↓
ChestInteractable  RespawnStatueInteractable
   │               │
   ├─ Drop         └─ RespawnManager
   └─ Trap Stop

FlameTrap
   ↓ Warning / Danger
FlameDamageArea
   ↓
PlayerStatusEffect.ApplyBurn()
```

- Chest와 Respawn Statue가 동일한 IInteractable 계약을 재사용
- 상자 보상은 DropTable / Inventory 시스템과 연결
- FlameTrap은 Idle → Warning → Danger → Flame 순서로 시각적 경고 후 활성화
- 실제 상태이상 적용은 FlameDamageArea로 분리하고 PlayerStatusEffect를 재사용
- 상자 보상 획득 후 TrapManager를 통해 구역 함정을 일괄 종료

### Environment / Respawn

```text
EnvironmentZone
      ↓
EnvironmentController
   ├─ Particle
   ├─ Fog
   ├─ Lighting
   ├─ Ambient Audio
   └─ Player Move Multiplier

RespawnStatueInteractable
      ↓
RespawnManager
   ├─ Position
   ├─ Rotation
   └─ EnvironmentState
      ↓
PlayerHealth.Respawn()
      ↓
EnvironmentController.ForceApplyState()
```

- Normal / Cave / StrongBlizzard / Castle 상태를 enum으로 관리
- 상태 하나로 파티클, Fog, 조명, 환경음, 플레이어 이동속도를 동기화
- Fog와 Lighting을 Coroutine으로 보간해 자연스럽게 전환
- 체크포인트 활성화 시 위치/회전뿐 아니라 EnvironmentState도 저장
- 리스폰 직후 Trigger를 거치지 않아도 저장된 환경 상태를 강제로 복구
- PlayerHealth는 체크포인트 데이터를 직접 소유하지 않고 RespawnManager를 통해 조회

### Map / Minimap
- 적 Transform과 아이콘 RectTransform을 Dictionary로 연결
- 플레이어 기준 상대 XZ 좌표를 UI 좌표로 변환
- Orthographic Camera 크기와 UI 반경을 이용한 스케일 계산
- 범위 밖 적 아이콘 비활성화
- Spawn / Death 흐름과 미니맵 아이콘 상태 동기화

## Selected Code

### Player
- [PlayerCombat.cs](Scripts/Player/PlayerCombat.cs)
- [PlayerAttackHit.cs](Scripts/Player/PlayerAttackHit.cs)
- [PlayerHealth.cs](Scripts/Player/PlayerHealth.cs)
- [PlayerStats.cs](Scripts/Player/PlayerStats.cs)
- [PlayerStatusEffect.cs](Scripts/Player/PlayerStatusEffect.cs)

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

### Equipment / UI / Map
- [EquipmentManager.cs](Scripts/Equipment/EquipmentManager.cs)
- [EquipmentSelectPopup.cs](Scripts/Equipment/EquipmentSelectPopup.cs)
- [MenuManager.cs](Scripts/UI/MenuManager.cs)
- [QuickSlotGameInput.cs](Scripts/UI/QuickSlotGameInput.cs)
- [InteractionUIManager.cs](Scripts/UI/InteractionUIManager.cs)
- [LoadingSceneUI.cs](Scripts/UI/LoadingSceneUI.cs)
- [MinimapEnemyIconManager.cs](Scripts/Map/MinimapEnemyIconManager.cs)

### Interaction / Environment / Trap / Respawn
- [ChestInteractable.cs](Scripts/Interaction/ChestInteractable.cs)
- [EnvironmentController.cs](Scripts/Environment/EnvironmentController.cs)
- [FlameTrap.cs](Scripts/Trap/FlameTrap.cs)
- [RespawnManager.cs](Scripts/Respawn/RespawnManager.cs)
- [RespawnStatueInteractable.cs](Scripts/Respawn/RespawnStatueInteractable.cs)

## Repository Structure

```text
Fallen-Throne-Portfolio/
├─ README.md
└─ Scripts/
   ├─ Player/       Combat / Health / Stats / Status Effect
   ├─ Enemy/        AI / Spawn / Encounter
   ├─ Boss/         AI / Phase / Pattern / Attack
   ├─ Item/         Data / Database / Effects / Drop
   ├─ Inventory/    Slot / Stack / Inventory UI
   ├─ Equipment/    Equip / Stats / Select UI
   ├─ UI/           Menu / QuickSlot / Interaction UI / Loading
   ├─ Map/          Minimap / Enemy Icon Tracking
   ├─ Interaction/  Interface / World Interaction / Chest
   ├─ Environment/  State / Zone / Fog / Lighting / Weather
   ├─ Trap/         Warning / Flame / Burn / Trap Control
   └─ Respawn/      Checkpoint / Environment Restore
```

각 폴더의 `README.md`에 시스템 흐름과 코드 리뷰 포인트를 별도로 정리했습니다.

## Repository Notice

이 저장소는 실행 가능한 전체 Unity 프로젝트를 배포하기 위한 저장소가 아닙니다.

대용량 모델, 텍스처, 애니메이션, 오디오 및 외부 에셋은  
**용량 및 라이선스 문제로 포함하지 않습니다.**

또한 포트폴리오 코드 검토와 직접적인 관련이 낮은 단순 Trigger, Hotkey Wrapper, 표시 전용 UI, 설정 화면, 단순 Camera Follow 코드는 일부 제외했습니다.  
핵심 구현 흐름과 코드 설계를 빠르게 확인할 수 있도록 선별한 저장소입니다.

## Status

현재 **Player / Enemy / Boss / Item / Inventory / Equipment / UI / Map / Interaction / Environment / Trap / Respawn** 핵심 코드 정리를 완료했습니다.
