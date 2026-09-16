# Environment

지역 상태에 따라 **파티클, Fog, 조명, 환경음, 플레이어 이동 속도**를 함께 전환하는 환경 시스템입니다.

## System Flow

```text
EnvironmentZone
      ↓ EnterZone / ExitZone
Active Zone Stack
      ↓
EnvironmentController
   ├─ Particle
   ├─ Fog
   ├─ Lighting
   ├─ Ambient Audio
   └─ Player Move Multiplier

RespawnManager
      ↓
ForceApplyState()
      ↓
EnvironmentController
```

## States

```text
Normal
Cave
StrongBlizzard
Castle
```

## Files

### Core
- **EnvironmentController.cs** — 환경 상태와 활성 Zone 순서를 관리하고 Fog/Lighting을 Coroutine으로 전환

### Support
- **EnvironmentZone.cs** — Trigger 진입/이탈 수를 추적하고 EnvironmentController에 Zone 등록/해제

## Review Points

- 환경 상태 enum 하나로 파티클/조명/Fog/사운드/이동 속도를 동기화
- 겹치는 Zone은 마지막으로 진입한 활성 Zone을 우선 적용하고, 이탈 시 이전 Zone 상태로 복귀
- Player의 여러 Collider가 Trigger를 통과해도 최초 진입/최종 이탈에서만 상태 변경
- Fog와 조명을 Coroutine으로 보간해 자연스럽게 전환
- Lerp 속도가 0 이하인 경우 즉시 적용해 종료되지 않는 Coroutine 방지
- StrongBlizzard에서 별도 이동 속도 배율 적용
- 환경음 처리는 AudioManager에 위임
- Respawn/순간이동 시 기존 Zone 기록을 비우고 ForceApplyState로 저장된 환경을 복구

## Related System
- [**Respawn System**](../Respawn/README.md) — 체크포인트의 EnvironmentState 저장 및 리스폰 직후 재적용
