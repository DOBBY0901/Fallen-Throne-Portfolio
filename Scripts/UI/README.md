# UI

메뉴, 퀵슬롯, 상호작용 프롬프트, 보스 HP, 로딩 화면처럼 **게임 상태와 직접 연결되는 UI 코드**만 선별해 정리했습니다.

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

## Interaction UI Flow

```text
IInteractable
   ↓
InteractionUIManager
   ├─ World Key HUD
   │    └─ WorldToScreenPoint
   └─ Screen Prompt

Menu Open
   ↓
HideAll()
```

## Loading Flow

```text
LoadingSceneController.LoadScene()
          ↓
      Loading Scene
          ↓
     LoadingSceneUI
          ↓
 SceneManager.LoadSceneAsync()
          ↓
 Progress 0.0 ~ 0.9 → UI 0 ~ 1
          ↓
   Scene Activation
```

## Files

### Core
- **MenuManager.cs** — 메뉴 열기/닫기, 탭 전환, 입력 잠금, 커서 및 TimeScale 제어
- **QuickSlotManager.cs** — 5개 슬롯의 Item ID와 현재 선택 슬롯 관리
- **QuickSlotGameInput.cs** — 마우스 휠 선택, 소비 아이템 사용, 쿨타임과 사용 피드백
- **QuickSlotHUD.cs** — Inventory / QuickSlot 이벤트 기반 아이콘·수량 갱신과 쿨타임 표시
- **QuickSlotAssignPopup.cs** — Inventory에서 선택한 소비 아이템을 1~5번 슬롯에 등록
- **InteractionUIManager.cs** — World Prompt와 Screen Prompt를 소유자 기준으로 관리하고 메뉴 상태와 연동
- **LoadingSceneUI.cs** — 비동기 씬 로딩 진행률을 UI 진행 바로 변환하고 최소 표시 시간 이후 씬 활성화

### Support
- **QuickSlotUI.cs** — 등록 팝업의 개별 슬롯 표시
- **BossHpUI.cs** — 보스 HP, 전투 활성 상태, 무적 표시
- **LoadingSceneController.cs** — 로딩 씬을 경유해 다음 씬 이름 전달

## Review Points

- QuickSlot에는 ItemData 전체가 아니라 ID만 저장
- Inventory 수량 변경과 QuickSlot 변경을 이벤트로 HUD에 전달
- ItemEffect가 실제 적용된 경우에만 아이템 수량 차감
- 아이템별 쿨타임 시간을 Dictionary로 관리
- 메뉴 활성화 상태에서 게임 입력과 QuickSlot 입력 차단
- 상호작용 UI는 요청한 `IInteractable`을 owner로 기억해 다른 오브젝트가 임의로 UI를 끄지 못하도록 처리
- World Prompt는 `WorldToScreenPoint`를 사용해 월드 위치를 화면 좌표로 변환
- Loading UI는 실제 AsyncOperation 진행률과 화면 표시 진행률을 분리해 갑작스러운 전환을 줄임

## Excluded

다음 코드는 실제 프로젝트에는 필요하지만 포트폴리오 코드 검토 기준에서는 정보량이 낮아 제외했습니다.

- **HpBarUI.cs** — PlayerHealth 이벤트를 Image Fill에 반영하는 단순 표시 코드
- **BillboardToCamera.cs** — 카메라 방향으로 UI를 회전시키는 단순 보조 코드
- **TitleMenuUI.cs** — 타이틀 버튼 및 오디오 슬라이더 중심의 일반적인 메뉴 코드
