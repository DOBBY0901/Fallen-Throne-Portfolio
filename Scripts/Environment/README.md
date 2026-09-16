# Environment

지역 상태에 따라 **파티클, Fog, 조명, 환경음, 플레이어 이동 속도**를 함께 전환하는 환경 시스템 코드입니다.

## System Flow

```text
EnvironmentZone
      ↓
SetEnvironmentState()
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

예를 들어 StrongBlizzard 상태에서는 강한 눈보라 파티클과 높은 Fog 밀도를 적용하고, 플레이어 이동 속도 배율을 낮춥니다.

## Files

### Core
- **EnvironmentController.cs** — 환경 상태를 기준으로 여러 게임 시스템의 설정을 한 번에 적용하고 Fog/Lighting을 Coroutine으로 보간

### Support
- **EnvironmentZone.cs** — Trigger 진입/이탈 시 EnvironmentState 전환

## Related System
- [**Respawn System**](../Respawn/README.md) — 체크포인트 활성화 시 현재 지역의 EnvironmentState를 저장하고 리스폰 직후 재적용

## Review Points

- 환경 상태를 enum 하나로 정의해 파티클/조명/Fog/사운드/이동 속도를 같은 상태 기준으로 동기화
- Fog와 조명을 즉시 변경하지 않고 Coroutine 보간으로 자연스럽게 전환
- StrongBlizzard에서 이동 속도 배율을 별도로 적용
- 환경음 처리는 AudioManager에 위임해 EnvironmentController가 재생 세부 구현을 직접 소유하지 않음
- `ForceApplyState`를 제공해 순간이동/리스폰처럼 Trigger를 거치지 않는 이동에서도 상태를 재적용
