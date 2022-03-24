using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Model.UserServices;
using AwesomeOverlay.Core.Services.Abstractions;
using AwesomeOverlay.UserDataStorage.Abstraction;

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeOverlay.UserDataStorage.Core
{
    public class UserDataStorageManager : IUserDataStorageManager
    {
        public const string DATABASE_FILENAME = "userdata.db";

        private string _databasePath;
        private string _connectionString;
        private IUserFactoryService _userFactory;
        private Dictionary<IUserService<UserBase>, StorageUsersAdapter> _storageUsers = new Dictionary<IUserService<UserBase>, StorageUsersAdapter>();

        public UserDataStorageManager(IUserFactoryService userFactory)
        {
            _userFactory = userFactory;

            _databasePath = Path.Combine(Environment.CurrentDirectory, DATABASE_FILENAME);
            _connectionString = $"Data Source={DATABASE_FILENAME}";
        }

        /// <inheritdoc />
        public async Task InitAsync(List<IUserService<UserBase>> storageUsers, CancellationToken token = default)
        {
            foreach (IUserService<UserBase> storageUser in storageUsers) {
                _storageUsers.Add(storageUser, new StorageUsersAdapter(storageUser, _userFactory));
            }

            if (!File.Exists(_databasePath)) {
                SQLiteConnection.CreateFile(_databasePath);
                Console.WriteLine("markuping");
                await MarkupDatabaseFileAsync(token);
            }
        }

        /// <inheritdoc />
        public async Task<int> GetCurrentUserId(CancellationToken token = default)
        {
            int userId = 0;

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString)) {
                await connection.OpenAsync(token);

                try {
                    using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM SQLITE_SEQUENCE;", connection)) {
                        using (SQLiteDataReader dataReader = await command.ExecuteReaderAsync(token) as SQLiteDataReader) {
                            await dataReader.ReadAsync(token);
                            userId = dataReader.GetInt32(1);
                        }
                    }
                } catch { };
            }

            return userId;
        }

        /// <inheritdoc />
        public async Task AddUserAsync<UserType>(IUserService<UserType> userService, UserType user, CancellationToken token = default)
            where UserType : UserBase
        {
            user.StorageName = userService.StorageName;

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString)) {
                await connection.OpenAsync(token);

                using (SQLiteCommand command = new SQLiteCommand($"INSERT INTO users (id, storage_name) VALUES (NULL, \'{userService.StorageName}\')", connection)) {
                    await command.ExecuteNonQueryAsync(token);
                }

                using (SQLiteCommand command = _storageUsers[userService].CreateAddUserCommand(user, connection))
                    await command.ExecuteNonQueryAsync(token);
            }
        }

        /// <inheritdoc />
        public async Task UpdateUserData<UserType>(IUserService<UserType> userService, UserType user, string propertyName, CancellationToken token = default)
            where UserType : UserBase
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString)) {
                await connection.OpenAsync(token);
            
                using (SQLiteCommand command = _storageUsers[userService].CreateUpdateCommand(user, propertyName, connection)) {
                    await command.ExecuteNonQueryAsync(token);
                }
            }
        }

        /// <inheritdoc />
        public async Task<List<UserBase>> GetAllUsersAsync(CancellationToken token = default)
        {
            List<UserBase> users = new List<UserBase>();

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString)) {
                await connection.OpenAsync(token);

                using (SQLiteCommand getAllCommand = new SQLiteCommand("SELECT * FROM users", connection)) {
                    SQLiteDataReader reader = await getAllCommand.ExecuteReaderAsync(token) as SQLiteDataReader;
                    while (await reader.ReadAsync(token)) {
                        int id = reader.GetInt32(0);
                        string storageName = reader.GetString(1);

                        using (SQLiteCommand getUserDataCommand = new SQLiteCommand($"SELECT * FROM {storageName} WHERE id = {id}", connection)) {
                            SQLiteDataReader userDataReader = await getUserDataCommand.ExecuteReaderAsync(token) as SQLiteDataReader;
                            UserBase user = await GetStorageAdapterByStorageName(storageName).ReadUser(id, userDataReader, token);
                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        /// <inheritdoc />
        public async Task DeleteUserAsync(UserBase user, CancellationToken token = default)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString)) {
                await connection.OpenAsync(token);
                await EnableForeignKeys(connection, token);

                Console.WriteLine("id");
                using (SQLiteCommand command = new SQLiteCommand($"DELETE FROM users WHERE id = {user.UserId}", connection)) {
                    await command.ExecuteNonQueryAsync(token);
                }
            }
        }

        /// <inheritdoc />
        public async Task UnauthorizeUserAsync<UserModelType>(UserModelType user, CancellationToken token = default)
            where UserModelType : LogoutAbleUser
        {
            user.Authorized = false;

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString)) {
                await connection.OpenAsync(token);

                foreach (string columnName in GetStorageAdapterByStorageName(user.StorageName).GetNullifingColumnNames()) {
                    using (SQLiteCommand command = new SQLiteCommand($"UPDATE {user.StorageName} SET {columnName} = NULL WHERE id = {user.UserId}", connection)) {
                        await command.ExecuteNonQueryAsync(token);
                    }
                }

                using (SQLiteCommand command = new SQLiteCommand($"UPDATE {user.StorageName} SET authorized = False WHERE id = {user.UserId}", connection)) {
                    await command.ExecuteNonQueryAsync(token);
                }
            }
        }

        private async Task EnableForeignKeys(SQLiteConnection connection, CancellationToken token = default)
        {
            using (SQLiteCommand command = new SQLiteCommand("PRAGMA foreign_keys = 1", connection)) {
                await command.ExecuteNonQueryAsync(token);
            }
        }

        private async Task MarkupDatabaseFileAsync(CancellationToken token = default)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString)) {
                await connection.OpenAsync(token);

                // Creating users table
                using (SQLiteCommand command = new SQLiteCommand("CREATE TABLE users (id INTEGER PRIMARY KEY AUTOINCREMENT, storage_name TEXT NOT NULL)", connection)) {
                    await command.ExecuteNonQueryAsync(token);
                }

                // Creating detailed users tables
                foreach (var storageUser in _storageUsers.Values) {
                    Console.WriteLine("storage");
                    using (SQLiteCommand command = new SQLiteCommand(storageUser.CreateTableMarkupQuery(), connection))
                        await command.ExecuteNonQueryAsync(token);
                }
            }
        }

        private StorageUsersAdapter GetStorageAdapterByStorageName(string storageName) {
            foreach (var pair in _storageUsers) {
                if (pair.Key.StorageName == storageName)
                    return pair.Value;
            }

            throw new Exception($"Storage user with {storageName} storage name is not found");
        }
    }
}
