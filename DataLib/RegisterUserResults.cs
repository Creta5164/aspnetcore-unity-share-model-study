namespace DataLib;

/// <summary>
/// 사용자 등록 응답 타입<para/>
/// User registration response type
/// </summary>
public enum RegisterUserResults {
    
    /// <summary>
    /// 등록하려는 데이터에 문제가 있음<para/>
    /// There's a problem with the data that trying to register
    /// </summary>
    NotValidData,
    /// <summary>
    /// 중복 닉네임<para/>
    /// duplicate nickname
    /// </summary>
    DuplicatedName,
    /// <summary>
    /// 등록 성공<para/>
    /// registration success
    /// </summary>
    Success
}