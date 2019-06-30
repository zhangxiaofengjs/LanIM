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
        public List<Contacter> Query()
        {
            DataTable dt = LanIMStore.Instance.Query(Sql.QUERY_CONTACTER);

            ModelConvert<Contacter> convert = new ModelConvert<Contacter>();
            IInstanceCreater<Contacter> ic = new CommonInstanceCreater<Contacter>();
            List<Contacter> cs = convert.Convert(dt, ic,
                new ColumnMapping("C_ID", "ID"),
                new ColumnMapping("C_NICK_NAME", "NickName"),
                new ColumnMapping("C_MAC", "MAC"),
                new ColumnMapping("C_IP", "IP"));
            return cs;
        }
    }
}
