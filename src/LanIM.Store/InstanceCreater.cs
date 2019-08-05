using Com.LanIM.Store.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Store
{
    interface IInstanceCreater<T>
    {
        T CreateInstance(DataRow row);
    }

    class CommonInstanceCreater<T> : IInstanceCreater<T>
    {
        public T CreateInstance(DataRow row)
        {
            T t = Activator.CreateInstance<T>();
            return t;
        }
    }

    class MessageInstanceCreater : IInstanceCreater<Message>
    {
        private string _typeColumnName;

        public MessageInstanceCreater(string typeColumnName)
        {
            this._typeColumnName = typeColumnName;
        }

        public Message CreateInstance(DataRow row)
        {
            int type = (int)row[this._typeColumnName];
            if((MessageType)type == MessageType.Text)
            {
                return new Message(MessageType.Text);
            }
            else if((MessageType)type == MessageType.Image)
            {
                return new ImageMessage();
            }
            else if ((MessageType)type == MessageType.File)
            {
                return new FileMessage();
            }
            return null;
        }
    }
}
