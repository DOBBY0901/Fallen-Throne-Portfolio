# Fallen Throne

> Unity 기반 3인칭 액션 RPG 개인 포트폴리오 프로젝트

**Fallen Throne**은 설원과 동굴, 보스 전투를 중심으로 구성한 3인칭 액션 RPG 프로젝트입니다.  
이 저장소는 **채용 및 포트폴리오 코드 검토를 위한 공개 저장소**이며, 게임 전체 프로젝트 대신 직접 구현한 핵심 C# 코드와 시스템 구조를 선별해 정리합니다.

## Project Overview

| 항목 | 내용 |
| --- | --- |
| Engine | Unity 6.3 |
| Language | C# |
| Role | Client / Gameplay Programmer |
| Project Type | Personal Project |
| Genre | Third-Person Action RPG |

## Start Here

- [**Player Combat**](Scripts/Player/README.md) — 콤보, 공격 판정, 체력/상태이상, 사망/리스폰
- [**Enemy AI**](Scripts/Enemy/README.md) — NavMesh 순찰, 시야 탐지, Chase/Attack, Spawn/Encounter
- [**Boss System**](Scripts/Boss/README.md) — HP Threshold 패턴, Slam/Rockfall, Flame Phase
- [**Item System**](Scripts/Item/README.md) — ScriptableObject 데이터, 효과 사전 검증, Drop Table
- [**Inventory System**](Scripts/Inventory/README.md) — 슬롯/스택, 원자적 Batch 보상, 이벤트 기반 UI
- [**Equipment System**](Scripts/Equipment/README.md) — 장착/해제, 슬롯별 장비, 스탯 합산
- [**UI System**](Scripts/UI/README.md) — 메뉴, QuickSlot, Interaction UI, 비동기 Loading
- [**Map / Minimap**](Scripts/Map/README.md) — 월드→UI 좌표 변환, Spawn/Death 아이콘 동기화
- [**Interaction System**](Scripts/Interaction/README.md) — PlayerInteraction + IInteractable 기반 상호작용
- [**Environment System**](Scripts/Environment/README.md) — Zone, Particle/Fog/Lighting/Audio/이동속도
- [**Trap System**](Scripts/Trap/README.md) — Emission 경고, 화염 패턴, Burn 상태이상
- [**Respawn System**](Scripts/Respawn/README.md) — 체크포인트 위치/회전/환경 상태 저장 및 복구

## Core Implementation

### Player Combat

```text
Input
  ↓
PlayerCombat
  ↓ Animation Event
PlayerAttackHit
  ↓
EnemyHealth / BossHealth
```

- 입력 버퍼와 Animation Event 기반 3단 콤보
- `OverlapSphereNonAlloc` 공격 판정과 다중 Collider 중복 타격 방지
- PlayerStats 기반 공격/방어 보정
- Burn/Knockback 등 전투 상태 처리
- 사망 시 입력/상태이상/전투 상태 정리 후 Respawn에서 명시적으로 복구

### Enemy / Boss

- EnemyMove와 EnemyCombatAI를 분리한 Patrol → Detect → Chase → Attack 흐름
- 거리 → 시야각 → Raycast 순서의 플레이어 탐지
- 런타임 Spawn 적을 Minimap에 등록하고 사망 시 해제
- Boss 기본 공격과 특수 패턴 상태 분리
- HP 70% / 50% / 30% Threshold 패턴
- 특수 패턴 중 AI/NavMesh/무적 UI 상태 동기화
- 사망 시 Boss UI의 전투 활성 상태까지 종료

### Item / Inventory / Equipment

```text
ItemDataSO
   ├─ ItemEffectSO
   │      ↓ CanApply All
   │   ItemEffectRunner
   │      ↓ Apply All
   │
   ↓
Inventory ─────→ EquipmentManager
   ↓ OnChanged        ↓ OnChanged
InventoryUI       PlayerStats / UI
```

- ScriptableObject 기반 아이템 데이터/사용 효과
- ID 기반 ItemDatabase Dictionary 캐시
- 여러 ItemEffect를 실제 적용하기 전에 전체 사전 검증
- 단일 아이템 및 Drop Batch의 수용 공간을 먼저 검사해 부분 지급 방지
- 장비 교체 실패 시 Inventory 롤백

### Interaction / World Systems

```text
PlayerInteraction
      ↓
 IInteractable
   ├─ ChestInteractable
   │    ├─ DropTable → Inventory
   │    └─ TrapManager
   │
   └─ RespawnStatueInteractable
        └─ RespawnManager
              ↓
        EnvironmentController
```

