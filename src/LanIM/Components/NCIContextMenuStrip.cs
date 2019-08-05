using Com.LanIM.Common.Network;
using Com.LanIM.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.Components
{
    class NCIContextMenuStrip : ContextMenuStrip
    {
        public event NCIInfoEventHandler NCIInfoSelected = null;

        public NCIContextMenuStrip(IContainer c)
            : base(c)
        {
            this.Renderer = new CommonContextMenuRender();
        }

        protected override void OnOpening(CancelEventArgs e)
        {
            base.OnOpening(e);

            List<NCIInfo> nciInfos = NCIInfo.GetNICInfo(NCIType.Physical | NCIType.Wireless);
            if (nciInfos.Count == 0)
            {
                e.Cancel = true;
                return;
            }

            this.Items.Clear();

            foreach (NCIInfo nciInfo in nciInfos)
            {
                ToolStripMenuItem item = this.Items.Add(nciInfo.Name) as ToolStripMenuItem;
                item.Tag = nciInfo;
                item.Click += Item_Click;
            }
        }

        private void Item_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            NCIInfoSelected?.Invoke(this, new NCIInfoEventArgs(item.Tag as NCIInfo));
        }
    }
}
