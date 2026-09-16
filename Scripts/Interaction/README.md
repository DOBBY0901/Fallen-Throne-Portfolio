# Interaction

플레이어 입력과 월드 오브젝트를 `IInteractable` 계약으로 연결하고, 상자와 체크포인트처럼 서로 다른 상호작용 대상을 같은 흐름으로 처리합니다.

## System Flow

```text
StarterAssetsInputs
        ↓
PlayerInteraction
        ↓
   IInteractable
   ├───────────────┐
   ↓               ↓
ChestInteractable  RespawnStatueInteractable
   │               │
   ├─ UI            ├─ UI
   ├─ Drop          ├─ Aura / SFX
   └─ Trap Stop     └─ RespawnManager
```

## Files

### Core
- **PlayerInteraction.cs** — 상호작용 입력, 상호작용 애니메이션, 이동 잠금, 현재 IInteractable 실행
- **IInteractable.cs** — 상호작용 대상이 공통으로 구현하는 최소 계약
- **ChestInteractable.cs** — 거리 기반 등록, World Prompt, 상자 연출, 보상 지급, Trap 종료

Respawn Statue 구현은 별도 [**Respawn System**](../Respawn/README.md)에 정리했습니다.

## Review Points

- 월드 오브젝트가 입력을 직접 읽지 않고 PlayerInteraction에 자신을 등록
- Interaction 시작 시 대상 객체를 캡처해 애니메이션 대기 중 대상 변경의 영향을 줄임
- Coroutine이 중단되는 경우에도 Player 이동 상태를 복구
- UI는 InteractionUIManager에 owner와 target을 전달해 관리
- Chest와 Respawn Statue가 같은 IInteractable 계약을 재사용
- 상자 드롭은 최초 시도에 한 번만 Roll하고, Inventory 전체 수용 공간을 확인한 뒤 오픈
- 공간이 부족하면 보상을 잃지 않고 동일한 Roll 결과로 다시 상호작용 가능

## Related Systems

- [**Inventory**](../Inventory/README.md) — 상자 보상 Batch 수용 가능 여부 검사 및 지급
- [**Trap**](../Trap/README.md) — 보상 획득 후 FlameTrap 일괄 종료
- [**Respawn**](../Respawn/README.md) — 같은 IInteractable 구조를 체크포인트에 재사용
