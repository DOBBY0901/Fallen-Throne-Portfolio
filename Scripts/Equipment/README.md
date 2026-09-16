# Equipment

Inventory와 ItemData를 기반으로 장비 교체와 스탯 합산을 처리하는 코드 샘플입니다.

## System Flow

```text
Inventory
   ↓ equipment item
EquipmentSelectPopup
   ↓
EquipmentManager
   ├─ equipped slot IDs
   ├─ Inventory remove / return
   └─ OnChanged
        ├─ PlayerStats
        └─ EquipmentStatusPanelUI
```

## Files

### Core
- **EquipmentManager.cs** — 장착/해제, 슬롯별 Item ID 관리, 장비 스탯 합산
- **EquipmentTypes.cs** — EquipmentSlot / StatType / EquipmentStat 정의
- **EquipmentSelectPopup.cs** — 현재 슬롯에 맞는 장비 후보를 Inventory에서 필터링

### Support
- **EquipmentPopupSlotUI.cs** — 장비 후보 아이콘과 클릭 이벤트
- **EquipmentStatusPanelUI.cs** — 장착 변경 이벤트에 맞춰 공격/방어/이동속도 갱신

## Review Points

- 실제 장비 데이터는 ItemDataSO에 두고 EquipmentManager는 ID만 보관
- 장착 시 새 아이템 제거와 기존 아이템 반환을 실패 가능한 단계로 처리
- 기존 장비 반환 실패 시 새 장비 제거를 Inventory로 롤백
- EquipmentSlot별 장착 상태를 Dictionary로 관리
- 장착된 ItemData의 EquipmentStat을 합산해 PlayerStats에서 사용
- OnChanged 이벤트를 통해 장비 로직과 UI/PlayerStats를 분리
