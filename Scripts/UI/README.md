# UI

메뉴와 퀵슬롯을 중심으로, 게임 상태와 UI가 연결되는 흐름을 보여주는 코드 샘플입니다.

## Quick Slot Flow

```text
InventoryUI
   ↓ selected item
QuickSlotAssignPopup
   ↓
QuickSlotManager
   ├─ selection / registered item IDs
   └─ OnChanged
        ↓
QuickSlotHUD

QuickSlotGameInput
   ↓
ItemEffectRunner
   ↓
Inventory.UseItem()
```

## Menu Flow

```text
MenuManager
  ├─ Inventory Panel
  ├─ Equipment Panel
  └─ Settings Panel

Open
  ├─ Player Input Off
  ├─ Cursor Unlock
  ├─ HUD Hide
  └─ TimeScale 0

Close
  └─ Restore Game State
```

## Files

### Core
- **MenuManager.cs** — 메뉴 열기/닫기, 탭 전환, 입력 잠금, 커서 및 TimeScale 제어
- **QuickSlotManager.cs** — 5개 슬롯의 Item ID와 현재 선택 슬롯 관리
- **QuickSlotGameInput.cs** — 마우스 휠 선택, 소비 아이템 사용, 쿨타임과 사용 피드백
- **QuickSlotHUD.cs** — Inventory / QuickSlot 이벤트 기반 아이콘·수량 갱신과 쿨타임 표시
- **QuickSlotAssignPopup.cs** — Inventory에서 선택한 소비 아이템을 1~5번 슬롯에 등록

### Support
- **QuickSlotUI.cs** — 등록 팝업의 개별 슬롯 표시

## Review Points

- QuickSlot에는 ItemData 전체가 아니라 ID만 저장
- Inventory 수량 변경과 QuickSlot 변경을 이벤트로 HUD에 전달
- ItemEffect가 실제 적용된 경우에만 아이템 수량 차감
- 아이템별 쿨타임 시간을 Dictionary로 관리
- 메뉴 활성화 상태에서 게임 입력과 QuickSlot 입력 차단
- UI가 게임 상태를 직접 소유하지 않고 각 시스템의 상태를 조회해 표시
