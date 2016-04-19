using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;

namespace EntityFix
{
    /// <summary>
    /// Fixes EF empty reader exception when calling procedure with output parameters
    /// Run EntityFix.Load() to start fix
    /// </summary>
    public static class EntityFix
    {

        private static readonly EntityFixInterceptor Interceptor = new EntityFixInterceptor();
        private static readonly object LoadingLock = new object();

        public static bool Loaded { get; private set; }
        /// <summary>
        /// Fixing entity
        /// </summary>
        public static void Load()
        {
            lock (LoadingLock)
            {
                if (Loaded)
                    return;
                DbInterception.Add(Interceptor);
                Loaded = true;
            }
        }

        /// <summary>
        /// Remove entity fix
        /// </summary>
        public static void Unload()
        {
            lock (LoadingLock)
            {
                if (!Loaded)
                    return;
                DbInterception.Remove(Interceptor);
                Loaded = false;
            }

        }
        /// <summary>
        /// Fixed data reader dummy
        /// </summary>
        private class EntityFixDataReader : DbDataReader
        {

            private readonly SqlDataReader _innerDataReader;

            public EntityFixDataReader(SqlDataReader innerDataReader)
            {
                _innerDataReader = innerDataReader;
            }

            public override void Close()
            {
                _innerDataReader.Close();  //close inner reader
            }

            public override DataTable GetSchemaTable()
            {
                return new DataTable();
            }

            public override bool NextResult()
            {
                return false;
            }

            public override bool Read()
            {
                return false;
            }

            public override int Depth
            {
                get { return 0; }
            }

            public override bool IsClosed
            {
                get { return _innerDataReader.IsClosed; }   //return inner state
            }

            public override int RecordsAffected
            {
                get { return 0; }
            }

            public override bool GetBoolean(int ordinal)
            {
                return default(bool);
            }

            public override byte GetByte(int ordinal)
            {
                return default(byte);
            }

            public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
            {
                return default(long);
            }

            public override char GetChar(int ordinal)
            {
                return default(char);
            }

            public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
            {
                return default(long);
            }

            public override Guid GetGuid(int ordinal)
            {
                return default(Guid);
            }

            public override short GetInt16(int ordinal)
            {
                return default(Int16);
            }

            public override int GetInt32(int ordinal)
            {
                return default(int);
            }

            public override long GetInt64(int ordinal)
            {
                return default(long);
            }

            public override DateTime GetDateTime(int ordinal)
            {
                return default(DateTime);
            }

            public override string GetString(int ordinal)
            {
                return default(string);
            }

            public override decimal GetDecimal(int ordinal)
            {
                return default(decimal);
            }

            public override double GetDouble(int ordinal)
            {
                return default(double);
            }

            public override float GetFloat(int ordinal)
            {
                return default(float);
            }

            public override string GetName(int ordinal)
            {
                return null;
            }

            public override int GetValues(object[] values)
            {
                return 0;
            }

            public override bool IsDBNull(int ordinal)
            {
                return true;
            }

            public override int FieldCount
            {
                get { return int.MaxValue; } //main fix
            }

            public override object this[int ordinal]
            {
                get { return null; }
            }

            public override object this[string name]
            {
                get { return null; }
            }

            public override bool HasRows
            {
                get { return false; }
            }

            public override int GetOrdinal(string name)
            {
                return default(int);
            }

            public override string GetDataTypeName(int ordinal)
            {
                return default(string);
            }

            public override Type GetFieldType(int ordinal)
            {
                return typeof(Object);
            }

            public override object GetValue(int ordinal)
            {
                return new object();
            }

            public override IEnumerator GetEnumerator()
            {
                return new object[0].GetEnumerator();
            }
        }
        /// <summary>
        /// Interceptor fix
        /// </summary>
        private class EntityFixInterceptor : IDbCommandInterceptor
        {


            public void NonQueryExecuting(
                DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
            {
            }

            public void NonQueryExecuted(
                DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
            {
            }

            public void ReaderExecuting(
                DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
            {

            }

            public void ReaderExecuted(
                DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
            {
                ChangeReader(interceptionContext);
            }

            public void ScalarExecuting(
                DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
            {

            }

            public void ScalarExecuted(
                DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
            {

            }

            private void ChangeReader<TResult>(DbCommandInterceptionContext<TResult> interceptionContext)
            {
                var r = interceptionContext.Result as SqlDataReader;
                if (r != null && r.VisibleFieldCount == 0)  //if data reder is empty — return fixed reader
                {
                    interceptionContext.Result = (TResult)(object)new EntityFixDataReader(r);
                }
            }
        }
    }

}
