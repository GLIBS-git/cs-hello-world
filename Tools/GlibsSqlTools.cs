using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace Glibs.SqlTools
{
    public interface IView
    {
    }
    public interface IRecord : IView
    {
        public abstract static string TableSqlName();
        public abstract string GetTableSqlName();
        public abstract static string IdentityField();
        public abstract string GetIdentityField();
        public abstract static string[] PrimaryKey();
        public abstract string[] GetPrimaryKey();
        //public string[] AllFields() => this.GetType().GetProperties().Select(pi => pi.Name).ToArray();
        //public string AllFieldsInsert() => string.Join(",", this.AllFields());
        //public string AllFieldsParams() => string.Join(",", this.GetType().GetProperties().Select(pi => "@" + pi.Name).ToArray());
    }
    public static class RecordUtils
    {
        public static string[] AllFieldsArr(IView _view) => _view.GetType().GetProperties().Select(_pi => _pi.Name).ToArray();
        public static string[] AllFieldsArr<T>() where T : IView => typeof(T).GetProperties().Select(_pi => _pi.Name).ToArray();
        public static string[] AllFieldsInsArr(IRecord _record)
        {
            HashSet<string> fields = RecordUtils.AllFieldsArr(_record).ToHashSet();
            fields.ExceptWith(new HashSet<string>([_record.GetIdentityField()]));
            return [.. fields];
        }
        public static string AllFieldsInsList(IRecord _record) => string.Join(",", RecordUtils.AllFieldsInsArr(_record).Select(_p => $"[{_p}]"));
        public static string AllFieldsInsValues(IRecord _record) => string.Join(",", RecordUtils.AllFieldsInsArr(_record).Select(_p => $"@{_p}"));
        public static string AllFieldsList(IView _view) => string.Join(",", _view.GetType().GetProperties().Select(_pi => $"[{_pi.Name}]"));
        public static string AllFieldsList<T>() where T : IView => string.Join(",", typeof(T).GetProperties().Select(_pi => $"[{_pi.Name}]"));
        public static async Task<int> Insert(this IRecord _record, IDbConnection _connection, IDbTransaction? _transaction = null)
        {
            string sqlQuery = $"INSERT INTO [{_record.GetTableSqlName()}] ({RecordUtils.AllFieldsInsList(_record)}) VALUES ({RecordUtils.AllFieldsInsValues(_record)});";
            //Console.WriteLine(sqlQuery);
            //return -1;
            return await _connection.ExecuteAsync(sqlQuery, _record, _transaction);
        }
        public static async Task<int> Insert(this IView _view, IDbConnection _connection, string _tableName, IDbTransaction? _transaction = null)
        {
            string sqlQuery = $"INSERT INTO [{_tableName}] ({RecordUtils.AllFieldsList(_view)}) VALUES ({RecordUtils.AllFieldsValues(_view)});";
            return await _connection.ExecuteAsync(sqlQuery, _view, _transaction);
        }
        public static string AllFieldsValues(IView _view) => string.Join(",", _view.GetType().GetProperties().Select(_pi => $"@{_pi.Name}"));
        public static string AllFieldsValues<T>() where T : IView => string.Join(",", typeof(T).GetProperties().Select(_pi => $"@{_pi.Name}"));
    }
    public class SelectBuilder
    {
        private string tableName;
        private int topN = -1;
        private string fieldsList = "";
        private string whereCriterias = "";
        public SelectBuilder(string _tableName) => this.tableName = _tableName;
        public static SelectBuilder Create(string _tableName) => new SelectBuilder(_tableName);
        public SelectBuilder TopN(int _topN)
        {
            if (_topN < 1)
                throw new ArgumentException("Value for TOP <N> must be positive!");
            this.topN = _topN;
            return this;
        }
        public SelectBuilder SetFieldsList(string _fieldsList)
        {
            this.fieldsList = _fieldsList;
            return this;
        }
        public SelectBuilder SetWhereCriterias(string _whereCriteriass)
        {
            this.whereCriterias = _whereCriteriass;
            return this;
        }
        public string Build()
        {
            StringBuilder sb = new StringBuilder("SELECT");
            if (this.topN > 0) sb.Append($" TOP {this.topN}");
            if (string.IsNullOrEmpty(this.fieldsList))
            {
                sb.Append(" *");
            }
            else
                sb.Append($" {this.fieldsList}");
            sb.Append($" FROM {this.tableName}");
            if (!string.IsNullOrEmpty(this.whereCriterias))
                sb.Append($" WHERE {this.whereCriterias}");
            //sb.Append(";");
            return sb.ToString();
        }
    }
    public static class SqlUtils
    {
        public static string ConnectionStringIntegratedSecurity(string _server, string _db, bool _encrypt = true, bool _trustCert = true, int _timeout = 0, bool _failover = false)
        {
            if (string.IsNullOrEmpty(_server)) 
                throw new ArgumentException("No server name provided!");
            if (string.IsNullOrEmpty(_db)) 
                throw new ArgumentException("No database name provided!");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = _server, // Server\Instance
                InitialCatalog = _db, // DB name
                IntegratedSecurity = true, // Windows domain authentication
                Encrypt = _encrypt, // Encription
                TrustServerCertificate = _trustCert, // Trust certificate
                //ConnectTimeout = _timeout, // Connection only timeout, seconds
                MultiSubnetFailover = _failover, // Faiover cluster 
                ApplicationName = "Glibs learning test application",
            };
            if (_timeout > 0)
                builder.ConnectTimeout = _timeout;
            return builder.ConnectionString;
        }
        public static string ConnectionStringIntegratedSecurity(string _server, string _db, bool _failover = false)
        {
            return SqlUtils.ConnectionStringIntegratedSecurity(_server, _db, true, true, 0 , _failover);
        }
        public static string ConnectionStringIntegratedSecurity(string _server, string _db, int _timeout, bool _failover = false)
        {
            return SqlUtils.ConnectionStringIntegratedSecurity(_server, _db, true, true, _timeout, _failover);
        }
        public static string ConnectionStringSqlLogin(string _server, string _db, string _login, string _password, bool _encrypt, bool _trustCert, int _timeout = 0, bool _failover = false)
        {
            if (string.IsNullOrEmpty(_server))
                throw new ArgumentException("No server name provided!");
            if (string.IsNullOrEmpty(_db))
                throw new ArgumentException("No database name provided!");
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = _server, // Server\Instance
                InitialCatalog = _db, // DB name
                UserID = _login, // Login
                Password = _password, // Password
                Encrypt = _encrypt, // Encription
                TrustServerCertificate = _trustCert, // Trust certificate
                //ConnectTimeout = _timeout, // Connection only timeout, seconds
                MultiSubnetFailover = _failover,
                ApplicationName = "Glibs learning test application",
            };
            if (_timeout > 0)
                builder.ConnectTimeout = _timeout;
            return builder.ConnectionString;
        }
        public static string ConnectionStringSqlLogin(string _server, string _db, string _login, string _password)
        {
            return SqlUtils.ConnectionStringSqlLogin(_server, _db, _login, _password, true, true);
        }
        public static string ConnectionStringSqlLogin(string _server, string _db, string _login, string _password, int _timeout)
        {
            return SqlUtils.ConnectionStringSqlLogin(_server, _db, _login, _password, true, true, _timeout);
        }
    }
}
