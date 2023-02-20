using System.Collections;
using UnityEngine;
using DataLib;
using UnityEngine.Networking;
using System;
using Newtonsoft.Json;
using ClientLogic.DataLibExtensions;

namespace ClientLogic {
    public class Client : MonoBehaviour {
        
        /// <summary>
        /// <c>ServerApp</c>의 주소<para/>
        /// Address of <c>ServerApp</c> 
        /// </summary>
#if UNITY_EDITOR
        public string serverUrl => "http://localhost:5171";
#else
        public string serverUrl => "https://my.live.server:port";
#endif
        
        private Coroutine currentRequest;
        
        /// <summary>
        /// <see cref="GetUser_WithExtension"/> 또는 <see cref="GetUser_WithWrapperType"/> 에서 <c>name</c>에 사용 할 정보<para/>
        /// Information to use for <c>name</c> in <see cref="GetUser_WithExtension"/> or <see cref="GetUser_WithWrapperType"/>
        /// </summary>
        public string targetUserName;
        
        /// <summary>
        /// <see cref="RegisterUser"/> 에서 <c>name</c>에 사용 할 정보<para/>
        /// Information to use for <c>name</c> in <see cref="RegisterUser"/>
        /// </summary>
        public string newUserName;
        
        /// <summary>
        /// <see cref="RegisterUser"/> 에서 <c>level</c>에 사용 할 정보<para/>
        /// Information to use for <c>level</c> in <see cref="RegisterUser"/>
        /// </summary>
        public int newUserLevel;
        
        /// <summary>
        /// <see cref="RegisterUser"/> 에서 <c>itemCount</c>에 사용 할 정보<para/>
        /// Information to use for <c>itemCount</c> in <see cref="RegisterUser"/>
        /// </summary>
        [Range(0, 20)]
        public int newUserRandomItemCount = 10;
        
        /// <summary>
        /// 요청을 처리하고 있는 지 여부입니다.<para/>
        /// Whether the request is being processed.
        /// </summary>
        /// <returns>
        /// 요청 처리 중인 지 여부<para/>
        /// Whether the request is being processed
        /// </returns>
        private bool IsRequestRunning()
            => currentRequest != null;
        
        /// <summary>
        /// 지정한 url에 <c>GET</c> 요청을 보냅니다.<para/>
        /// Sends a <c>GET</c> request to the specified url.
        /// </summary>
        /// <param name="url">
        /// GET 요청을 보낼 주소<para/>
        /// Address to send GET requests to
        /// </param>
        /// <param name="onSuccess">
        /// 처리 성공 시 호출할 메서드<para/>
        /// Method to be called on successful processing
        /// </param>
        /// <param name="onError">
        /// 처리 실패 시 호출할 메서드<para/>
        /// Method to be called in case of processing failure
        /// </param>
        /// <typeparam name="T">
        /// 서버 응답으로 약속한 데이터 타입 (서버 앱과 <see cref="DataLib"/> 어셈블리 하위 유형만 약속함)<para/>
        /// Data types promised in server response (promised only by server app and <see cref="DataLib"/> assembly subtype)
        /// </typeparam>
        /// <returns></returns>
        private IEnumerator GetRequestAsync<T>(string url, Action<T> onSuccess, Action<T> onError = null) {
            
            Debug.Log($"{url} 에서 GET 응답을 기다리는 중...");
            Debug.Log($"Waiting for GET response from {url}...");
            
            using (var request = UnityWebRequest.Get(url)) {
                
                yield return request.SendWebRequest();
                
                currentRequest = null;
                
                switch (request.result) {
                    
                    default:
                        Debug.LogError($"{url} 에서 오류를 응답했습니다 : {request.result}");
                        Debug.LogError($"{url} responses with error : {request.result}");
                        
                        if (string.IsNullOrEmpty(request.downloadHandler.text))
                            yield break;
                        
                        try {
                            
                            T resultData = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
                            onError?.Invoke(resultData);
                            
                        } catch (Exception ex) {
                            
                            Debug.LogError($"서버에서 응답한 내용이 약속한 타입 {typeof(T).Name}으로 변환할 수 없는 결과를 반환했습니다.");
                            Debug.LogError($"응답 : {request.downloadHandler.text}");
                            Debug.LogError($"The response from the server returned a result that could not be converted to the promised type {typeof(T).Name}.");
                            Debug.LogError($"Response : {request.downloadHandler.text}");
                            Debug.LogError(ex.Message);
                            Debug.LogError(ex.StackTrace);
                        }
                        
                        break;
                    
                    case UnityWebRequest.Result.Success:
                        
                        try {
                            
                            T resultData = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
                            onSuccess?.Invoke(resultData);
                            
                        } catch (Exception ex) {
                            
                            Debug.LogError($"서버에서 응답한 내용이 약속한 타입 {typeof(T).Name}으로 변환할 수 없는 결과를 반환했습니다.");
                            Debug.LogError($"응답 : {request.downloadHandler.text}");
                            Debug.LogError($"The response from the server returned a result that could not be converted to the promised type {typeof(T).Name}.");
                            Debug.LogError($"Response : {request.downloadHandler.text}");
                            Debug.LogError(ex.Message);
                            Debug.LogError(ex.StackTrace);
                        }
                        
                        break;
                }
            }
        }
        
