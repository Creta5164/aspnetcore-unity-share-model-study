using System.ComponentModel.DataAnnotations;

namespace DataLib;

/// <summary>
/// 인벤토리 데이터<para/>
/// Inventory data
/// </summary>
public class Inventory {
    
    /// <summary>
    /// 테이블 고유 ID<para/>
    /// Table unique id
    /// </summary>
    /// <value>ID값</value>
    [Key]
    public int ID { get; set; }
    
    /// <summary>
    /// 인벤토리 아이템 ID 데이터<para/>
    /// Inventory item ID data
    /// </summary>
    /// <remarks>
    /// 스터디로 활용했던 DB 환경이 SQLite이기 때문에 배열 타입을 활용하지 못했습니다.<para/>
    /// 다른 DB 환경(예를 들어 pgsql 등)의 경우에는 배열 타입을 사용할 수도 있습니다.<para/>
    /// DB environment used for the study was SQLite, so the array type could not be used.<para/>
    /// For other DB environments (e.g. pgsql) you can may use array types.
    /// </remarks>
    /// <value>
    /// 아이템 ID 문자열 배열<para/>
    /// Array of item id strings in JSON<para/>
    /// </value>
    public string ?Items { get; set; }
}