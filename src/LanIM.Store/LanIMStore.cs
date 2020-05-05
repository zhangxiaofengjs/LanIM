using Com.LanIM.Common;
using Com.LanIM.Common.Logger;
using Com.LanIM.Store.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.Store
{
    public class LanIMStore
    {
        private static LanIMStore _instance = null;
        internal static LanIMStore Instance
        {
            get
            {
                return _instance;
            }
        }

        private SQLiteConnection _conn = null;

        public SQLiteConnection Conn { get => _conn; set => _conn = value; }

        public LanIMStore()
        {
        }

        public static bool Initialize(string dbPath)
        {
            try
            {
                bool initDb = false;
                if (!File.Exists(dbPath))
                {
                    SQLiteConnection.CreateFile(dbPath);
                    initDb = true;
                }

                SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
                builder.DataSource = dbPath;
                //builder.Password = "minal";

                _instance = new LanIMStore();
                _instance.Conn = new SQLiteConnection(builder.ConnectionString);
                _instance.Conn.Open();

                if (initDb)
                {
                    _instance.ExecuteNoQuery(Sql.CREATE_DB);
                }
                return true;
            }
            catch (Exception e)
            {
                LoggerFactory.Error("打开消息数据库错误。{0}", e);
            }

            return false;
        }

        public static bool UnInitialize()
        {
            _instance.Conn.Close();
            return true;
        }

        internal DataTable Query(string sql, params SQLiteParameter[] parameters)
        {
            try
            {
                SQLiteCommand cmd = CreateCommand(sql, parameters);

                SQLiteDataAdapter adpter = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                adpter.Fill(dt);
                return dt;
            }
            catch(Exception e)
            {
                LoggerFactory.Error("Query错误:", e);
            }
            return null;
        }

        private SQLiteCommand CreateCommand(string sql, SQLiteParameter[] parameters)
        {
            SQLiteCommand cmd = _conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Connection = _conn;
            cmd.CommandType = CommandType.Text;

            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            return cmd;
        }

        internal object ExecuteScalar(string sql, params SQLiteParameter[] parameters)
        {
            try
            {
                SQLiteCommand cmd = CreateCommand(sql, parameters);
                object obj = cmd.ExecuteScalar();
                if (obj is DBNull)
                {
                    return null;
                }
                return obj;
            }
            catch (Exception e)
            {
                LoggerFactory.Error("ExecuteScalar错误:", e);
            }
            return null;
        }

        internal void Delete(string sql, params SQLiteParameter[] parameters)
        {
            ExecuteNoQuery(sql, parameters);
        }
        internal void Update(string sql, params SQLiteParameter[] parameters)
        {
            ExecuteNoQuery(sql, parameters);
        }
        private void ExecuteNoQuery(string sql, params SQLiteParameter[] parameters)
        {
            try
            {
                SQLiteCommand cmd = CreateCommand(sql, parameters);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                LoggerFactory.Error("ExecuteNoQuery错误:", e);
            }
        }

        /// <summary>
        /// 插入返回最后一条记录ID
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal int Insert(string sql, params SQLiteParameter[] parameters)
        {
            try
            {
                SQLiteCommand cmd = CreateCommand(sql + ";select last_insert_rowid();", parameters);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                LoggerFactory.Error("Insert错误:", e);
            }
            return -1;
        }
    }
}
