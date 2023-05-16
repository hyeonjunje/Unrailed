# Git 전략

## branch 설명

+ main : 본 결과물이 저장될 main branch (팀장만 관리 가능) 
+ develop : 실제로 작업할 공간
+ test : 테스트 관련된 작업할 공간 (필요하면 마음껏 쓰세요)


## Git 협업

### 세팅
1. 팀원들은 팀장의 Repo를 Fork합니다.
2. 팀원들은 각자의 Repo의 develop branch를 만들어 동기화(sync fork)합니다.

### 반복작업
1. 팀원들은 각자의 repo에서 작업한 뒤 팀장의 repo로 PR을 하기 전에 Sync Fork
2. 깃허브 데스크탑 앱에서 Fetch origin
3. Fetch origin하면 Pull 버튼 뜸 Pull 누르기
4. 유니티 들어가서 내가 한 거 잘되는지 다시 확인
5. 커밋 메세지 작성하면 Push 뜸 Push 누르기
6. 다시 홈페이지 들어가서 Pull requests 누르기
7. 내 Develop에서 팀장 Develop으로((경로 확인 잘하기)) Pull requests 보내기
