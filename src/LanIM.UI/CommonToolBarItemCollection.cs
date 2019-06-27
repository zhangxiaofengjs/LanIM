using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.UI
{
    public class CommonToolBarItemCollection
    {
        private CommonToolBar _owner;
        private List<CommonToolBarItem> _list = new List<CommonToolBarItem>();

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public CommonToolBarItem this[int index]
        {
            get
            {
                return _list[index];
            }
        }

        public CommonToolBarItemCollection(CommonToolBar owner)
        {
            this._owner = owner;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (CommonToolBarItem item in _list)
            {
                yield return item;
            }
        }

        public void Add(CommonToolBarItem item)
        {
            this._list.Add(item);
            this._owner.MeasureItems();
        }

        public void AddRange(IEnumerable<CommonToolBarItem> collection)
        {
            this._list.AddRange(collection);
            this._owner.MeasureItems();
        }

        public void Remove(CommonToolBarItem item)
        {
            this._list.Remove(item);
            this._owner.MeasureItems();
        }

        public void RemoveAt(int index)
        {
            this._list.RemoveAt(index);
            this._owner.MeasureItems();
        }

        public void Clear()
        {
            this._list.Clear();
        }
    }
}