        /// <summary>
        /// 모든 사용자 정보를 불러온 후, 사용자 정보와 확장 메서드를 통해 사용자의 인벤토리 데이터까지 표시합니다.<para/>
        /// After fetching all user information, display the user information and even the user's inventory data through extension methods.
        /// </summary>
        [TriInspector.Button]
        void GetAllUsers_WithExtension() {
            
            if (IsRequestRunning())
                return;
            
            Debug.Log($"[{nameof(GetAllUsers_WithExtension)}] 모든 사용자 정보를 불러온 후, 사용자 정보와 확장 메서드를 통해 사용자의 인벤토리 데이터까지 표시합니다.");
            Debug.Log($"[{nameof(GetAllUsers_WithExtension)}] After fetching all user information, display the user information and even the user's inventory data through extension methods.");
            
            currentRequest = StartCoroutine(GetRequestAsync<UserData[]>(serverUrl, ShowAllUsersWithExtension));
        }
        
        /// <summary>
        /// 특정한 닉네임(<see cref="targetUserName"/>)을 가진 사용자의 정보를 불러온 후, 사용자 정보와 확장 메서드를 통해 사용자의 인벤토리 데이터까지 표시합니다.
        /// After fetching the information of a user with a specific nickname (<see cref="targetUserName"/>), display the user information and even the user's inventory data through extension methods.
        /// </summary>
        [TriInspector.Button]
        void GetUser_WithExtension() {
            
            if (IsRequestRunning())
                return;
            
            Debug.Log($"[{nameof(GetUser_WithExtension)}] 특정한 닉네임({nameof(targetUserName)}:{targetUserName})을 가진 사용자의 정보를 불러온 후, 사용자 정보와 확장 메서드를 통해 사용자의 인벤토리 데이터까지 표시합니다.");
            Debug.Log($"[{nameof(GetUser_WithExtension)}] After fetching the information of a user with a specific nickname ({nameof(targetUserName)}:{targetUserName}), display the user information and even the user's inventory data through extension methods.");
            
            currentRequest = StartCoroutine(GetRequestAsync<UserData>($"{serverUrl}/name={targetUserName}", ShowUserInfoWithExtension));
        }
        
        /// <summary>
        /// 모든 사용자 정보를 불러온 후, 사용자 정보와 <see cref="InventoryClient"/> 타입을 활용해 사용자의 인벤토리 데이터까지 표시합니다.<para/>
        /// After fetching all user information, display the user information and using <see cref="InventoryClient"/> type to display the user's inventory data.
        /// </summary>
        [TriInspector.Button]
        void GetAllUsers_WithWrapperType() {
            
            if (IsRequestRunning())
                return;
            
            Debug.Log($"[{nameof(GetAllUsers_WithWrapperType)}] 모든 사용자 정보를 불러온 후, 사용자 정보와 {nameof(InventoryClient)} 타입을 활용해 사용자의 인벤토리 데이터까지 표시합니다.");
            Debug.Log($"[{nameof(GetAllUsers_WithWrapperType)}] After fetching all user information, display the user information and using {nameof(InventoryClient)} type to display the user's inventory data.");
            
            currentRequest = StartCoroutine(GetRequestAsync<UserData[]>(serverUrl, ShowAllUsersWithWrapperType));
        }
        
        /// <summary>
        /// 특정한 닉네임(<see cref="targetUserName"/>)을 가진 사용자의 정보를 불러온 후, 사용자 정보와 <see cref="InventoryClient"/> 타입을 활용해 사용자의 인벤토리 데이터까지 표시합니다.<para/>
        /// After fetching the information of a user with a specific nickname (<see cref="targetUserName"/>),  display the user information and using <see cref="InventoryClient"/> type to display the user's inventory data.
        /// </summary>
        [TriInspector.Button]
        void GetUser_WithWrapperType() {
            
            if (IsRequestRunning())
                return;
            
            Debug.Log($"[{nameof(GetUser_WithWrapperType)}] 특정한 닉네임({nameof(targetUserName)}:{targetUserName})을 가진 사용자의 정보를 불러온 후, 사용자 정보와 {nameof(InventoryClient)} 타입을 활용해 사용자의 인벤토리 데이터까지 표시합니다.");
            Debug.Log($"[{nameof(GetUser_WithWrapperType)}] After fetching the information of a user with a specific nickname ({nameof(targetUserName)}:{targetUserName}),  display the user information and using {nameof(InventoryClient)} type to display the user's inventory data.");
            
            currentRequest = StartCoroutine(GetRequestAsync<UserData>($"{serverUrl}/name={targetUserName}", ShowUserInfoWithWrapperType));
        }
        
