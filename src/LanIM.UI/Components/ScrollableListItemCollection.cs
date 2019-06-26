using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.UI.Components
{
    internal class ScrollableListItemCollection
    {
        private ScrollableList _owner;
        private List<ScrollableListItem> _list = new List<ScrollableListItem>();

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public ScrollableListItem this[int index]
        {
            get
            {
                return _list[index];
            }
        }

        public ScrollableListItemCollection(ScrollableList owner)
        {
            this._owner = owner;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (ScrollableListItem item in _list)
            {
                yield return item;
            }
        }

        public void Add(ScrollableListItem item)
        {
            this._list.Add(item);
            this._owner.MeasureItemOnAdd(item);
            this._owner.Invalidate();
        }

        public void AddRange(IEnumerable<ScrollableListItem> collection)
        {
            this._list.AddRange(collection);
            this._owner.MeasureItemOnAdd(collection);
            this._owner.Invalidate();
        }

        public void Remove(ScrollableListItem item)
        {
            this._list.Remove(item);
            this._owner.MeasureItemOnRemove(item);
            this._owner.Invalidate();
        }

        public void RemoveAt(int index)
        {
            ScrollableListItem item = this._list[index];
            this._list.RemoveAt(index);
            this._owner.MeasureItemOnRemove(item);
            this._owner.Invalidate();
        }

        public void Clear()
        {
            this._list.Clear();
            this._owner.MeasureItems();
            this._owner.Invalidate();
        }

        public int IndexOf(ScrollableListItem item)
        {
            return this._list.IndexOf(item);
        }
    }
}
