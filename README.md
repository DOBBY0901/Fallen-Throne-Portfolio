# Fallen Throne

> Unity 기반 3인칭 액션 RPG 개인 포트폴리오 프로젝트

**Fallen Throne**은 설원과 동굴, 보스 전투를 중심으로 구성한 3인칭 액션 RPG 프로젝트입니다.  
이 저장소는 **채용 및 포트폴리오 코드 검토를 위한 공개 저장소**이며, 게임 전체 프로젝트가 아닌 제가 직접 구현한 핵심 C# 코드와 구조를 중심으로 정리합니다.

## Project Overview

| 항목 | 내용 |
| --- | --- |
| Engine | Unity 6.3 |
| Language | C# |
| Role | Client / Gameplay Programmer |
| Project Type | Personal Project |
| Genre | Third-Person Action RPG |

## Implemented Systems

### Gameplay & Combat
- 3인칭 캐릭터 이동 및 애니메이션 연동
- 3단계 근접 콤보 공격
- 피격 / 체력 / 사망 처리
- 화상 등 상태이상 시스템
- 공격 판정 및 전투 흐름 제어

### Enemy & Boss AI
- Patrol → Chase → Attack 상태 기반 적 AI
- 시야각, 거리, 장애물 기반 플레이어 감지
- NavMesh 기반 이동
- 보스 기본 공격 / 특수 패턴 상태 제어
- HP 구간 기반 패턴 전환
- Ground Slam / Rockfall / Flame Phase 패턴
- Root Motion과 NavMeshAgent 상태 동기화

### Game Systems
- Inventory / Item / Equipment 시스템
- Quick Slot 및 아이템 사용
- Drop Table 기반 아이템 드롭
- Chest Interaction
- 설원 / 동굴 / 강한 눈보라 환경 전환
- 지역에 따른 이동 속도, 파티클, 사운드 변화

### UI
- Player / Enemy / Boss HP UI
- Inventory 및 Quick Slot UI
- 사망 패널 및 Respawn 흐름
- 게임 상태와 연동되는 UI 갱신

## Current Code Samples

### Player

- [PlayerCombat.cs](Scripts/Player/PlayerCombat.cs) — 콤보 입력 버퍼, Animation Event 기반 콤보 윈도우, 공격 상태 제어
- [PlayerAttackHit.cs](Scripts/Player/PlayerAttackHit.cs) — NonAlloc 공격 판정, 중복 타격 방지, 일반 적/보스 피해 처리
- [PlayerCombatState.cs](Scripts/Player/PlayerCombatState.cs) — 전투 상태 진입 및 자동 해제
- [PlayerHealth.cs](Scripts/Player/PlayerHealth.cs) — 체력, 방어 보정, 사망, UI, 체크포인트 리스폰 흐름
- [PlayerStats.cs](Scripts/Player/PlayerStats.cs) — 장비 스탯 반영 및 최종 능력치 계산
- [PlayerStatusEffect.cs](Scripts/Player/PlayerStatusEffect.cs) — Coroutine 기반 화상 지속 피해

### Enemy

- [EnemyMove.cs](Scripts/Enemy/EnemyMove.cs) — NavMesh 기반 순찰, 랜덤 목적지 선택, 도착 후 대기
- [EnemyCombatAI.cs](Scripts/Enemy/EnemyCombatAI.cs) — 시야각/거리/장애물 탐지, Idle → Chase → Attack 상태 전환, 공격 판정
- [EnemyHealth.cs](Scripts/Enemy/EnemyHealth.cs) — 체력, 피격, 사망, 드롭, 미니맵 등록 해제, 지연 제거
- [EnemyHitReaction.cs](Scripts/Enemy/EnemyHitReaction.cs) — 피격 애니메이션, VFX, SFX 피드백
- [EnemyHpBarUI.cs](Scripts/Enemy/EnemyHpBarUI.cs) — 이벤트 기반 적 체력 UI 동기화
- [EnemySpawnManager.cs](Scripts/Enemy/EnemySpawnManager.cs) — 지정 Spawn Point 기반 적 생성 및 생성 목록 관리
- [EnemySpawnTrigger.cs](Scripts/Enemy/EnemySpawnTrigger.cs) — 플레이어 진입 기반 1회성 적 생성 트리거
- [EnemySpawnSequence.cs](Scripts/Enemy/EnemySpawnSequence.cs) — 등장 연출 중 AI/NavMesh 비활성화 후 전투 상태 복귀
- [WolfEncounterSequence.cs](Scripts/Enemy/WolfEncounterSequence.cs) — 하울링 → 돌진 → 강제 추적으로 이어지는 인카운터 연출
- [WolfEncounterTrigger.cs](Scripts/Enemy/WolfEncounterTrigger.cs) — 플레이어 진입 시 늑대 인카운터 시작

