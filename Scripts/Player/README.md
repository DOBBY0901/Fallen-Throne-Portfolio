# Player

플레이어 전투, 체력, 능력치, 상태이상 흐름을 보여주는 코드 샘플입니다.

## Combat Flow

```text
Input
  ↓
PlayerCombat
  ↓ Animation Event
PlayerAttackHit
  ↓
EnemyHealth / BossHealth
```

피격과 상태이상은 다음처럼 분리했습니다.

```text
Enemy / Boss Attack
  ↓
PlayerHealth.TakeDamage()
  ├─ PlayerStats 방어 보정
  ├─ PlayerHitReaction
  ├─ PlayerCombatState
  └─ HP 0 → Death / Respawn

Boss Flame / Flame Trap
        ↓
PlayerStatusEffect.ApplyBurn()
        ↓
Coroutine Damage Tick
```

## Files

### Core
- **PlayerCombat.cs** — 3단 콤보, 입력 버퍼, Animation Event 기반 콤보 윈도우
- **PlayerAttackHit.cs** — NonAlloc 공격 판정, 다중 Collider 중복 타격 방지
- **PlayerHealth.cs** — 체력, 방어 보정, 사망, UI 이벤트, 체크포인트 리스폰
- **PlayerStats.cs** — 장비 스탯을 반영한 공격/방어/이동속도 계산
- **PlayerStatusEffect.cs** — 보스 Flame과 Trap이 공통으로 사용하는 Coroutine 기반 화상 지속 피해

### Support
- **PlayerCombatState.cs** — 전투 상태 진입 및 일정 시간 후 해제
- **PlayerHitReaction.cs** — 피격 Animation / VFX / SFX
- **PlayerKnockback.cs** — 보스 공격에 사용되는 넉백 처리

## Related Systems
- [**Boss System**](../Boss/README.md) — Flame Phase 공격에서 Burn 적용
- [**Trap System**](../Trap/README.md) — FlameDamageArea에서 같은 PlayerStatusEffect를 재사용

## Review Points

- 입력과 애니메이션 타이밍을 분리한 콤보 구조
- `OverlapSphereNonAlloc`을 사용한 공격 판정
- HP 변경을 이벤트로 외부 UI에 전달
- PlayerStats를 통한 데미지/방어 계산 분리
- Burn 효과를 공격 주체마다 중복 구현하지 않고 PlayerStatusEffect로 통합
- 사망 → 입력 차단 → Respawn까지 이어지는 상태 복구 흐름
