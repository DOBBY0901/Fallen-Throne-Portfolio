# Player

플레이어 전투, 체력, 능력치, 상태이상 흐름을 보여주는 코드 샘플입니다.

## 핵심 흐름

```text
Input
  ↓
PlayerCombat
  ↓ Animation Event
PlayerAttackHit
  ↓
EnemyHealth / BossHealth
```

피격 흐름은 다음처럼 분리했습니다.

```text
Enemy / Boss Attack
  ↓
PlayerHealth.TakeDamage()
  ├─ PlayerStats 방어 보정
  ├─ PlayerHitReaction
  ├─ PlayerCombatState
  └─ HP 0 → Death / Respawn
```

## Files

### Core
- **PlayerCombat.cs** — 3단 콤보, 입력 버퍼, Animation Event 기반 콤보 윈도우
- **PlayerAttackHit.cs** — NonAlloc 공격 판정, 다중 Collider 중복 타격 방지
- **PlayerHealth.cs** — 체력, 방어 보정, 사망, UI 이벤트, 체크포인트 리스폰
- **PlayerStats.cs** — 장비 스탯을 반영한 공격/방어/이동속도 계산
- **PlayerStatusEffect.cs** — Coroutine 기반 화상 지속 피해

### Support
- **PlayerCombatState.cs** — 전투 상태 진입 및 일정 시간 후 해제
- **PlayerHitReaction.cs** — 피격 Animation / VFX / SFX
- **PlayerKnockback.cs** — 보스 공격에 사용되는 넉백 처리

## Review Points

- 입력과 애니메이션 타이밍을 분리한 콤보 구조
- `OverlapSphereNonAlloc`을 사용한 공격 판정
- HP 변경을 이벤트로 외부 UI에 전달
- PlayerStats를 통한 데미지/방어 계산 분리
- 사망 → 입력 차단 → Respawn까지 이어지는 상태 복구 흐름
