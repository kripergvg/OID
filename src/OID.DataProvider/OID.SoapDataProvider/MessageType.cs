namespace OID.SoapDataProvider
{
    public enum MessageType
    {
        ExpiredClosesNewopened = 1,
        SessionExpiredTimeAdded = 2,
        SesssionOpened = 3,
        UnableOpeSession = 4,
        AuthComplited = 5,
        AuthError = 6,
        ImposibleCreateUser = 7,
        LoginExist = 8,
        SessionNotAcitve = 9,
        SessionCheked = 10,
        UserAdded = 11,
        NotComplitedAction = 12,
        SessionClosed = 13,
        ErorrClosingSession = 14,
        SessionClosedAuto = 15,
        UserStatusChanged = 16,
        UserInfoUpdated = 17,
        UserInfoAdded = 18,
        UserInfoDeleted = 19,
        ObjectUpdated = 20,
        ObjectInfoAdded = 21,
        ObjectInfoDeleted = 22,
        DealUpdated = 23,
        DealinfoAdded = 24,
        DealInfoDeleted = 25,
        UnableDatabaseConnect = 26,
        SqlParsingError = 27,
        ConvertingXmlToSqlError = 28,
        CommitingDataError = 29,
        QueryComplitedButDataNotConverted = 30
    }
}
