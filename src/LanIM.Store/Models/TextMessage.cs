using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Store.Models
{
    public class TextMessage : Message
    {
        public string Text { get; set; }

        public TextMessage(string text)
        {
            this.Text = text;
        }

        public override string ToString()
        {
            return "content:" + this.Text;
        }
    }
}
