
<img width="1536" height="1024" alt="ChatGPT Image 2026년 3월 10일 오후 11_26_38" src="https://github.com/user-attachments/assets/ff08703b-30f4-4fcf-bd79-17349c553e6b" />


---

# EzDoDffense

유닛을 배치하고 조합하며 강화해 밀려오는 적을 막는 캐주얼 3D 디펜스 게임

---

## 프로젝트 소개

* 개발 인원 : 개발자 1인, 기획자 1인 (협업 프로젝트)
* 개발 기간 : 2025.09.12 ~ 2026.10.01 (4주)
* 플랫폼 : Android (Google PlayStore)
* 최소 지원 버전 : Android 6.0 Mashmallow (API Level 23)
* 개발 환경 : Unity 6.0
* 주요 라이브러리 : CSVHelper, Newtonsoft.Json

---

## 기술 스택 / 개발 환경

프로젝트 개발 환경 및 사용 기술

| Category        | Technology           |
| --------------- | -------------------- |
| Engine          | Unity                |
| Language        | C#                   |
| IDE             | Visual Studio Code   |
| Version Control | Git / GitHub         |

---

### 게임 설명

* 몰려오는 적들을 잡아 재화를 얻고, 원하는 강화 선택지를 골라 자신의 플레이 전략을 맞춰나갑니다.
* 모든 웨이브를 버티면서 마지막 보스를 잡아 스테이지를 클리어합니다.

---

## 🚀 설치 및 실행

Google PlayStore에서 다운로드할 수 있습니다.

[▶ PlayStore에서 다운로드](https://play.google.com/store/apps/details?id=com.Kyungil.EzDoDefense&pli=1)

---

## 📁 폴더 구조

```
Assets/
├── 📂 Scripts/
│   ├── 📂 Data/  # 데이터 테이블 등
│   ├── 📂 Interfaces/
│   ├── 📂 UI/
│   ├──   ├── GameManager
│   ├──   ├── GameTimer
│   ├──   ├── UIGradeEnforcePanel
│   ├──   ├── UIManager
│   ├──   ├── UIOption
│   ├──   ├── UIPanel
│   ├──   ├── UISkillButton
│   ├──   ├── UISkillInfo
│   ├──   ├── UITitle
│   ├──   ├── UIUnitInfo
│   ├──   ├── UIUpgradePanel
│   ├── AllyUnit
│   ├── AudioManager
│   ├── Character
│   ├── Clickable
│   ├── ClickableComponent
│   ├── EnemySpawner
│   ├── EnemyUnit
│   ├── PlacementManager
│   ├── SkillManager
├── 📂 Prefabs/
│   ├── Allys/
│   ├── Enemies/
│   └── SkillEffects/

```

---

## 주요 기능 및 시스템

🛸 **아군 유닛**
- 클릭 기반 이동 시스템
- N 대 N 슬롯 스왑 구현
- [`AllyUnit.cs`](Assets/Scripts/AllyUnit.cs) / [`Clickable.cs`](Assets/Scripts/Clickable.cs) / [`ClickableComponent.cs`](Assets/Scripts/ClickableComponent.cs)

💀 **적 유닛**
- 웨이브 기반 적 등장 시스템
- 비주얼 모델 교체
- [`EnemyUnit.cs`](Assets/Scripts/EnemyUnit.cs) / [`EnemySpawner.cs`](Assets/Scripts/EnemySpawner.cs)

🌿 **전투 시스템**
- 버프/디버프, 단일 및 광역 공격 스킬 구현
- 파티클/효과음 게임 연출
- [`SkillManager.cs`](Assets/Scripts/SkillManager.cs) / [`AudioManager.cs`](Assets/Scripts/AudioManager.cs)

⚡ **UI / UX**
- 타입별 유닛 랜덤 뽑기
- 유닛 등급 및 타입 강화
- 옵션창 구현
- [`PlacementManager.cs`](Assets/Scripts/PlacementManager.cs) / [`UIOption.cs`](Assets/Scripts/UI/UIOption.cs)

---

## 게임 스크린샷 및 GIF


<table align="center">
<tr>
  
<td align="center"><img width="311" height="671" alt="타이틀" src="https://github.com/user-attachments/assets/1da54126-d4f4-44b4-a31f-1f850da56139" /><br><sub>타이틀</sub></td>
<td align="center"><img width="732" height="331" alt="튜토리얼" src="https://github.com/user-attachments/assets/68ef6404-ae05-4673-9294-8a8ef431efdc" /><br><sub>튜토리얼</sub></td>
</tr>
<tr>
<td align="center"><img width="843" height="475" alt="스킬캡처 (1)" src="https://github.com/user-attachments/assets/503cdf84-854b-446e-9fc4-5527c48c1b49" /><br><sub>인게임 스킬</sub></td>
<td align="center"><img width="845" height="472" alt="보스캡처 (1)" src="https://github.com/user-attachments/assets/0ca8a424-0309-4041-aaad-37797add32ce" /><br><sub>인게임 보스</sub></td>
</tr>
</table>


---
