﻿using Com.LanIM.Common;
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

        public static bool Initialize()
        {
            try
            {
                bool initDb = false;
                string dbPath = LanConfig.Instance.DbPath;
                if (!File.Exists(dbPath))
                {
                    SQLiteConnection.CreateFile(dbPath);
                    initDb = true;
                }

                SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
                builder.DataSource = dbPath;
                //builder.Password = "mimal";

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
            return true;
        }

        internal DataTable Query(string sql)
        {
            SQLiteCommand cmd = _conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Connection = _conn;
            cmd.CommandType = CommandType.Text;

            SQLiteDataAdapter adpter = new SQLiteDataAdapter(cmd);
            DataTable dt = new DataTable();
            adpter.Fill(dt);
            return dt;
        }

        internal void ExecuteNoQuery(string sql, SQLiteParameter[] parameters = null)
        {
            SQLiteCommand cmd = _conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Connection = _conn;
            cmd.CommandType = CommandType.Text;

            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            cmd.ExecuteNonQuery();
        }
    }
}
