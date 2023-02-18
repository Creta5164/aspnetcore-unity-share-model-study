# ASP.NET Core의 EF Core와 Unity 양방향 데이터 처리 스터디

[블로그 글][blog-ko] [문서][doc]

이 저장소는 제 지인과 함께 데이터 관리를 위한 서버 백엔드 스터디를 위해 공부한 내용입니다.

- 서버와 클라이언트가 서로 약속한 데이터 규약 준비 => `DataLib` 프로젝트
- ASP.NET Core에서 EF core 쓰는 법 => `ServerApp` 프로젝트
  - `DataLib`을 활용해 DB 테이블 구축
  - 연습이라 DB를 `sqlite`로 사용했기 때문에 배열을 DB에 넣을 수 없었음
- 유니티(클라이언트)에서 `DataLib`을 가져와 서버와 REST API 통신 => `UnityClient`
  - `DataLib`에서 `dotnet build` 명령을 내리면 유니티 에셋 폴더에 빌드 파일을 복사하도록 지시
  - 서버와 통신하는 것을 확인

---

# ASP.NET Core's EF Core and Unity Bidirectional Data Processing Study

[Blog post (Korean)][blog-ko] [Documentation][doc]

This repository is what I studied for a server backend study for data management with my acquaintances.

- Prepare the data protocols promised by the server and client => `DataLib` project
- How to use EF core in ASP.NET Core => `ServerApp` project
   - Build DB table using `DataLib`
   - I couldn't put the array into the DB because I used the DB as `sqlite` for practice.
- Import `DataLib` from Unity (client) and communicate REST API with server => `UnityClient`
   - Commanding `dotnet build` in `DataLib` tells it to copy build files to the Unity Assets folder.
   - Check communication with the server

[blog-ko]: https://blog.naver.com/hd3306/223020370206
[doc]: https://creta5164.github.io/aspnetcore-unity-share-model-study