# Respawn

체크포인트 조각상에서 **위치 + 회전 + 환경 상태**를 저장하고, 사망 후 해당 상태로 플레이어와 월드를 함께 복구하는 리스폰 시스템입니다.

## System Flow

```text
RespawnStatueInteractable
      ↓ Interact
RespawnManager.SetRespawnPoint()
   ├─ Position / Rotation
   ├─ EnvironmentState
   └─ Active Statue Aura

PlayerHealth.Die()
      ↓
Respawn Button
      ↓
RespawnManager.TryGetRespawnPose()
      ↓ valid
PlayerHealth.Respawn()
   ├─ Move Player
   ├─ Restore HP / Input / Animator
   └─ ApplyRespawnEnvironment()
                ↓
        EnvironmentController
```

## Files

### Core
- **RespawnManager.cs** — 현재 체크포인트 Transform과 EnvironmentState 보관, 유효 Respawn Pose 제공, 환경 복구
- **RespawnStatueInteractable.cs** — IInteractable 체크포인트, 거리 기반 UI 등록, 활성 Aura 및 SFX 처리

## Review Points

- 리스폰 위치뿐 아니라 회전값과 환경 상태를 함께 저장
- 새 체크포인트 활성화 시 이전 조각상의 Aura를 해제하고 현재 조각상만 활성화
- PlayerHealth는 리스폰 위치를 직접 소유하지 않고 RespawnManager에서 조회
- 기본/현재 Respawn Transform이 없으면 Vector3.zero로 이동하지 않고 복구를 중단
- CharacterController를 일시 비활성화한 뒤 위치를 변경해 이동 충돌 문제 방지
- Trigger를 통과하지 않는 순간이동 후 저장된 EnvironmentState를 강제로 재적용

## Related Systems

- [**Player**](../Player/README.md) — Death / Respawn 상태 복구
- [**Interaction**](../Interaction/README.md) — 체크포인트 조각상이 IInteractable 구현
- [**Environment**](../Environment/README.md) — 저장된 EnvironmentState 재적용

## External Reference

`RespawnStatueInteractable`의 Aura 연출은 외부 VFX 컴포넌트 `INab.VFXAssets.CharacterEffect`를 참조합니다. 외부 에셋 소스는 이 포트폴리오 저장소에 포함하지 않습니다.
