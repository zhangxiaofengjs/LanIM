using Com.LanIM.Store.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Store
{
    public class ContacterMapper
    {
        public List<Contacter> QueryList()
        {
            DataTable dt = LanIMStore.Instance.Query(Sql.QUERY_CONTACTERS);

            DataTable2ModelConvert<Contacter> convert = new DataTable2ModelConvert<Contacter>();

            List<Contacter> cs = convert.Convert(dt,
                new ColumnMapping("C_ID", "ID"),
                new ColumnMapping("C_NICK_NAME", "NickName"),
                new ColumnMapping("C_MAC", "MAC"),
                new ColumnMapping("C_IP", "IP"));
            return cs;
        }
    }
}
