using System;
using System.Data.SQLite;

namespace AwesomeOverlay.UserDataStorage.Utilities
{
    public class DBColumnInfo
    {
        public Type ObjectType { get; }
        public string SqlType { get; }
        public Func<object, string, SQLiteParameter> ConvertToSQL { get; }
        public Func<object, object> ConvertFromSQL { get; }

        public DBColumnInfo(Type objectType, string sqlType, Func<object, string, SQLiteParameter> convertToSQL, Func<object, object> convertFromSQL) =>
            (ObjectType, SqlType, ConvertToSQL, ConvertFromSQL) = (objectType, sqlType, convertToSQL, convertFromSQL);
    }
}