- 상호작용 입력과 월드 오브젝트 역할 분리
- 상자 보상은 Roll 결과를 유지한 채 Inventory 공간 확보 후 지급
- FlameTrap의 Warning → Danger → Flame 흐름과 PlayerStatusEffect 재사용
- Respawn 위치/회전과 EnvironmentState를 함께 저장
- 겹치는 Environment Zone은 활성 순서를 추적해 이탈 시 이전 상태로 복귀
- Respawn처럼 Trigger를 거치지 않는 이동은 ForceApplyState로 환경 복구

### UI / Loading / Minimap

- Inventory/QuickSlot 변경 이벤트 기반 HUD 갱신
- 소비 효과 성공 시에만 아이템 차감 및 쿨타임 시작
- 메뉴 오픈 시 입력/커서/HUD/TimeScale 상태 관리
- owner 기반 Interaction UI 관리
- AsyncOperation 실제 진행률과 표시 진행률을 분리한 Loading UI
- 적 월드 XZ 상대 위치를 Minimap UI 좌표로 변환

## Selected Code

### Gameplay
- [PlayerCombat.cs](Scripts/Player/PlayerCombat.cs)
- [PlayerAttackHit.cs](Scripts/Player/PlayerAttackHit.cs)
- [PlayerHealth.cs](Scripts/Player/PlayerHealth.cs)
- [EnemyCombatAI.cs](Scripts/Enemy/EnemyCombatAI.cs)
- [BossPatternController.cs](Scripts/Boss/BossPatternController.cs)

### Systems
- [ItemDataSO.cs](Scripts/Item/ItemDataSO.cs)
- [Inventory.cs](Scripts/Inventory/Inventory.cs)
- [EquipmentManager.cs](Scripts/Equipment/EquipmentManager.cs)
- [PlayerInteraction.cs](Scripts/Interaction/PlayerInteraction.cs)
- [EnvironmentController.cs](Scripts/Environment/EnvironmentController.cs)
- [RespawnManager.cs](Scripts/Respawn/RespawnManager.cs)

### UI / World
- [QuickSlotGameInput.cs](Scripts/UI/QuickSlotGameInput.cs)
- [InteractionUIManager.cs](Scripts/UI/InteractionUIManager.cs)
- [LoadingSceneUI.cs](Scripts/UI/LoadingSceneUI.cs)
- [MinimapEnemyIconManager.cs](Scripts/Map/MinimapEnemyIconManager.cs)
- [FlameTrap.cs](Scripts/Trap/FlameTrap.cs)

## Repository Structure

```text
Fallen-Throne-Portfolio/
├─ README.md
└─ Scripts/
   ├─ Player/       Combat / Health / Stats / Status Effect
   ├─ Enemy/        AI / Spawn / Encounter
   ├─ Boss/         AI / Phase / Pattern / Attack
   ├─ Item/         Data / Database / Effects / Drop
   ├─ Inventory/    Slot / Stack / Batch / Inventory UI
   ├─ Equipment/    Equip / Stats / Select UI
   ├─ UI/           Menu / QuickSlot / Interaction UI / Loading
   ├─ Map/          Minimap / Enemy Icon Tracking
   ├─ Interaction/  Player Interaction / Interface / Chest
   ├─ Environment/  State / Zone / Fog / Lighting / Weather
   ├─ Trap/         Warning / Flame / Burn / Trap Control
   └─ Respawn/      Checkpoint / Environment Restore
```

각 폴더의 `README.md`에서 해당 시스템의 흐름과 코드 리뷰 포인트를 확인할 수 있습니다.

## Dependencies / Omitted Runtime Code

이 저장소는 전체 Unity 프로젝트가 아닌 **선별된 코드 포트폴리오**입니다. 따라서 아래 런타임 요소는 코드에서 참조하지만 저장소에는 포함하지 않습니다.

- **Unity Starter Assets** — `StarterAssetsInputs`, `ThirdPersonController`
- **Project Infrastructure** — 범용 오디오 재생을 담당하는 `AudioManager`
- **External VFX Asset** — `INab.VFXAssets.CharacterEffect`
- Scenes, Prefabs, Animator Controllers, Models, Textures, Animations, Audio와 기타 외부 에셋

대용량/외부 에셋과 포트폴리오 검토 가치가 낮은 단순 Trigger, Hotkey Wrapper, 표시 전용 UI, Camera Follow 코드는 의도적으로 제외했습니다.

## Status

**Player / Enemy / Boss / Item / Inventory / Equipment / UI / Map / Interaction / Environment / Trap / Respawn** 핵심 시스템의 코드 선별과 최종 구조 정리를 완료했습니다.
