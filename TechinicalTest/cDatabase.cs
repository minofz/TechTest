using System;
using System.Data;
using System.Data.SQLite;

namespace TechinicalTest
{
    public partial class MappingURL
    {
        public int Id { get; set; }
        public string BaseURL { get; set; }
        public string LocatedURL { get; set; }
    }
    internal class cDatabase
    {
        private static SQLiteConnection sqliteConnection;

        public cDatabase()
        { }

        private static SQLiteConnection DbConnection()
        {
            sqliteConnection = new SQLiteConnection($@"Data Source={AppDomain.CurrentDomain.BaseDirectory}/URLdata.sqlite; Version=3;");
            sqliteConnection.Open();
            return sqliteConnection;
        }
        public static void CreateDB()
        {
            try
            {
                SQLiteConnection.CreateFile($@"{AppDomain.CurrentDomain.BaseDirectory}/URLdata.sqlite");
            }
            catch
            {
                throw;
            }
        }
        public static void CreateTables()
        {
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS MappingURL(id integer primary key autoincrement, baseurl text, locatedurl text)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetListMappingURL(string pBaseURL = "")
        {
            SQLiteDataAdapter da = null;
            DataTable dt = new DataTable();
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = $"SELECT * FROM MappingURL {(pBaseURL != "" ? $"where baseurl = '{pBaseURL}'" : "")}";
                    da = new SQLiteDataAdapter(cmd.CommandText, DbConnection());
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static MappingURL GetMappingURL(int pId)
        {
            SQLiteDataAdapter da = null;
            DataTable dt = new DataTable();
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = $"SELECT * FROM MappingURL where id = {pId}";
                    da = new SQLiteDataAdapter(cmd.CommandText, DbConnection());
                    da.Fill(dt);

                    MappingURL vReturn = new MappingURL();

                    if (dt.Rows.Count > 0)
                    {
                        vReturn.Id = Convert.ToInt32(dt.Rows[0][0]);
                        vReturn.BaseURL = dt.Rows[0][1].ToString();
                        vReturn.LocatedURL = dt.Rows[0][2].ToString();
                    }
                    return vReturn;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void Add(MappingURL pReg)
        {
            try
            {
                if (pReg.Id == 0)
                {
                    using (var cmd = DbConnection().CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO MappingURL(baseurl, locatedurl ) values (@baseurl, @locatedurl)";
                        cmd.Parameters.AddWithValue("@baseurl", pReg.BaseURL);
                        cmd.Parameters.AddWithValue("@locatedurl", pReg.LocatedURL);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    Update(pReg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void Update(MappingURL pReg)
        {
            try
            {
                using (var cmd = new SQLiteCommand(DbConnection()))
                {
                    if (pReg.Id != 0)
                    {
                        cmd.CommandText = "UPDATE MappingURL SET baseurl=@baseurl, locatedurl=@locatedurl WHERE Id=@Id";
                        cmd.Parameters.AddWithValue("@Id", pReg.Id);
                        cmd.Parameters.AddWithValue("@baseurl", pReg.BaseURL);
                        cmd.Parameters.AddWithValue("@locatedurl", pReg.LocatedURL);
                        cmd.ExecuteNonQuery();
                    }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }      

    }
}
