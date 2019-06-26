using Com.LanIM.Store.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Store
{
    public class ColumnMapping
    {
        public string Column { get; set; }
        public string Property { get; set; }

        public ColumnMapping(string column, string property)
        {
            this.Column = column;
            this.Property = property;
        }
    }
}
