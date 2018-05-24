using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using SimpleSQL;

namespace SqlCipher4Unity3D
{
    public delegate void DatabaseCreatedDelegate(string fullPath);

    public class SimpleSQLManager : MonoBehaviour
    {
        protected SQLiteConnection _db;

        public string databaseName = "";
        public string overrideBasePath = "";
        public bool changeWorkingName = false;
        public string workingName = "";
        public bool overwriteIfExists = false;
        public bool overwriteIfAppVersionUp = false;
        public bool debugTrace = false;
        public bool dbPasswordOn = false;
        private string dbPassword = null;

        public DatabaseCreatedDelegate databaseCreated;

        void Awake()
        {
            Initialize(false);
        }

        private static string SOLITE_APPVERSION_KEY = "SOLITE_APPVERSION";

        public void Initialize(bool forceReinitialization)
        {
            if (_db == null || forceReinitialization)
            {
                initalizeFlags();

                //TODOリリース時はパスワードを埋め込む
#if BUILD_TYPE_DEBUG
                dbPassword = "x'0000000000000000000000000000000000000000000000000000000000000000'";
#else
                dbPassword = "x'0000000000000000000000000000000000000000000000000000000000000000'";
#endif

                if (changeWorkingName && workingName.Trim() == "")
                {
                    Debug.LogError("Sqlite3: If you want to change the database's working name, then you will need to supply a new working name in the SimpleSQLManager [" + gameObject.name + "]");
                    return;
                }

                Dispose();

                DivRNUtil.ResetTableName();

                string documentsPath = getPath();

                bool fileExists = (DbFileList().Count > 0) ? true : false;
                bool proceed = true;

                if ((overwriteIfExists && fileExists) ||
                    !fileExists ||
                    (overwriteIfAppVersionUp && PlayerPrefs.GetInt(SOLITE_APPVERSION_KEY, 0) < GlobalDefine.AppVerToInt()))
                {
                    try
                    {
                        if (fileExists)
                        {
                            RemoveDbFiles();
                        }

                        CopyDbFile(documentsPath);
                        PlayerPrefs.SetInt(SOLITE_APPVERSION_KEY, GlobalDefine.AppVerToInt());
                        PlayerPrefs.Save();

                        if (databaseCreated != null)
                        {
                            databaseCreated(documentsPath);
                        }
                    }
                    catch
                    {
                        proceed = false;
                        Debug.LogError("Sqlite3: Failed to open database at the working path: " + documentsPath);
                    }
                }

                if (proceed)
                {
                    CreateConnection(documentsPath);
                    _db.Trace = debugTrace;
                }
            }
        }

        private void initalizeFlags()
        {
#if UNITY_EDITOR
#else
            changeWorkingName = false;
            overwriteIfExists = false;
            overwriteIfAppVersionUp = true;
            debugTrace = false;
            dbPasswordOn = true;
            overrideBasePath = null;
#endif
        }

        public string getPath()
        {
            initalizeFlags();

            string filename = changeWorkingName ? workingName.Trim() : getFilename();
            string documentsPath = Path.Combine(string.IsNullOrEmpty(overrideBasePath) ? LocalSaveUtilToInstallFolder.GetSavePathSqlite() : overrideBasePath, filename);
            return documentsPath;
        }

        private void CopyDbFile(string documentsPath)
        {
            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, databaseName);

#if UNITY_ANDROID && !UNITY_EDITOR
            WWW www = new WWW(filePath);
            while (!www.isDone){} 
            File.WriteAllBytes(documentsPath, www.bytes);
#else
            File.Copy(filePath, documentsPath);
#endif
        }

        protected virtual void CreateConnection(string documentsPath)
        {

            if (dbPasswordOn == true && string.IsNullOrEmpty(dbPassword) == false)
            {
                _db = new SQLiteConnection(documentsPath, dbPassword);
            }
            else
            {
                _db = new SQLiteConnection(documentsPath);
            }
        }

