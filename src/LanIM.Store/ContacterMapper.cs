using Com.LanIM.Store.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Store
{
    public class ContacterMapper
    {
        public List<Contacter> Query()
        {
            DataTable dt = LanIMStore.Instance.Query(Sql.QUERY_CONTACTER);

            ModelConvert<Contacter> convert = new ModelConvert<Contacter>();
            IInstanceCreater<Contacter> ic = new CommonInstanceCreater<Contacter>();
            List<Contacter> cs = convert.Convert(dt, ic,
                new ColumnMapping("C_ID", "ID"),
                new ColumnMapping("C_NICK_NAME", "NickName"),
                new ColumnMapping("C_MAC", "MAC"),
                new ColumnMapping("C_IP", "IP"),
                new ColumnMapping("C_PORT", "Port"),
                new ColumnMapping("C_MEMO", "Memo"));
            return cs;
        }

        public void Add(Contacter c)
        {
            ModelConvert<Contacter> convert = new ModelConvert<Contacter>();

            SQLiteParameter[] parameters = convert.CreateParameters(c,
                new ColumnMapping("C_NICK_NAME", "NickName"),
                new ColumnMapping("C_MAC", "MAC"),
                new ColumnMapping("C_IP", "IP"),
                new ColumnMapping("C_PORT", "Port"));

            int id = LanIMStore.Instance.Insert(Sql.ADD_CONTACTER, parameters);
            c.ID = id;
        }

        public void UpdateMemo(Contacter c)
        {
            ModelConvert<Contacter> convert = new ModelConvert<Contacter>();

            SQLiteParameter[] parameters = convert.CreateParameters(c,
                new ColumnMapping("C_MAC", "MAC"),
                new ColumnMapping("C_MEMO", "Memo"));

            LanIMStore.Instance.Update(Sql.UPDATE_CONTACTER_MEMO, parameters);
        }

        public void Update(Contacter m)
        {
            ModelConvert<Contacter> convert = new ModelConvert<Contacter>();

            SQLiteParameter[] parameters = convert.CreateParameters(m,
                new ColumnMapping("C_NICK_NAME", "NickName"),
                new ColumnMapping("C_MAC", "MAC"),
                new ColumnMapping("C_PORT", "Port"),
                new ColumnMapping("C_IP", "IP"));

            LanIMStore.Instance.Update(Sql.UPDATE_CONTACTER, parameters);
        }
    }
}
