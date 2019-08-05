using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.UI
{
    public delegate void MeasureItemEventHandler(object sender, MeasureItemEventArgs args);
    public delegate void ItemClickedEventHandler(object sender, ItemClickedEventArgs args);
    public delegate void ItemHoverEventHandler(object sender, ItemHoverEventArgs args);
    public delegate void SearchEventHandler(object sender, SearchEventArgs e);
}