        private static byte[] StreamToBytes(Stream input)
        {
            int capacity = input.CanSeek ? (int)input.Length : 0;
            using (MemoryStream output = new MemoryStream(capacity))
            {
                int readLength;
                byte[] buffer = new byte[4096];

                do
                {
                    readLength = input.Read(buffer, 0, buffer.Length); // had to change to buffer.Length
                    output.Write(buffer, 0, readLength);
                }
                while (readLength != 0);

                return output.ToArray();
            }
        }

        public void Dispose()
        {
            if (_db != null)
            {
#if BUILD_TYPE_DEBUG
                if (debugTrace)
                {
                    Debug.Log("Sqlite3: " + name + ": disposing connection");
                }
#endif
                _db.Dispose();
                _db = null;
            }
        }

        void OnApplicationQuit()
        {
            Dispose();
        }

        private List<string> DbFileList()
        {
            string[] ext = new string[] { "", "-shm", "-wal" };
            List<string> filelist = new List<string>();
            string path = LocalSaveUtilToInstallFolder.GetSavePathSqlite() + "/" + databaseName;
            for (int i = 0; i < ext.Length; i++)
            {
                string documentsPath = path + ext[i];
                if (File.Exists(documentsPath))
                {
                    //Debug.Log("documentsPath list: add" + documentsPath);
                    filelist.Add(documentsPath);
                }
            }

            return filelist;
        }

        public void RemoveDbFiles()
        {
            List<string> list = DbFileList();

            for (int i = 0; i < list.Count; i++)
            {
                //Debug.Log("documentsPath remove: " + list[i]);
                File.Delete(list[i]);
            }
        }

        public bool BrokenDb()
        {
            if (_db == null)
            {
                return true;
            }

            return _db.brokenDb();
        }

        /// <summary>
        /// Sets a busy handler to sleep the specified amount of time when a table is locked.
        /// The handler will sleep multiple times until a total time of <see cref="BusyTimeout"/> has accumulated.
        /// </summary>
        public TimeSpan BusyTimeout
        {
            get
            {
                Initialize(false);

                return _db.BusyTimeout;
            }
            set
            {
                Initialize(false);

                _db.BusyTimeout = value;
            }
        }

        /// <summary>
        /// Whether <see cref="BeginTransaction"/> has been called and the database is waiting for a <see cref="Commit"/>.
        /// </summary>
        public bool IsInTransaction
        {
            get
            {
                Initialize(false);

                return _db.IsInTransaction;
            }
        }

        /// <summary>
        /// Returns the mappings from types to tables that the connection
        /// currently understands.
        /// </summary>
        public IEnumerable<TableMapping> TableMappings
        {
            get
            {
                Initialize(false);

                return _db.TableMappings;
            }
        }

        /// <summary>
        /// Retrieves the mapping that is automatically generated for the given type.
        /// </summary>
        /// <param name="type">
        /// The type whose mapping to the database is returned.
        /// </param>
        /// <returns>
        /// The mapping represents the schema of the columns of the database and contains 
        /// methods to set and get properties of objects.
        /// </returns>
        public TableMapping GetMapping(Type type)
        {
            Initialize(false);

            return _db.GetMapping(type);
        }

        /// <summary>
        /// Executes a "create table if not exists" on the database. It also
        /// creates any specified indexes on the columns of the table. It uses
        /// a schema automatically generated from the specified type. You can
        /// later access this schema by calling GetMapping.
        /// </summary>
        /// <returns>
        /// The number of entries added to the database schema.
        /// </returns>
        public int CreateTable<T>()
        {
            Initialize(false);

            return _db.CreateTable<T>();
        }

