# Trap

경고 연출 → 화염 활성화 → 상태이상 적용으로 이어지는 반복형 함정 시스템입니다.

## System Flow

```text
FlameTrap
  ↓ idle / warning / danger
Emission Transition
  ↓
Flame Active
  ↓
FlameDamageArea
  ↓
PlayerStatusEffect.ApplyBurn()

ChestInteractable
  ↓ reward obtained
TrapManager.StopAllTraps()
```

## Files

### Core
- **FlameTrap.cs** — 랜덤 대기, Emission 경고 단계, 화염 활성화, 조명/SFX를 Coroutine으로 순서 제어

### Support
- **FlameDamageArea.cs** — 화염 Trigger 진입 시 PlayerStatusEffect에 Burn 적용
- **TrapManager.cs** — 여러 FlameTrap을 일괄 종료하고 ChestInteractable과 연결

## Review Points

- Idle → Warning → Danger → Flame을 Coroutine 하나의 반복 흐름으로 구성
- Emission 색상과 강도를 단계적으로 보간해 피해 발생 전 시각적 경고 제공
- 실제 피해 효과는 FlameTrap에서 분리하고 FlameDamageArea가 상태이상 시스템에 전달
- Burn 구현 자체는 기존 PlayerStatusEffect를 재사용
- 상자 보상 획득 후 TrapManager를 통해 해당 구역의 함정을 일괄 종료
- 런타임 Material 인스턴스는 OnDestroy에서 정리

## Excluded

- **TrapTrigger.cs** — Player 진입 시 Trap GameObject를 활성화하는 단순 Trigger 코드라 포트폴리오에서는 제외
