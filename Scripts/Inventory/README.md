# Inventory

아이템 스택, 슬롯 압축, 수량 관리와 인벤토리 UI 동기화를 보여주는 코드 샘플입니다.

## System Flow

```text
ItemDatabaseSO
      ↓
Inventory.AddItem()
      ↓
 Slot[] Data
      ↓ OnChanged
InventoryUI
      ↓
InventorySlotUI
```

## Files

### Core
- **Inventory.cs** — 25개 슬롯, 스택 처리, 추가/제거, 수량 조회, 슬롯 압축
- **InventoryUI.cs** — Inventory 이벤트 구독, 슬롯 갱신, 선택 아이템 상세 정보 표시

### Support
- **InventorySlotUI.cs** — 슬롯 아이콘과 수량 표시, 클릭 이벤트 전달

## Review Points

- 아이템 추가 전에 전체 수용 가능 공간을 계산해 부분 추가를 방지
- 스택 가능한 아이템은 기존 스택을 먼저 채운 뒤 빈 슬롯 사용
- 제거 전 전체 수량을 검사해 부분 제거를 방지
- 빈 슬롯 발생 시 Compact 처리로 슬롯 정렬 유지
- `OnChanged` 이벤트를 통해 데이터와 UI를 직접 결합하지 않고 갱신
- UI는 Item ID를 통해 ItemDatabaseSO에서 표시 데이터를 조회
