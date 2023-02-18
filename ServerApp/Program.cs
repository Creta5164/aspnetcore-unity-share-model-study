using DataLib;
using ServerApp.Database;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace ServerApp;

public static class Program {
    
    /// <summary>
    /// 더미로 사용할 게임 아이템 데이터입니다.<para/>
    /// Game item data to be used as a dummy.
    /// </summary>
    /// <value>
    /// 더미 아이템 목록<para/>
    /// List of dummy items
    /// </value>
    private static readonly string[] dummyItemsData = new string[] {
            "Redstone Dust (레드스톤 가루)",
            "Redstone Torch (레드스톤 횃불)",
            "Block of Redstone (레드스톤 블록)",
            "Redstone Repeater (레드스톤 중계기)",
            "Redstone Comparator (레드스톤 비교기)",
            "Target (과녁)",
            "Lever (레버)",
            "Button (버튼)",
            "Pressure Plate (압력판)",
            "Weighted Pressure Plate (무게 압력판)",
            "Sculk Sensor (스컬크 감지체)",
            "Tripwire Hook (철사 덫 갈고리)",
            "Lectern (독서대)",
            "Daylight Detector (햇빛 감지기)",
            "Lightning Rod (피뢰침)",
            "Observer (관측기)",
            "Trapped Chest (덫 상자)",
            "Piston (피스톤)",
            "Slime Block (슬라임 블록)",
            "Honey Block (꿀 블록)",
            "Dispenser (발사기)",
            "Dropper (공급기)",
            "Hopper (호퍼)",
            "Note Block (소리 블록)",
            "Wood Door (나무 문)",
            "Iron Door (철 문)",
            "Wood Fence Gate (나무 울타리 문)",
            "Wood Trapdoor (나무 다락문)",
            "Iron Trapdoor (철 다락문)",
            "TNT",
            "Redstone Lamp (레드스톤 조명)"
        };
    
    public static void Main(string[] args) {
        
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEntityFrameworkSqlite()
            .AddDbContext<DataLibContext>()
            .ConfigureHttpJsonOptions(options => {
                
                options.SerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            });

        var app = builder.Build();

        using(var scope = app.Services.CreateScope()) {
            
            var dbContext = scope.ServiceProvider.GetRequiredService<DataLibContext>();
            
            dbContext.Database.EnsureCreated();
            dbContext.SaveChanges();
        }
        
        //모든 사용자 정보 가져오기
        //Get all registered users
        app.MapGet("/", GetAllUsers);
        
        //특정 사용자 정보 가져오기
        //Get specified user
        app.MapGet("/name={name}", GetUserByName);
        
        //사용자 등록하기
        //Register new user
        app.MapGet("/register", OnRegisterUser);

        app.Run();
    }
    
    /// <summary>
    /// 경로(Route) : <c>/</c><para/>
    /// 모든 사용자 정보를 가져옵니다.<para/>
    /// Gets all user information.
    /// </summary>
    /// <param name="db">
    /// 데이터베이스 컨텍스트<para/>
    /// Database context
    /// </param>
    /// <returns>
    /// 모든 사용자 정보가 담긴 목록<para/>
    /// List of users with information
    /// </returns>
    private static UserData[] GetAllUsers(DataLibContext db)
        => db.Users!.Include(u => u.Inventory).ToArray();
    
    /// <summary>
    /// 경로(Route) : <c>/name={name}</c><para/>
    /// 특정한 사용자 정보를 가져옵니다.<para/>
    /// Gets specified user information.
    /// </summary>
    /// <param name="db">
    /// 데이터베이스 컨텍스트<para/>
    /// Database context
    /// </param>
    /// <param name="name">
    /// 특정할 닉네임<para/>
    /// Specified nickname
    /// </param>
    /// <returns>
    /// 특정한 사용자 정보가 담긴 목록<para/>
    /// List of users with information
    /// </returns>
    private static UserData? GetUserByName(DataLibContext db, string name)
        => db.Users!.Include(u => u.Inventory).FirstOrDefault(u => u.Name == name);
    
    /// <summary>
    /// 경로(Route) : <c>/register</c><para/>
    /// 사용자를 등록합니다.<para/>
    /// Registers user.
    /// </summary>
    /// <param name="db">
    /// 데이터베이스 컨텍스트<para/>
    /// Database context
    /// </param>
    /// <param name="name">
    /// 사용자 닉네임<para/>
    /// User nickname
    /// </param>
    /// <param name="level">
    /// 사용자 레벨<para/>
    /// User level
    /// </param>
    /// <param name="itemCount">
    /// <see cref="UserData.Inventory"/>에 들어갈 더미 아이템 개수<para/>
    /// Number of dummy items to fit in <see cref="UserData.Inventory"/>
    /// </param>
    /// <returns>
    /// 가능한 경우, <see cref="RegisterUserResults"/>를 반환합니다.
    /// If possible, returns <see cref="RegisterUserResults"/>.
    /// </returns>
    private static async Task<IResult> OnRegisterUser(DataLibContext db, string name, uint level, int itemCount) {
        
        //TODO : 사용자 데이터 등록 검증 로직 구현하기
        //TODO : Implement user data registration validation logic
        if (false)
            return Results.BadRequest(RegisterUserResults.NotValidData);
        
        //데이터베이스에 등록하기 전에 name으로 된 사용자가 있는지 검사
        //Check if there is a user with name before registering in database
        if (db.Users!.Any(u => u.Name == name))
            return Results.Conflict(RegisterUserResults.DuplicatedName);
        
        //스터디 중에 실수해서 서버 멈출 수도 있으니까 제한
        //Clamp count because there is a possibility that the server may be stopped due to a mistake during study
        itemCount = Math.Clamp(itemCount, 0, 20);
        
        //DB UserData 테이블에 등록
        //Register UserData table in DB
        try {
            
            db.Users!.Add(new UserData {
                Name = name,
                Level = level,
                Inventory = TestInventory(itemCount)
            });
            
            await db.SaveChangesAsync();
            
        } catch (Exception ex) {
            
            //TODO : 서버 로그 남기기
            //TODO : Leave server log
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        return Results.Ok(RegisterUserResults.Success);
    }
    
    /// <summary>
    /// 랜덤 아이템이 개수만큼 들어간 더미 인벤토리를 생성합니다.<para/>
    /// Create a dummy inventory containing the number of random items.
    /// </summary>
    /// <param name="itemCount">
    /// 더미 아이템 개수<para/>
    /// Count of dummy items
    /// </param>
    /// <returns>
    /// 더미 아이템이 들어간 인벤토리 데이터<para/>
    /// Inventory data with dummy items
    /// </returns>
    private static Inventory TestInventory(int itemCount) {
        
        Random random = new Random();
        string[] result = Enumerable.Range(0, itemCount)
            .Select(_ => dummyItemsData[random.Next(dummyItemsData.Length)])
            .ToArray();
        
        return new Inventory {
            Items = JsonSerializer.Serialize(result)
        };
    }
}