# Inventory

아이템 스택, 슬롯 압축, 수량 관리와 이벤트 기반 UI 동기화를 보여주는 코드 샘플입니다.

## System Flow

```text
Single Item
ItemDatabaseSO
      ↓
Inventory.AddItem()

Reward Batch
DropTableSO.Roll()
      ↓
CanAddItems()
      ↓ simulated Slot[]
Inventory.AddItems()
      ↓
    Slot[]
      ↓ OnChanged
InventoryUI
      ↓
InventorySlotUI
```

## Files

### Core
- **Inventory.cs** — 25개 슬롯, 스택 처리, 단일/Batch 추가, 제거, 수량 조회, 슬롯 압축
- **InventoryUI.cs** — Inventory 이벤트 구독, 슬롯 갱신, 선택 아이템 상세 정보 표시

### Support
- **InventorySlotUI.cs** — 슬롯 아이콘과 수량 표시, 클릭 이벤트 전달

## Review Points

- 아이템 추가 전에 전체 수용 가능 공간을 계산해 부분 추가 방지
- Batch 보상은 Slot 배열 복사본으로 먼저 시뮬레이션한 뒤 전체 수용 가능할 때만 실제 반영
- 스택 가능한 아이템은 기존 스택을 먼저 채운 뒤 빈 슬롯 사용
- 제거 전 전체 수량을 검사해 부분 제거 방지
- 빈 슬롯 발생 시 Compact 처리로 슬롯 정렬 유지
- Batch 추가 시 OnChanged를 한 번만 발생시켜 UI 갱신 횟수를 줄임
- UI는 Item ID를 통해 ItemDatabaseSO에서 표시 데이터를 조회