        /// <summary>
        /// Creates a new SQLiteCommand given the command text with arguments. Place a '?'
        /// in the command text for each of the arguments.
        /// </summary>
        /// <param name="cmdText">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the command text.
        /// </param>
        /// <returns>
        /// A <see cref="SQLiteCommand"/>
        /// </returns>
        public SQLiteCommand CreateCommand(string cmdText, params object[] ps)
        {
            Initialize(false);

            return _db.CreateCommand(cmdText, ps);
        }

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// Use this method instead of Query when you don't expect rows back. Such cases include
        /// INSERTs, UPDATEs, and DELETEs.
        /// You can set the Trace or TimeExecution properties of the connection
        /// to profile execution.
        /// </summary>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// The number of rows modified in the database as a result of this execution.
        /// </returns>	
        public int Execute(string query, params object[] args)
        {
            Initialize(false);

            return _db.Execute(query, args);
        }

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// It returns each row of the result using the mapping automatically generated for
        /// the given type.
        /// </summary>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// An enumerable with one result for each row returned by the query.
        /// </returns>	
        public List<T> Query<T>(string query, params object[] args) where T : new()
        {
            Initialize(false);

            return _db.Query<T>(query, args);
        }

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// It returns each row of the result using the specified mapping. This function is
        /// only used by libraries in order to query the database via introspection. It is
        /// normally not used.
        /// </summary>
        /// <param name="map">
        /// A <see cref="TableMapping"/> to use to convert the resulting rows
        /// into objects.
        /// </param>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// An enumerable with one result for each row returned by the query.
        /// </returns>	
        public List<object> Query(TableMapping map, string query, params object[] args)
        {
            Initialize(false);

            return _db.Query(map, query, args);
        }

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// It returns a simple data table structure that mimics some of the functionality
        /// of a System.Data.DataTable without the overhead or requirements of that .NET library.
        /// </summary>
        /// <param name="query">The fully escaped SQL.</param>
        /// <param name="args">Arguments to substitute for the occurences of '?' in the query.</param>
        /// <returns>Simple data table similar to System.Data.DataTable</returns>
        public SimpleDataTable QueryGeneric(string query, params object[] args)
        {
            var cmd = CreateCommand(query, args);
            return cmd.ExecuteQueryGeneric();
        }

        /// <summary>
        /// Returns the first record of a query formatted to the ORM class. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// </summary>
        /// <typeparam name="T">The type of the ORM class to format the results to</typeparam>
        /// <param name="recordExists">True if there was at least one record</param>
        /// <param name="query">The fully escaped SQL.</param>
        /// <param name="args">Arguments to substitute for the occurences of '?' in the query.</param>
        /// <returns>An object populated with the results, formatted to the ORM class</returns>
        public T QueryFirstRecord<T>(out bool recordExists, string query, params object[] args) where T : new()
        {
            Initialize(false);

            List<T> list = _db.Query<T>(query, args);

            if (list.Count > 0)
            {
                recordExists = true;
                return list[0];
            }
            else
            {
                recordExists = false;
                return default(T);
            }
        }

        /// <summary>
        /// Returns the first record of a query as an object. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// </summary>
        /// <param name="recordExists">True if there was at least one record</param>
        /// <param name="map">Mapping of the table structure</param>
        /// <param name="query">The fully escaped SQL.</param>
        /// <param name="args">Arguments to substitute for the occurences of '?' in the query.</param>
        /// <returns>An object with the results of the query</returns>
        public object QueryFirstRecord(out bool recordExists, TableMapping map, string query, params object[] args)
        {
            Initialize(false);

            List<object> list = _db.Query(map, query, args);

            if (list.Count > 0)
            {
                recordExists = true;
                return list[0];
            }
            else
            {
                recordExists = false;
                return null;
            }
        }

        /// <summary>
        /// Returns a queryable interface to the table represented by the given type.
        /// </summary>
        /// <returns>
        /// A queryable object that is able to translate Where, OrderBy, and Take
        /// queries into native SQL.
        /// </returns>	
        public TableQuery<T> Table<T>() where T : new()
        {
            Initialize(false);

            return _db.Table<T>();
        }

