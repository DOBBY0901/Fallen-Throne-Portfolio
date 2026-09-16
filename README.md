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

- [**Player Combat**](Scripts/Player/README.md) — 3단 콤보, 공격 판정, 체력/사망/리스폰, 능력치, 상태이상
- [**Enemy AI**](Scripts/Enemy/README.md) — NavMesh 순찰, 시야 탐지, Chase/Attack 상태 전환, 인카운터
- [**Boss System**](Scripts/Boss/README.md) — 기본 공격, HP Threshold 패턴, Slam/Rockfall, Flame Phase
- [**Item System**](Scripts/Item/README.md) — ScriptableObject 데이터, ID Database, 사용 효과, Drop Table
- [**Inventory System**](Scripts/Inventory/README.md) — 25 슬롯, 스택/수량 관리, 이벤트 기반 UI 동기화

## Core Implementation

### Player Combat
- 입력 버퍼와 Animation Event를 사용한 3단 콤보
- `OverlapSphereNonAlloc` 기반 근접 공격 판정
- 장비 스탯을 반영한 데미지/방어 계산
- 사망 → 입력 차단 → 체크포인트 Respawn 흐름
- 화상 및 넉백 등 전투 상태 처리

### Enemy AI
- NavMesh 기반 랜덤 순찰
- 거리, 시야각, Raycast 장애물 검사를 조합한 플레이어 탐지
- 피격 시 공격자를 추적 대상으로 전환
- Spawn 연출과 전투 AI 활성 시점 분리
- Wolf 전용 Howl → Rush → ForceChase 인카운터

### Boss System
- 기본 공격과 특수 패턴 상태 분리
- 특수 패턴 중 무적 및 AI 상태 제어
- Root Motion 공격 전후 NavMeshAgent 동기화
- Slam, Rockfall 패턴과 경고 지점 처리
- 2페이즈 진입 시 보스 머티리얼과 전투 맵 전환

### Item & Inventory

```text
ItemDataSO
   ↓
ItemDatabaseSO
   ↓
Inventory
   ↓ OnChanged
InventoryUI
```

- ScriptableObject 기반 아이템 데이터와 사용 효과
- ID 기반 Dictionary 캐시로 ItemData 조회
- 기존 스택 우선 채우기 → 빈 슬롯 사용
- 추가/제거 전 전체 공간 및 수량을 검사해 부분 처리 방지
- 아이템 제거 후 슬롯 Compact
- `OnChanged` 이벤트 기반 UI 갱신
- 선택 슬롯의 ID로 ItemDatabase에서 상세 정보 조회

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

## Repository Structure

```text
Fallen-Throne-Portfolio/
├─ README.md
└─ Scripts/
   ├─ Player/
   │  ├─ README.md
   │  └─ Combat / Health / Stats / Support
   ├─ Enemy/
   │  ├─ README.md
   │  └─ AI / Spawn / Encounter
   ├─ Boss/
   │  ├─ README.md
   │  └─ AI / Phase / Pattern / Attack
   ├─ Item/
   │  ├─ README.md
   │  └─ Data / Database / Effects / Drop
   └─ Inventory/
      ├─ README.md
      └─ Inventory / UI / Slot UI
```

## Repository Notice

이 저장소는 실행 가능한 전체 Unity 프로젝트를 배포하기 위한 저장소가 아닙니다.

대용량 모델, 텍스처, 애니메이션, 오디오 및 외부 에셋은  
**용량 및 라이선스 문제로 포함하지 않습니다.**

또한 포트폴리오 코드 검토와 직접적인 관련이 낮은 단순 Trigger, UI 표시 전용 코드, 연출 전용 보조 스크립트는 일부 제외했습니다.  
핵심 구현 흐름과 코드 설계를 빠르게 확인할 수 있도록 선별한 저장소입니다.

## Status

현재 **Player / Enemy / Boss / Item / Inventory** 핵심 코드 정리를 완료했습니다.  
이후 Interaction, Environment, UI 영역도 같은 기준으로 필요한 코드만 선별해 추가할 예정입니다.
