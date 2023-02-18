using System.ComponentModel.DataAnnotations;

namespace DataLib;

/// <summary>
/// 유저 데이터
/// </summary>
public class UserData {
    
    /// <summary>
    /// 닉네임<para/>
    /// Nickname
    /// </summary>
    /// <value>
    /// 닉네임 문자열<para/>
    /// Nickname string
    /// </value>
    [Key]
    public virtual string ?Name { get; set; }
    
    /// <summary>
    /// 레벨<para/>
    /// Level
    /// </summary>
    /// <value>
    /// 유저 레벨값<para/>
    /// User level value
    /// </value>
    public uint Level { get; set; }
    
    /// <summary>
    /// 유저 인벤토리 정보<para/>
    /// User inventory information
    /// </summary>
    /// <value>
    /// <see cref="DataLib.Inventory"/> 테이블 내에 참조된 열<para/>
    /// Referenced row in <see cref="DataLib.Inventory"/> table
    /// </value>
    public Inventory ?Inventory { get; set; }
}