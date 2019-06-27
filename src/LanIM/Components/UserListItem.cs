using Com.LanIM.Components;
using Com.LanIM.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Components
{
    internal class UserListItem : ScrollableListItem
    {
        public UserListItem()
        {
        }

        public UserListItem(LanUser user)
        {
            this.User = user;
        }

        public LanUser User { get; set; }

        public override string ToString()
        {
            return User.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(obj is LanUser)
            {
                return (obj as LanUser).ID == this.User.ID;
            }
            return base.Equals(obj);
        }
    }
}
