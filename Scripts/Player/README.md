# Player

플레이어 전투, 체력, 능력치, 상태이상과 사망/리스폰 흐름을 보여주는 코드 샘플입니다.

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
  └─ HP 0 → Die()

Boss Flame / Flame Trap
        ↓
PlayerStatusEffect.ApplyBurn()
        ↓
Coroutine Damage Tick
```

사망 후 복구는 Respawn 시스템과 연결됩니다.

```text
PlayerHealth.Die()
   ├─ Input Off
   ├─ Death UI
   ├─ Cursor Unlock
   └─ TimeScale 0
          ↓
     Respawn Button
          ↓
PlayerHealth.Respawn()
          ↓
   RespawnManager
   ├─ Position / Rotation
   └─ EnvironmentState
```

## Files

### Core
- **PlayerCombat.cs** — 3단 콤보, 입력 버퍼, Animation Event 기반 콤보 윈도우
- **PlayerAttackHit.cs** — NonAlloc 공격 판정, 다중 Collider 중복 타격 방지
- **PlayerHealth.cs** — 체력, 방어 보정, 사망 UI/입력 제어, RespawnManager 기반 상태 복구
- **PlayerStats.cs** — 장비 스탯을 반영한 공격/방어/이동속도 계산
- **PlayerStatusEffect.cs** — 보스 Flame과 Trap이 공통으로 사용하는 Coroutine 기반 화상 지속 피해

### Support
- **PlayerCombatState.cs** — 전투 상태 진입 및 일정 시간 후 해제
- **PlayerHitReaction.cs** — 피격 Animation / VFX / SFX
- **PlayerKnockback.cs** — 보스 공격에 사용되는 넉백 처리

## Related Systems
- [**Boss System**](../Boss/README.md) — Flame Phase 공격에서 Burn 적용
- [**Trap System**](../Trap/README.md) — FlameDamageArea에서 같은 PlayerStatusEffect 재사용
- [**Respawn System**](../Respawn/README.md) — 체크포인트 위치/환경 상태를 이용한 사망 후 복구

## Review Points

- 입력과 애니메이션 타이밍을 분리한 콤보 구조
- `OverlapSphereNonAlloc`을 사용한 공격 판정
- HP 변경을 이벤트로 외부 UI에 전달
- PlayerStats를 통한 데미지/방어 계산 분리
- Burn 효과를 공격 주체마다 중복 구현하지 않고 PlayerStatusEffect로 통합
- 사망 시 게임 입력/UI/TimeScale 상태를 변경하고 Respawn에서 명시적으로 복구
- PlayerHealth가 체크포인트 데이터를 직접 소유하지 않고 RespawnManager에서 조회
