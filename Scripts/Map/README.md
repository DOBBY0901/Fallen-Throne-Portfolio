# Map / Minimap

미니맵에서 적의 월드 위치를 UI 좌표로 변환하고, 적 생성/사망에 맞춰 아이콘을 등록·해제하는 흐름을 보여주는 코드 샘플입니다.

## System Flow

```text
Initial Enemy
   ↓ Inspector List
RegisterEnemy()

Runtime Spawn
EnemySpawnManager
   ↓ Instantiate
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
   ↓ EnemyHealth
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
- Orthographic Camera 크기와 UI 반경으로 월드/UI 스케일 계산
- 미니맵 범위 밖의 적 아이콘 비활성화
- 씬 초기 적과 런타임 Spawn 적을 모두 RegisterEnemy로 등록
- EnemyHealth 사망 시 UnregisterEnemy를 호출해 아이콘 제거

## Excluded

- **MinimapCameraFollow.cs** — 플레이어 위치를 따라가는 Top-down Camera 이동만 담당
- **MinimapPlayerIcon.cs** — 플레이어 Y 회전값을 UI 아이콘 Z 회전으로 변환하는 단순 표시 코드