        /// <summary>
        /// [name = <see cref="newUserName"/>, level = <see cref="newUserLevel"/>, itemCount = <see cref="newUserRandomItemCount"/>] 정보를 서버에 넘겨주어 사용자를 등록합니다.
        /// Register the user by passing the information [name = <see cref="newUserName"/>, level = <see cref="newUserLevel"/>, itemCount = <see cref="newUserRandomItemCount"/>] to the server.
        /// </summary>
        /// <remarks>
        /// 서버에 정보를 넘겨주는 경우에는 반드시 <c>POST</c>로 요청하세요.<para/>
        /// 헤더에는 아래와 같은 값이 들어가며, 서버 어플리케이션에서 구현한 요구사항에 따라 헤더가 달라질 수 있습니다.
        /// <code>
        ///           'Accept' = 'application/json'
        ///     'Content-Type' = 'application/json'
        /// </code>
        /// 그런 뒤, body에 DataLib에서 역직렬화가 가능한 JSON으로 직렬화를 하여 보내면 됩니다.<para/>
        /// DataLib에서 정의하는 데이터 유형이 바로 서버(ASP.NET Core/EF Core)와 클라이언트(Unity) 간의 약속이기 때문입니다.<para/>
        /// When passing information to the server, be sure to make a <c>POST</c> request.<para/>
        /// The header contains the following values.<para/>
        /// <code>
        ///           'Accept' = 'application/json'
        ///     'Content-Type' = 'application/json'
        /// </code>
        /// After that, serialize the body to JSON that can be deserialized in DataLib and send it.<para/>
        /// That's because the data types that DataLib defines are promises between the server(ASP.NET Core/EF Core) and the client(Unity).<para/>
        /// <para/>
        /// 🔗https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-javascript?view=aspnetcore-7.0 <para/>
        /// <para/>
        /// 서버측 로직은 단순히 빠른 스터디 진행을 위해 REST API를 구체적으로 구현하지 않았습니다.<para/>
        /// Server side logic does not specifically implement a REST API, simply to speed study progress.
        /// </remarks>
        [TriInspector.Button]
        void RegisterUser() {
            
            if (IsRequestRunning())
                return;
            
            string logInfo = $"{nameof(newUserName)}(name):{newUserName} / {nameof(newUserLevel)}(level):{newUserLevel} / {nameof(newUserRandomItemCount)}(itemCount):{newUserRandomItemCount}";
            Debug.Log($"[{nameof(RegisterUser)}] {logInfo} 정보를 서버에 넘겨주어 사용자를 등록합니다.");
            Debug.Log($"[{nameof(RegisterUser)}] Register the user by passing the information {logInfo} to the server.");
            
            string url = $"{serverUrl}/register?name={newUserName}&level={newUserLevel}&itemCount={newUserRandomItemCount}";
            currentRequest = StartCoroutine(GetRequestAsync<RegisterUserResults?>(url, OnRegisterUserResult, OnRegisterUserResult));
        }
        
        /// <summary>
        /// 서버 라우팅 경로 <c>/register</c> 에서 응답한 결과를 표시합니다.
        /// </summary>
        /// <param name="results"></param>
        private void OnRegisterUserResult(RegisterUserResults? results) {
            
            if (!results.HasValue) {
                
                Debug.LogError($"[{nameof(OnRegisterUserResult)}] 서버측에서 문제가 발생했습니다.");
                Debug.LogError($"[{nameof(OnRegisterUserResult)}] A problem occurred inside the server.");
                return;
            }
            
            switch (results.Value) {
                
                case RegisterUserResults.Success:
                    Debug.Log($"[{nameof(OnRegisterUserResult)}] 사용자를 성공적으로 등록했습니다.");
                    Debug.Log($"[{nameof(OnRegisterUserResult)}] User registered successfully.");
                    break;
                    
                case RegisterUserResults.NotValidData:
                    Debug.LogWarning($"[{nameof(OnRegisterUserResult)}] 등록하려는 사용자 정보가 유효하지 않습니다.");
                    Debug.LogWarning($"[{nameof(OnRegisterUserResult)}] The user data that trying to register is invalid.");
                    break;
                    
                case RegisterUserResults.DuplicatedName:
                    Debug.LogWarning($"[{nameof(OnRegisterUserResult)}] 사용자명이 중복되었습니다.");
                    Debug.LogWarning($"[{nameof(OnRegisterUserResult)}] Username already exist.");
                    break;
            }
        }
        
