using System.Runtime.Serialization;
using DataLib;
using Newtonsoft.Json;

namespace ClientLogic.DataLibExtensions {
    
    /// <summary>
    /// Inventory 데이터에서 SQLite가 배열을 지원하지 않는 것을 보조하기 위한 확장 클래스<para/>
    /// An extension class to assist with SQLite's lack of support for arrays in Inventory data.
    /// </summary>
    public static class InventoryItemArrayExtensions {
        
        /// <summary>
        /// <see cref="T:DataLib.Inventory.Items"/> 문자열을 JSON 배열로 파싱하여 문자열 배열을 반환합니다.<para/>
        /// Parses the <see cref="T:DataLib.Inventory.Items"/> string into a JSON array and returns an array of strings.
        /// </summary>
        /// <param name="context">확장 메서드 대상 / Extension method target</param>
        /// <returns>
        /// <see cref="T:DataLib.Inventory.Items"/>를 <see cref="T:string[]"/>로 파싱한 값<para/>
        /// <see cref="T:string[]"/> that parsed from <see cref="T:DataLib.Inventory.Items"/>
        /// </returns>
        public static string[] GetItems(this Inventory context)
            => JsonConvert.DeserializeObject<string[]>(context.Items);
        
        /// <summary>
        /// <see cref="T:string[]"/>배열 값을 JSON으로 변환하여 <see cref="T:DataLib.Inventory.Items"/>에 담습니다.<para/>
        /// Convert the <see cref="T:string[]"/> array values to JSON and put them in <see cref="T:DataLib.Inventory.Items"/>.
        /// </summary>
        /// <param name="context">확장 메서드 대상 / Extension method target</param>
        /// <param name="value"><see cref="T:DataLib.Inventory.Items"/>에 지정하려는 배열 값 / Array value that assign to <see cref="T:DataLib.Inventory.Items"/></param>
        public static void SetItems(this Inventory context, string[] value)
            => context.Items = value == null ? null : JsonConvert.SerializeObject(value);
    }
    
    /// <summary>
    /// 확장 메서드 대신 데이터 타입 클래스로부터 상속받아 추가 기능을 구현한 <see cref="T:DataLib.Inventory"/>의 데이터 클래스<para/>
    /// Data class of <see cref="T:DataLib.Inventory"/> that implements additional functions by inheriting from data type class instead of extension method
    /// </summary>
    /// <remarks>
    /// 서버에서 사용하는 동일 타입의 데이터 클래스는 이를 모르기 때문에,
    /// 새로 정의하는 필드 및 속성값은 필요한 경우가 아니라면 반드시 직렬화가 되지 않도록 만들어야 합니다.<para/>
    /// 자세한 내용은 <see cref="Client.RegisterUser"/>의 주석을 확인하세요.<para/>
    /// Since the data classes of the same type used by the server do not know this,
    /// so newly defined field and property values must be made non-serializable unless necessary.<para/>
    /// Check comments on <see cref="Client.RegisterUser"/> for details.
    /// </remarks>
    public class InventoryClient : Inventory {
        
        /// <summary>
        /// 약속한 데이터 타입 <see cref="T:DataLib.Inventory"/>으로부터 데이터를 적용합니다.<para/>
        /// Apply from the promised data type <see cref="T:DataLib.Inventory"/>.
        /// </summary>
        /// <param name="source">원본 데이터 / Source data</param>
        public InventoryClient(Inventory source) {
            
            this.Items = JsonConvert.DeserializeObject<string[]>(source.Items);
        }
        
        /// <summary>
        /// <see cref="T:DataLib.Inventory.Items"/> 속성을 JSON 직렬화/역직렬화로 재정의한 속성입니다.<para/>
        /// Overriding the <see cref="T:DataLib.Inventory.Items"/> property to JSON serialize/deserialize.
        /// </summary>
        /// <remarks>
        /// https://www.newtonsoft.com/json/help/html/SerializationCallbacks.htm
        /// </remarks>
        /// <value>
        /// <see cref="T:DataLib.Inventory.Items"/>에서 파싱된 <see cref="T:string[]"/>값<para/>
        /// <see cref="T:string[]"/> value parsed from <see cref="T:DataLib.Inventory.Items"/>
        /// </value>
        [JsonIgnore]
        public new string[] Items { get; set; }
        
        /// <summary>
        /// Newtonsoft.Json에서 <see cref="T:DataLib.InventoryClient"/>를 직렬화 할 때,
        /// <see cref="T:DataLib.InventoryClient.Items"/>배열을 <see cref="T:DataLib.Inventory.Items"/>에 직렬화하여 넣습니다.
        /// </summary>
        /// <param name="context"></param>
        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context) {
            
            base.Items = JsonConvert.SerializeObject(this.Items);
        }
        
        /// <summary>
        /// Newtonsoft.Json에서 <see cref="T:DataLib.Inventory"/>를 역직렬화 할 때,
        /// JSON으로 직렬화 되어있는(약속한 타입) <see cref="T:DataLib.Inventory.Items"/> 문자열을 <see cref="T:DataLib.InventoryClient.Items"/>에 역직렬화하여 넣습니다.
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context) {
            
            this.Items = JsonConvert.DeserializeObject<string[]>(base.Items);
        }
    }
}