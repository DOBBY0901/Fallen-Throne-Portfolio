# Enemy

일반 적의 순찰, 탐지, 추적, 공격과 인카운터 연출을 정리한 코드 샘플입니다.

## AI Flow

```text
EnemyMove
  ↓ Player detected
Idle → Chase → Attack
       ↑       ↓
       └───────┘
```

플레이어 탐지는 거리와 시야각을 먼저 검사하고, Raycast로 장애물 가림 여부를 확인합니다.

## Files

### Core
- **EnemyMove.cs** — NavMesh 기반 랜덤 순찰과 도착 후 대기
- **EnemyCombatAI.cs** — Idle / Chase / Attack 상태 전환, 시야각·거리·장애물 탐지
- **EnemyHealth.cs** — 피격, 사망, 드롭, AI 정지, 지연 제거
- **EnemyHitReaction.cs** — 피격 Animation / VFX / SFX

### Encounter
- **EnemySpawnManager.cs** — 지정된 Spawn Point에 적 생성 및 생성 목록 관리
- **EnemySpawnSequence.cs** — 등장 연출 동안 AI/NavMesh를 비활성화하고 이후 복구
- **WolfEncounterSequence.cs** — Howl → Rush → ForceChase로 이어지는 전용 인카운터

## Review Points

- NavMesh 순찰과 전투 AI 역할 분리
- 거리 → 시야각 → Raycast 순서의 탐지 조건
- 피격 시 공격자를 새로운 추적 대상으로 설정
- 등장 연출 중 AI와 NavMeshAgent 상태를 명시적으로 제어
- 특정 적만 별도의 Encounter Sequence로 확장
