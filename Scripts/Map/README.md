# Map / Minimap

미니맵에서 적의 월드 위치를 UI 좌표로 변환하고, 적 생성/사망에 맞춰 아이콘을 등록·해제하는 흐름을 보여주는 코드 샘플입니다.

## System Flow

```text
Enemy Spawn
    ↓
RegisterEnemy()
    ↓
MinimapEnemyIconManager
    ↓
Enemy World Position
    ↓ relative to Player
XZ Offset
    ↓ worldToUiScale
RectTransform.anchoredPosition

Enemy Death
    ↓
UnregisterEnemy()
    ↓
Icon Destroy
```

## Files

### Core
- **MinimapEnemyIconManager.cs** — 적 Transform과 UI Icon을 Dictionary로 연결하고, 플레이어 기준 상대 위치를 미니맵 좌표로 변환

## Review Points

- `Transform → RectTransform` Dictionary로 적과 아이콘을 직접 매핑
- 플레이어 기준 XZ 상대 좌표를 미니맵 UI 좌표로 변환
- Orthographic Camera 크기와 UI 반경을 사용해 월드/UI 스케일 계산
- 미니맵 범위 밖의 적 아이콘은 비활성화
- Spawn 시 `RegisterEnemy`, Death 시 `UnregisterEnemy`로 동적 상태 동기화
- EnemyHealth에서 사망 시 등록 해제할 수 있도록 전역 접근 지점을 제공

## Excluded

실제 프로젝트에서 사용되는 보조 스크립트 중 포트폴리오 코드 검토 기준에서 정보량이 낮은 코드는 제외했습니다.

- **MinimapCameraFollow.cs** — 플레이어 위치를 따라가는 Top-down Camera 이동만 담당
- **MinimapPlayerIcon.cs** — 플레이어 Y 회전값을 UI 아이콘 Z 회전으로 변환하는 단순 표시 코드
