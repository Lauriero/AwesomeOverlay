using AwesomeOverlay.Core.Attributes;
using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Model.UserServices;
using AwesomeOverlay.Core.Services.Abstractions;
using AwesomeOverlay.Core.Utilities.SecurityUtilities;
using AwesomeOverlay.UserDataStorage.Abstraction;
using AwesomeOverlay.UserDataStorage.Utilities;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeOverlay.UserDataStorage.Core
{
    /// <summary>
    /// Contains information of the specific IStoreUserData<UserModel> class and UserModel type
    /// </summary> 
    class StorageUsersAdapter
    {
        private Type _userModelType;
        private IUserFactoryService _userFactory;
        private IUserService<UserBase> _userService;
        private Dictionary<PropertyInfo, DataBaseColumnAttribute> _tableColumns = new Dictionary<PropertyInfo, DataBaseColumnAttribute>();

        public StorageUsersAdapter(IUserService<UserBase> userService, IUserFactoryService userFactory)
        {
            _userFactory = userFactory;
            _userService = userService;

            _userModelType = userService.GetType().GetInterface("IUserService`1").GetGenericArguments()[0];
            foreach (PropertyInfo property in _userModelType.GetProperties()) {
                DataBaseColumnAttribute columnAttribute = property.GetCustomAttribute<DataBaseColumnAttribute>();
                if (columnAttribute == null) 
                    continue;
                else if (!IsTypeSupported(property.PropertyType))
                    throw new Exception($"Type {property.PropertyType} is not supported");

                _tableColumns.Add(property, columnAttribute);
            }
        }

        /// <summary>
        /// Returns sql query for marking up the table
        /// </summary>
        public string CreateTableMarkupQuery()
        {
            string sqlQuery = $"CREATE TABLE {_userService.StorageName} (id INTEGER,";
            foreach (var tableColumn in _tableColumns) {
                sqlQuery += $"{tableColumn.Value.ColumnName} {GetColumnInfoByType(tableColumn.Key.PropertyType).SqlType} {(tableColumn.Key.PropertyType.IsValueType ? "NOT NULL" : "")},";
            }

            sqlQuery += "FOREIGN KEY (id) REFERENCES users (id) ON DELETE CASCADE ON UPDATE CASCADE);";

            return sqlQuery;
        }

        /// <summary>
        /// Returns sql query for adding user into a table for storage user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public SQLiteCommand CreateAddUserCommand(UserBase user, SQLiteConnection connection)
        {
            SQLiteCommand command = new SQLiteCommand(connection);

            string sqlQuery = $"INSERT INTO {_userService.StorageName} VALUES ({user.UserId}, ";
            for (int i = 0; i < _tableColumns.Count; i++) {
                sqlQuery += $"@{i.ToString()} {((i != _tableColumns.Count - 1) ? ',' : ' ')}";
                
                var tableColumn = _tableColumns.ElementAt(i);
                command.Parameters.Add(
                    GetColumnInfoByType(tableColumn.Key.PropertyType)
                        .ConvertToSQL(tableColumn.Key.GetValue(user), i.ToString()));
            }

            sqlQuery += ");";
            command.CommandText = sqlQuery;

            return command;
        }

        /// <summary>
        /// Creates sql query for updating a single property of user object
        /// </summary>
        public SQLiteCommand CreateUpdateCommand(UserBase user, string propertyName, SQLiteConnection connection)
        {
            foreach (var pair in _tableColumns) {
                if (pair.Key.Name == propertyName) {
                    SQLiteCommand command = new SQLiteCommand(connection);
                    command.CommandText = $"UPDATE {_userService.StorageName} SET {pair.Value.ColumnName} = @param";
                    command.Parameters.Add(
                        _availableColumns
                            .Where(c => c.ObjectType == pair.Key.PropertyType)
                            .FirstOrDefault()
                            .ConvertToSQL(pair.Key.GetValue(user), "param")
                    );

                    return command;
                }
            }

            throw new Exception($"{user.GetType()} has no {propertyName} property");
        }

        /// <summary>
        /// Returns enumerator of database column names which should be set to null when user is unauthorized
        /// </summary>
        public List<string> GetNullifingColumnNames()
        {
            List<string> columnNames = new List<string>();

            foreach (DataBaseColumnAttribute dataBaseColumn in _tableColumns.Values) {
                if (dataBaseColumn.DeleteOnUnauthorize)
                    columnNames.Add(dataBaseColumn.ColumnName);
            }

            return columnNames;
        }

        public async Task<UserBase> ReadUser(int userId, SQLiteDataReader dataReader, CancellationToken token)
        {
            await dataReader.ReadAsync(token);

            UserBase user = _userFactory.CreateInstance(_userService, userId) as UserBase;
            user.StorageName = _userService.StorageName;

            for (int i = 0; i < _tableColumns.Count; i++) {
                var tableColumn = _tableColumns.ElementAt(i);
                tableColumn.Key.SetValue(user, GetColumnInfoByType(tableColumn.Key.PropertyType).ConvertFromSQL(dataReader.GetValue(i + 1)));
            }

            return user;
        }

        public static bool IsTypeSupported(Type type)
        {
            foreach (var item in _availableColumns) {
                if (item.ObjectType == type)
                    return true;
            }

            return false;
        }

        public static DBColumnInfo GetColumnInfoByType(Type type)
        {
            foreach (var item in _availableColumns) {
                if (item.ObjectType == type)
                    return item;
            }

            throw new Exception($"{type} column type is not supported");
        }

        private static ReadOnlyCollection<DBColumnInfo> _availableColumns = new ReadOnlyCollection<DBColumnInfo>(new List<DBColumnInfo>() {
            new DBColumnInfo(
                typeof(string),
                "TEXT",
                (obj, parameterName) => {
                    SQLiteParameter parameter = new SQLiteParameter(DbType.String, obj);
                    parameter.ParameterName = parameterName;
                    return parameter;
                },
                s => { return s is DBNull ? "" : s.ToString(); }),

            new DBColumnInfo(
                typeof(bool),
                "INTEGER",
                (obj, parameterName) => {
                    SQLiteParameter parameter = new SQLiteParameter(DbType.Int32, obj);
                    parameter.ParameterName = parameterName;
                    return parameter;
                },
                s => {return s is DBNull ? false : Convert.ToBoolean(s); }),

            new DBColumnInfo(
                typeof(SecureString), 
                "BLOB",
                (obj, parameterName) => {
                    SQLiteParameter parameter = new SQLiteParameter(DbType.Binary, (obj as SecureString).Protect(null, SecureStringExtensions.DataProtectionScope.CurrentUser));
                    parameter.ParameterName = parameterName;
                    return parameter;
                },
                s => {
                    SecureString data = new SecureString();
                    data.AppendProtected(s as byte[], null, SecureStringExtensions.DataProtectionScope.CurrentUser);
                    return data;
                })
        });
    }
}