### Boss

- [BossAI.cs](Scripts/Boss/BossAI.cs) — 추적, 기본 공격, 특수 패턴 상태 제어와 Root Motion/NavMesh 동기화
- [BossBasicAttack.cs](Scripts/Boss/BossBasicAttack.cs) — Animation Event 기반 근접 공격 판정, 넉백, 2페이즈 화상 연계
- [BossHealth.cs](Scripts/Boss/BossHealth.cs) — 보스 체력, 무적 상태, 패턴 체크, 사망 처리
- [BossPatternController.cs](Scripts/Boss/BossPatternController.cs) — HP 70%/50%/30% 구간 기반 패턴 큐와 무적 상태 관리
- [BossPhaseController.cs](Scripts/Boss/BossPhaseController.cs) — Roar 연출 이후 화염 페이즈 및 맵/머티리얼 전환
- [BossSlamAttack.cs](Scripts/Boss/BossSlamAttack.cs) — Slam 범위 공격, 페이즈별 VFX/SFX, 넉백/화상 처리
- [BossRockFallPattern.cs](Scripts/Boss/BossRockFallPattern.cs) — 경고 지점 생성 후 랜덤 낙석을 반복하는 Coroutine 패턴
- [BossRock.cs](Scripts/Boss/BossRock.cs) — 낙석 충돌 피해, 넉백, 화상, 경고 오브젝트 정리
- [BossRoomFlameFloor.cs](Scripts/Boss/BossRoomFlameFloor.cs) — 화염 지형 진입 시 상태이상 적용
- [FogWallInteractable.cs](Scripts/Boss/FogWallInteractable.cs) — 보스룸 진입, 퇴로 차단, HP UI 표시, 전투 시작
- [RiseFromGround.cs](Scripts/Boss/RiseFromGround.cs) — 흔들림 후 지면에서 상승하는 보스 등장 연출
- [WarningCirclePulse.cs](Scripts/Boss/WarningCirclePulse.cs) — 낙석 경고 원의 Emission Pulse 연출

## Repository Structure

```text
Fallen-Throne-Portfolio/
├─ README.md
├─ Scripts/
│  ├─ Player/
│  ├─ Enemy/
│  ├─ Boss/
│  │  ├─ BossAI.cs
│  │  ├─ BossBasicAttack.cs
│  │  ├─ BossHealth.cs
│  │  ├─ BossPatternController.cs
│  │  ├─ BossPhaseController.cs
│  │  ├─ BossSlamAttack.cs
│  │  ├─ BossRockFallPattern.cs
│  │  ├─ BossRock.cs
│  │  ├─ BossRoomFlameFloor.cs
│  │  ├─ FogWallInteractable.cs
│  │  ├─ RiseFromGround.cs
│  │  └─ WarningCirclePulse.cs
│  ├─ Item/
│  ├─ Interaction/
│  ├─ Map/
│  ├─ Trap/
│  └─ UI/
└─ Docs/
   ├─ Screenshots/
   └─ Architecture/
```

## Code Review Guide

코드를 볼 때 다음 영역을 중심으로 확인할 수 있도록 구성합니다.

- **Combat** — 입력, 콤보 진행, 공격 판정
- **Enemy AI** — 감지, 추적, 공격 상태 전환
- **Enemy Encounter** — Spawn, 등장 연출, 강제 추적 전환
- **Boss AI** — 기본 공격과 특수 패턴 상태 분리, Root Motion 처리
- **Boss Pattern** — HP Threshold, Slam → Rockfall, Flame Phase 전환
- **Inventory** — 아이템 데이터와 인벤토리 상태 관리
- **Environment** — 지역에 따른 환경 상태 전환
- **UI** — 게임 데이터와 UI 동기화

## Repository Notice

이 저장소는 실행 가능한 전체 Unity 프로젝트를 배포하기 위한 저장소가 아닙니다.

대용량 모델, 텍스처, 애니메이션, 오디오 및 외부 에셋은  
**용량 및 라이선스 문제로 포함하지 않습니다.**

대신 제가 직접 작성한 핵심 C# 코드와 시스템 구조를 중심으로 공개하여  
구현 방식과 코드 설계를 확인할 수 있도록 구성합니다.

## Status

현재 Player, Enemy, Boss 영역의 핵심 코드 정리를 완료했으며, Inventory, Interaction, Environment, UI 코드를 순차적으로 추가할 예정입니다.