        /// <summary>
        /// 확장 메서드 기능(<see cref="InventoryItemArrayExtensions"/>)으로 사용자들의 정보와 인벤토리 데이터를 표시하는 사용자 목록을 로그에 남깁니다.<para/>
        /// Log a list of users displaying it's info and inventory with extension method(<see cref="InventoryItemArrayExtensions"/>) function.
        /// </summary>
        /// <param name="users">
        /// 사용자 목록<para/>
        /// List of user information.
        /// </param>
        private void ShowAllUsersWithExtension(UserData[] users) {
            
            Debug.Log("모든 사용자 목록");
            Debug.Log("List of registered users");
            
            foreach (var user in users)
                ShowUserInfoWithExtension(user);
        }
        
        /// <summary>
        /// 타입 래핑(<see cref="InventoryClient"/>)으로 사용자들의 정보와 인벤토리 데이터를 표시하는 사용자 목록을 로그에 남깁니다.<para/>
        /// Log a list of users displaying it's info and inventory data with wrapped type(<see cref="InventoryClient"/>).
        /// </summary>
        /// <param name="users">
        /// 사용자 목록<para/>
        /// List of user information.
        /// </param>
        private void ShowAllUsersWithWrapperType(UserData[] users) {
            
            Debug.Log("모든 사용자 목록");
            Debug.Log("List of registered users");
            
            foreach (var user in users)
                ShowUserInfoWithWrapperType(user);
        }
        
        /// <summary>
        /// 확장 메서드 기능(<see cref="InventoryItemArrayExtensions"/>)으로 사용자 정보와 인벤토리 데이터를 표시하는 사용자 정보를 로그에 남깁니다.<para/>
        /// Log a user displaying it's info and inventory data with extension method(<see cref="InventoryItemArrayExtensions"/>) function.
        /// </summary>
        /// <param name="user">
        /// 사용자 정보<para/>
        /// User information
        /// </param>
        private void ShowUserInfoWithExtension(UserData user) {
            
            Inventory inventory = user.Inventory;
            string[] items = inventory.GetItems();
            
            Debug.Log($"[Lv. {user.Level}] {user.Name} : Have {items.Length} items (클릭해서 자세히 보기 / Click to detail)\n- {string.Join("\n- ", items)}");
        }
        
        /// <summary>
        /// 타입 래핑(<see cref="InventoryClient"/>)으로 사용자 정보와 인벤토리 데이터를 표시하는 사용자 정보를 로그에 남깁니다.<para/>
        /// Log a user displaying it's info and inventory data with wrapped type(<see cref="InventoryClient"/>).
        /// </summary>
        /// <param name="user">
        /// 사용자 정보<para/>
        /// User information
        /// </param>
        private void ShowUserInfoWithWrapperType(UserData user) {
            
            InventoryClient inventory = new InventoryClient(user.Inventory);
            string[] items = inventory.Items;
            
            Debug.Log($"[Lv. {user.Level}] {user.Name} : Have {items.Length} items (클릭해서 자세히 보기 / Click to detail)\n- {string.Join("\n- ", items)}");
        }
        
        /// <summary>
        /// <see cref="InventoryClient"/>와 <see cref="Inventory"/>간 데이터 처리를 통해 DataLib에서 약속한 데이터로 변환할 수 있는지 증명하는 코드입니다.<para/>
        /// This code proves that data can be converted to the data promised by DataLib through data processing between <see cref="InventoryClient"/> and <see cref="Inventory"/>.<para/>
        /// </summary>
        [TriInspector.Button]
        private void TestConversationWrappingType() {
            
            Inventory source = new Inventory {
                    Items = JsonConvert.SerializeObject(new string[] {
                        "Target (과녁)",
                        "Lever (레버)",
                        "Button (버튼)",
                        "Pressure Plate (압력판)",
                        "Weighted Pressure Plate (무게 압력판)",
                        "Sculk Sensor (스컬크 감지체)"
                    })
                };
            
            InventoryClient wrappedData = new InventoryClient(source);
            
            string sourceJson = JsonConvert.SerializeObject(source, Formatting.Indented);
            string wrappedJson = JsonConvert.SerializeObject(wrappedData, Formatting.Indented);
            
            Debug.Log("Serialized source data " + sourceJson);
            Debug.Log("Serialized wrapped data " + wrappedJson);
            
            Inventory deserializeFromServer = JsonConvert.DeserializeObject<Inventory>(wrappedJson);
            
            Debug.Log("Deserialized with Inventory type from wrapped data(InventoryClient type)'s JSON");
            Debug.Log("Items : " + deserializeFromServer.Items);
            Debug.Log("Items (joined) : " + string.Join(", ", deserializeFromServer.GetItems()));
        }
    }
}