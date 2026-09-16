# Interaction

플레이어가 월드 오브젝트와 상호작용할 때 **인터페이스 → 상호작용 대상 → UI/게임 시스템**으로 이어지는 흐름을 보여주는 코드 샘플입니다.

## System Flow

```text
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
- **IInteractable.cs** — 상호작용 대상이 공통으로 구현하는 최소 계약
- **ChestInteractable.cs** — 거리 기반 상호작용 등록, UI 프롬프트, 상자 연출, 드롭, 트랩 종료까지 연결

Respawn Statue 구현은 별도 [**Respawn System**](../Respawn/README.md)에 정리했습니다.

## Related Systems
- [**Trap System**](../Trap/README.md) — 상자 보상 획득 후 TrapManager를 통해 FlameTrap 일괄 종료
- [**Respawn System**](../Respawn/README.md) — 같은 IInteractable 구조를 체크포인트 조각상에 재사용

## Review Points

- 상호작용 대상을 `IInteractable` 인터페이스로 추상화
- 월드 오브젝트가 입력을 직접 처리하지 않고 PlayerInteraction에 자신을 등록
- UI 표시 역시 InteractionUIManager에 owner와 target을 전달해 관리
- Chest와 Respawn Statue가 같은 상호작용 계약을 사용하면서 서로 다른 시스템으로 연결
- 상자 오픈 연출과 실제 보상 지급 시점을 Coroutine으로 순서화
- DropTableSO와 Inventory를 재사용해 상자 전용 아이템 로직을 별도로 만들지 않음
- 상호작용 결과가 TrapManager, RespawnManager 같은 다른 월드 시스템으로 이어질 수 있도록 구성
