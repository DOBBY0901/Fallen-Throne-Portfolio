# Boss

보스의 기본 전투와 HP 구간 기반 특수 패턴, 2페이즈 전환을 보여주는 코드 샘플입니다.

## Boss Flow

```text
BossAI
  ├─ Chase
  └─ Basic Attack
        ↓
BossHealth
        ↓ HP Threshold
BossPatternController
  ├─ 70% → Slam → Rockfall
  ├─ 50% → Flame Phase
  └─ 30% → Slam → Rockfall
```

특수 패턴 중에는 기본 공격과 이동을 중단하고, 패턴 종료 후 NavMeshAgent 상태를 복구합니다.

## Files

### Core
- **BossAI.cs** — 추적, 기본 공격, 특수 패턴 상태와 Root Motion/NavMesh 동기화
- **BossHealth.cs** — 체력, 무적 상태, 패턴 체크, 사망 및 Boss UI 종료 처리
- **BossPatternController.cs** — HP 70% / 50% / 30% 패턴 관리
- **BossPhaseController.cs** — Roar 이후 화염 페이즈, 보스 머티리얼과 맵 전환

### Attacks
- **BossBasicAttack.cs** — Animation Event 기반 근접 공격, 넉백, 화상 연계
- **BossSlamAttack.cs** — 범위 공격, 페이즈별 VFX/SFX, 넉백/화상
- **BossRockFallPattern.cs** — 경고 지점 생성 후 반복 낙석
- **BossRock.cs** — 낙석 충돌 피해와 경고 오브젝트 정리

## Review Points

- 기본 공격과 특수 패턴을 별도 상태로 분리
- HP Threshold 기반 일회성 패턴 실행
- 치명타로 HP가 0이 된 경우 새 Threshold 패턴을 시작하지 않고 즉시 사망 처리
- 패턴 중 무적/UI/AI 상태를 한 흐름에서 관리
- 사망 시 BossHpUI의 활성 전투 상태까지 종료해 메뉴 복귀 시 UI가 다시 나타나는 문제 방지
- Root Motion 공격 전후 NavMeshAgent 동기화
- Flame Phase 여부에 따라 공격 효과와 낙석 프리팹 변경