        /// <summary>
        /// Attempts to retrieve an object with the given primary key from the table
        /// associated with the specified type. Use of this method requires that
        /// the given type have a designated PrimaryKey (using the PrimaryKeyAttribute).
        /// </summary>
        /// <param name="pk">
        /// The primary key.
        /// </param>
        /// <returns>
        /// The object with the given primary key. Throws a not found exception
        /// if the object is not found.
        /// </returns>	
        public T Get<T>(object pk) where T : new()
        {
            Initialize(false);

            return _db.Get<T>(pk);
        }

        /// <summary>
        /// Begins a new transaction. Call <see cref="Commit"/> to end the transaction.
        /// </summary>
        public void BeginTransaction()
        {
            Initialize(false);

            _db.BeginTransaction();
        }

        /// <summary>
        /// Rolls back the transaction that was begun by <see cref="BeginTransaction"/>.
        /// </summary>
        public void Rollback()
        {
            Initialize(false);

            _db.Rollback();
        }

        /// <summary>
        /// Commits the transaction that was begun by <see cref="BeginTransaction"/>.
        /// </summary>
        public void Commit()
        {
            Initialize(false);

            _db.Commit();
        }

        /// <summary>
        /// Executes <param name="action"> within a transaction and automatically rollsback the transaction
        /// if an exception occurs. The exception is rethrown.
        /// </summary>
        /// <param name="action">
        /// The <see cref="Action"/> to perform within a transaction. <param name="action"> can contain any number
        /// of operations on the connection but should never call <see cref="BeginTransaction"/>,
        /// <see cref="Rollback"/>, or <see cref="Commit"/>.
        /// </param>
        public void RunInTransaction(Action action)
        {
            Initialize(false);

            _db.RunInTransaction(action);
        }

        /// <summary>
        /// Inserts all specified objects.
        /// </summary>
        /// <param name="objects">
        /// An <see cref="IEnumerable"/> of the objects to insert.
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        public int InsertAll(System.Collections.IEnumerable objects)
        {
            Initialize(false);
            return _db.InsertAll(objects);
        }

        public int InsertAll(System.Collections.IEnumerable objects, string extra)
        {
            Initialize(false);
            return _db.InsertAll(objects, extra);
        }

        public int InsertAll(System.Collections.IEnumerable objects, Type objType)
        {
            Initialize(false);
            return _db.InsertAll(objects, objType);
        }

        /// <summary>
        /// Inserts the given object and retrieves its
        /// auto incremented primary key if it has one.
        /// </summary>
        /// <param name="obj">
        /// The object to insert.
        /// </param>
        /// <returns>
        /// The number of rows added to the table.
        /// </returns>
        public int Insert(object obj)
        {
            Initialize(false);
            return _db.Insert(obj);
        }

        public int Insert(object obj, Type objType)
        {
            Initialize(false);
            return _db.Insert(obj, objType);
        }

        public int Insert(object obj, string extra)
        {
            Initialize(false);
            return Insert(obj, extra);
        }

        /// <summary>
        /// Updates all of the columns of a table using the specified object
        /// except for its primary key.
        /// The object is required to have a primary key.
        /// </summary>
        /// <param name="obj">
        /// The object to update. It must have a primary key designated using the PrimaryKeyAttribute.
        /// </param>
        /// <returns>
        /// The number of rows updated.
        /// </returns>
        public int UpdateTable(object obj)
        {
            Initialize(false);

            return _db.Update(obj);
        }

        public int UpdateTable(object obj, Type objType)
        {
            Initialize(false);

            return _db.Update(obj, objType);
        }

        /// <summary>
        /// Deletes the given object from the database using its primary key.
        /// </summary>
        /// <param name="obj">
        /// The object to delete. It must have a primary key designated using the PrimaryKeyAttribute.
        /// </param>
        /// <returns>
        /// The number of rows deleted.
        /// </returns>
        public int Delete<T>(T obj)
        {
            Initialize(false);

            return _db.Delete<T>(obj);
        }

        public int DeleteAll<T>()
        {
            Initialize(false);

            return _db.DeleteAll<T>();
        }

        public string getFilename()
        {
            return databaseName;
        }
    }
}
