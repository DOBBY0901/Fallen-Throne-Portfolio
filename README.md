# Fallen Throne

> Unity 기반 3인칭 액션 RPG 개인 포트폴리오 프로젝트

**Fallen Throne**은 설원과 동굴, 보스 전투를 중심으로 구성한 3인칭 액션 RPG 프로젝트입니다.  
이 저장소는 **채용 및 포트폴리오 코드 검토를 위한 공개 저장소**이며, 게임 전체 프로젝트가 아닌 제가 직접 구현한 핵심 C# 코드와 구조를 중심으로 정리합니다.

## Project Overview

| 항목 | 내용 |
| --- | --- |
| Engine | Unity 6.3 |
| Language | C# |
| Role | Client / Gameplay Programmer |
| Project Type | Personal Project |
| Genre | Third-Person Action RPG |

## Implemented Systems

### Gameplay & Combat
- 3인칭 캐릭터 이동 및 애니메이션 연동
- 3단계 근접 콤보 공격
- 피격 / 체력 / 사망 처리
- 화상 등 상태이상 시스템
- 공격 판정 및 전투 흐름 제어

### Enemy & Boss AI
- Patrol → Chase → Attack 상태 기반 적 AI
- 시야각, 거리, 장애물 기반 플레이어 감지
- NavMesh 기반 이동
- 피격 및 사망 처리
- 보스 패턴 및 페이즈 전환
- Ground Slam, Rockfall 등 보스 공격 패턴

### Game Systems
- Inventory / Item / Equipment 시스템
- Quick Slot 및 아이템 사용
- Drop Table 기반 아이템 드롭
- Chest Interaction
- 설원 / 동굴 / 강한 눈보라 환경 전환
- 지역에 따른 이동 속도, 파티클, 사운드 변화

### UI
- Player / Enemy / Boss HP UI
- Inventory 및 Quick Slot UI
- 사망 패널 및 Respawn 흐름
- 게임 상태와 연동되는 UI 갱신

## Repository Structure

아래 구조를 기준으로 포트폴리오용 코드를 순차적으로 정리할 예정입니다.

```text
Fallen-Throne-Portfolio/
├─ README.md
├─ Scripts/
│  ├─ Player/
│  ├─ Combat/
│  ├─ EnemyAI/
│  ├─ Boss/
│  ├─ Inventory/
│  ├─ Environment/
│  └─ UI/
└─ Docs/
   ├─ Screenshots/
   └─ Architecture/
```

## Code Review Guide

코드를 볼 때 다음 영역을 중심으로 확인할 수 있도록 구성합니다.

- **Combat** — 입력, 콤보 진행, 공격 판정
- **Enemy AI** — 감지, 추적, 공격 상태 전환
- **Boss** — 패턴 실행 및 페이즈 관리
- **Inventory** — 아이템 데이터와 인벤토리 상태 관리
- **Environment** — 지역에 따른 환경 상태 전환
- **UI** — 게임 데이터와 UI 동기화

## Repository Notice

이 저장소는 실행 가능한 전체 Unity 프로젝트를 배포하기 위한 저장소가 아닙니다.

대용량 모델, 텍스처, 애니메이션, 오디오 및 외부 에셋은  
**용량 및 라이선스 문제로 포함하지 않습니다.**

대신 제가 직접 작성한 핵심 C# 코드와 시스템 구조를 중심으로 공개하여  
구현 방식과 코드 설계를 확인할 수 있도록 구성합니다.

## Status

현재 포트폴리오용 코드와 문서를 정리하여 순차적으로 업로드하고 있습니다.
