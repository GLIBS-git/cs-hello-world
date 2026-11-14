using Dapper;
using Glibs.SqlTools;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Exercises
{
    internal class TestSql
    {
        public static string? connectionString;
        public static void Starter()
        {
            TestSql.connectionString = SqlUtils.ConnectionStringIntegratedSecurity(Program.configuration["ConnectionSettings:SqlServer"] ?? "",
                Program.configuration["ConnectionSettings:SqlServerDb"] ?? "", false);
            //TestSql.Test();
            //TestSql.TestRecord();
            //TestSql.TestRecord_2();
            TestSql.TestRecord_2_1();
            //TestSql.TestRecord_3().Wait();
            //TestSql.TestRecord_3_1().Wait();
            //TestSql.TestRecord_3_2().Wait();
        }
        public static void TestRecord_2()
        {
            using (IDbConnection connection = new SqlConnection(TestSql.connectionString))
            {
                string sqlQuery = $"SELECT {RecordUtils.AllFieldsList<Product>()} FROM [{Product.TableSqlName()}]";
                Console.WriteLine(sqlQuery);
                IEnumerable<Product>? products = connection.Query<Product>(sqlQuery, buffered: false);
                products?.ToList().ForEach(_p => Console.WriteLine($"{_p.Id} -- {_p.Code} -- {_p.Description} -- {_p.Weight} -- {_p.GroupId}"));
            }
        }
        public static void TestRecord_2_1()
        {
            using (IDbConnection connection = new SqlConnection(TestSql.connectionString))
            {
                string sqlQuery = $"SELECT {RecordUtils.AllFieldsList<ProductGroup>()} FROM [{ProductGroup.TableSqlName()}]"; //  with (repeatableread)
                Console.WriteLine(sqlQuery);
                IEnumerable<ProductGroup>? products = connection.Query<ProductGroup>(sqlQuery, buffered: false, commandTimeout: 0);
                products?.ToList().ForEach(_p => Console.WriteLine($"{_p.Id} -- {_p.Code} -- {_p.Description}"));
            }
        }
        public static async Task TestRecord_3()
        {
            Product productIns = new Product() { Code = "000007", Description = "Laptop Samsung", Weight = 1, GroupId = 2 };
            using (IDbConnection connection = new SqlConnection(TestSql.connectionString))
            {
                try
                {
                    Console.WriteLine(await productIns.Insert(connection));
                }
                catch (SqlException _)
                {
                    Console.WriteLine("Insert failed...");
                }
            }
        }
        public static async Task TestRecord_3_1()
        {
            ProductGroup productGroupIns = new ProductGroup() { Code = "000001", Description = "Cars" };
            using (IDbConnection connection = new SqlConnection(TestSql.connectionString))
            {
                try
                {
                    Console.WriteLine(await productGroupIns.Insert(connection));
                }
                catch (SqlException _e)
                {
                    Console.WriteLine(_e.Message);
                    Console.WriteLine(_e.StackTrace);
                    Console.WriteLine("Insert failed...");
                }
            }
        }
        public record ProductGroupIns ( string Code, string Description) : IView;
        public static async Task TestRecord_3_2()
        {
            ProductGroupIns productGroupIns = new ProductGroupIns("000003", "Phones" );
            using (IDbConnection connection = new SqlConnection(TestSql.connectionString))
            {
                try
                {
                    Console.WriteLine(await productGroupIns.Insert(connection, ProductGroup.TableSqlName()));
                }
                catch (SqlException _e)
                {
                    Console.WriteLine(_e.Message);
                    Console.WriteLine(_e.StackTrace);
                    Console.WriteLine("Insert failed...");
                }
            }
        }
        public static void Test()
        {
            //Product product = new Product();
            //Console.WriteLine(RecordUtils.AllFieldsList(product));
            //product.Code = "000009";
            //Product product_1 = new Product() { Code= "000008" };
            //Product product_1_1 = new Product()
            //{
            //    Code = "000008",
            //    Description = "Car BMW i3",
            //    Weight = 1500,
            //    GroupId = 1,
            //};
            //Console.WriteLine(typeof(Product_9));
            //public record Product_4(int? Id, string? Name) : IView;
            //var product = new Product_2(1, "123");
            //Console.WriteLine(Product.tableSqlName);
            //Console.WriteLine(product.tableSqlName());
            //Console.WriteLine(((IDapperTools)product).AllFieldsInsert());
            //Console.WriteLine(RecordUtils.AllFieldsList<Product>());
            //Console.WriteLine(RecordUtils.AllFieldsList<Product_2>());
            //Console.WriteLine(RecordUtils.AllFieldsList<Product_3>());
            //Console.WriteLine(RecordUtils.AllFieldsParams(product));
            //Console.WriteLine("Properties:");
            //typeof(Product).GetProperties().Select(pi => pi.Name).ToList().ForEach(_p => Console.WriteLine($"    {_p}"));
            //Console.WriteLine();
            //Console.WriteLine("Methods:");
            //typeof(Product).GetMethods().Select(pi => pi.Name).ToList().ForEach(_p => Console.WriteLine($"    {_p}"));
            //Console.WriteLine();
            //Console.WriteLine("Variables:");
            //typeof(Product).GetFields().Select(pi => pi.Name).ToList().ForEach(_p => Console.WriteLine($"    {_p}"));
        }
        public static void TestRecord()
        {
            //Product product = new Product(0,"000001","Car BMW i7", 1800, 1);
            //Product product = new Product();
            IEnumerable<Product>? products = null;
            using (IDbConnection connection = new SqlConnection(TestSql.connectionString))
            {
                string sqlQuery = $"SELECT {RecordUtils.AllFieldsList<Product>()} FROM [{Product.TableSqlName()}]";
                Console.WriteLine(sqlQuery);
                products = connection.Query<Product>(sqlQuery);
            }
            products.ToList().ForEach(_p => Console.WriteLine($"{_p.Id} -- {_p.Code} -- {_p.Description} -- {_p.Weight} -- {_p.GroupId}"));
            Console.WriteLine();
            using (IDbConnection connection = new SqlConnection(TestSql.connectionString))
            {
                string sqlQuery = $"SELECT {RecordUtils.AllFieldsList<Product>()} FROM [{Product.TableSqlName()}]";
                Console.WriteLine(sqlQuery);
                products = connection.Query<Product>(sqlQuery, buffered:false);
                products.ToList().ForEach(_p => Console.WriteLine($"{_p.Id} -- {_p.Code} -- {_p.Description} -- {_p.Weight} -- {_p.GroupId}"));
            }
            Console.WriteLine();
            using (IDbConnection connection = new SqlConnection(TestSql.connectionString))
            {
                string sqlQuery = $"SELECT {RecordUtils.AllFieldsList<Product>()} FROM [{Product.TableSqlName()}] WHERE [Id] = @Id";
                Console.WriteLine(sqlQuery);
                products = connection.Query<Product>(sqlQuery, new { Id = 3});
            }
            products.ToList().ForEach(_p => Console.WriteLine($"{_p.Id} -- {_p.Code} -- {_p.Description} -- {_p.Weight} -- {_p.GroupId}"));
            using (IDbConnection connection = new SqlConnection(TestSql.connectionString))
            {
                string sqlQuery = $"SELECT {RecordUtils.AllFieldsList<Product>()} FROM [{Product.TableSqlName()}] WHERE [Id] = @Id";
                Console.WriteLine(sqlQuery);
                products = connection.Query<Product>(sqlQuery, new { Id = 2 });
            }
            products.ToList().ForEach(_p => Console.WriteLine($"{_p.Id} -- {_p.Code} -- {_p.Description} -- {_p.Weight} -- {_p.GroupId}"));
        }
    }
    public record Product_2(int Id, string Description) : IView;
    //public record Product(int? Id = 0, string? Code = "", string? Description = "", decimal? Weight = 0, int? GroupId = 0) : IRecord
    public record Product : IRecord
    {
        public int? Id { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public decimal? Weight { get; set; }
        public int? GroupId { get; set; }
        public static string IdentityField() => "Id";
        public static string[] PrimaryKey() => ["Id"];
        public static string TableSqlName() => "Product";
        public string GetIdentityField() => Product.IdentityField();
        public string[] GetPrimaryKey() => Product.PrimaryKey();
        public string GetTableSqlName() => Product.TableSqlName();
        public static IEnumerable<Product> Find(int _id, IDbConnection _connection, IDbTransaction? _transaction = null)
        {
            string sqlQuery = SelectBuilder.Create(Product.TableSqlName())
                .TopN(1)
                .SetFieldsList(RecordUtils.AllFieldsList<Product>())
                .SetWhereCriterias($"[Id] = @Id")
                .Build();
            Console.WriteLine(sqlQuery);
            return _connection.Query<Product>(sqlQuery, new { @Id = _id }, transaction: _transaction, buffered: true);
        }
        public static IEnumerable<Product> Find2(int _id, IDbConnection _connection, IDbTransaction? _transaction = null)
        {
            string sqlQuery = @$"SELECT TOP 1 {RecordUtils.AllFieldsList<Product>()} FROM [{Product.TableSqlName()}] WHERE [Id] = @Id;";
            Console.WriteLine(sqlQuery);
            return _connection.Query<Product>(sqlQuery, new { @Id = _id }, transaction: _transaction, buffered: true);
        }
        public static int Delete(int _id, IDbConnection _connection, IDbTransaction? _transaction = null)
        {
            string sqlCommand = @$"DELETE FROM [{Product.TableSqlName()}] WHERE [Id] = @Id;";
            Console.WriteLine(sqlCommand);
            return _connection.Execute(sqlCommand, new { @Id = _id }, transaction: _transaction);
        }
        public static async Task<IEnumerable<Product>> FindAsync(int _id, IDbConnection _connection, IDbTransaction? _transaction = null)
        {
            string sqlQuery = SelectBuilder.Create(Product.TableSqlName())
                .TopN(1)
                .SetFieldsList(RecordUtils.AllFieldsList<Product>())
                .SetWhereCriterias($"[Id] = @Id")
                .Build();
            Console.WriteLine(sqlQuery);
            return await _connection.QueryAsync<Product>(sqlQuery, param: new { @Id = _id }, _transaction);
        }
        public static async Task<int> DeleteAsync(int _id, IDbConnection _connection, IDbTransaction? _transaction = null)
        {
            string sqlCommand = @$"DELETE FROM [{Product.TableSqlName()}] WHERE [Id] = @Id;";
            Console.WriteLine(sqlCommand);
            return await _connection.ExecuteAsync(sqlCommand, new { @Id = _id }, transaction: _transaction);
        }
    }
    public record ProductGroup : IRecord
    {
        public int? Id { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public static string IdentityField() => "Id";
        public static string[] PrimaryKey() => ["Id"];
        public static string TableSqlName() => "ProductGroup";
        public string GetIdentityField() => ProductGroup.IdentityField();
        public string[] GetPrimaryKey() => ProductGroup.PrimaryKey();
        public string GetTableSqlName() => ProductGroup.TableSqlName();
        public static ProductGroup Find(int _id, IDbConnection _connection, IDbTransaction? _transaction = null)
        {
            return new ProductGroup();
        }
    }
    public record Product_v(int Id = 9, string Code = "123456", string Description = "jgfgvjdj", decimal Weight = 7, int GroupId = 9) : IView;
}
