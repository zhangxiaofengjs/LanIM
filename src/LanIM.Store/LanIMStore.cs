using Com.LanIM.Common.Logger;
using Com.LanIM.Store.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Store
{
    public class LanIMStore
    {
        private static readonly LanIMStore _instance = new LanIMStore();

        private SQLiteConnection _conn = null;

        public LanIMStore()
        {
        }

        public static bool Initialize()
        {
            try
            {
                string strDb = "lanim.db";
                if (!File.Exists(strDb))
                {
                    StoreSchemaCreater.Execute(strDb);
                }

                _instance._conn = new SQLiteConnection(strDb);
                _instance._conn.Open();
                return true;
            }
            catch(Exception e)
            {
                LoggerFactory.Error("打开消息数据库错误。{0}", e);
            }

            return false;
        }

        public static bool UnInitialize()
        {
            return true;
        }
    }
}
