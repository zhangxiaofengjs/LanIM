using Com.LanIM.Store.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Com.LanIM.Components.MessageListItem;

namespace Com.LanIM.Components
{
    class MessageListBoxSelectionInfo
    {
        private MessageListItem _selectingTextItem = null;
        private int _selectStartDoIndex = -1;
        private int _selectEndDoIndex = -1;
        private List<TextBlockObj> _prevSelection = new List<TextBlockObj>();
        private bool HasSelection { get { return _selectingTextItem != null && _selectStartDoIndex != -1; } }
        private MessageListBox _owner;

        public MessageListBoxSelectionInfo(MessageListBox owner)
        {
            _owner = owner;
        }

        public void Clear()
        {
            if (_selectingTextItem != null)
            {
                foreach (DrawingObject dobj in _selectingTextItem.DrawingObjects)
                {
                    if (dobj.Type == DrawingObjectType.TextBlock)
                    {
                        TextBlockObj tb = dobj.Tag as TextBlockObj;
                        tb.ClearSelection();
                    }
                }
            }

            _selectingTextItem = null;
            _selectStartDoIndex = -1;
            _selectEndDoIndex = -1;
            _prevSelection.Clear();
        }

        internal void SetSelectionStartItem(Point p)
        {
            //清除其他的选择状态
            this.Clear();

            MessageListItem item = _owner.GetItemAtPosition(p) as MessageListItem;
            if (item == null)
            {
                return;
            }

            //设定文本选中
            if (item.Message.Type == MessageType.Text)
            {
                this._selectingTextItem = item;
                Rectangle rect = item.Bounds;

                for (int i = 0; i < this._selectingTextItem.DrawingObjects.Count; i++)
                {
                    DrawingObject dobj = item.DrawingObjects[i];
                    if (dobj.Type == DrawingObjectType.TextBlock)
                    {
                        Rectangle bounds = dobj.Offset(rect.X, rect.Y);

                        if (bounds.Contains(p))
                        {
                            TextBlockObj tb = dobj.Tag as TextBlockObj;
                            StringPart sp = tb.StringPart;
                            using (Graphics g = _owner.CreateGraphics())
                            {
                                tb.SelectionStart = StringMeasurer.GetCharIndex(g, sp.Font, p.X - (int)dobj.X, sp.String);
                                this._selectStartDoIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
        }

        internal void SetSelectionEndItem(Point location)
        {
            if (!this.HasSelection)
            {
                return;
            }

            MessageListItem hoverItem = _owner.GetItemAtPosition(location) as MessageListItem;
            if (hoverItem == null)
            {
                //鼠标移动到其他地方了，算了，不选了
                this.Clear();
                return;
            }

            if (hoverItem != this._selectingTextItem)
            {
                //当前的选择的不是上面一个
                this.Clear();
                return;
            }

            Rectangle rect = this._selectingTextItem.Bounds;
            for (int i = 0; i < this._selectingTextItem.DrawingObjects.Count; i++)
            {
                DrawingObject dobj = this._selectingTextItem.DrawingObjects[i];

                if (dobj.Type == DrawingObjectType.TextBlock)
                {
                    Rectangle bounds = dobj.Offset(rect.X, rect.Y);
                    TextBlockObj tb = dobj.Tag as TextBlockObj;

                    if (bounds.Contains(location))
                    {
                        StringPart sp = tb.StringPart;
                        using (Graphics g = _owner.CreateGraphics())
                        {
                            tb.SelectionEnd = StringMeasurer.GetCharIndex(g, sp.Font, location.X - (int)dobj.X, sp.String);
                            this._selectEndDoIndex = i;
                            break;
                        }
                    }
                }
            }

            //开始与结束直接的所有TextBlock都设定为全选中
            int startIndex = Math.Min(this._selectStartDoIndex, this._selectEndDoIndex);
            int endIndex = Math.Max(this._selectStartDoIndex, this._selectEndDoIndex);

            List<TextBlockObj> selObj = new List<TextBlockObj>();
            for (int i = startIndex; i <= endIndex; i++)
            {
                DrawingObject dobj = this._selectingTextItem.DrawingObjects[i];
                if (dobj.Type == DrawingObjectType.TextBlock)
                {
                    TextBlockObj tbObj = dobj.Tag as TextBlockObj;

                    if (this._selectStartDoIndex < this._selectEndDoIndex)
                    {
                        //从上往下选择时
                        if (i == this._selectStartDoIndex)
                        {
                            tbObj.SelectionEnd = tbObj.Length - 1;
                        }
                        else if (i == this._selectEndDoIndex)
                        {
                            tbObj.SelectionStart = 0;
                        }
                    }
                    else if (this._selectStartDoIndex > this._selectEndDoIndex)
                    {
                        //从下往上选择时
                        if (i == this._selectStartDoIndex)
                        {
                            tbObj.SelectionEnd = 0;
                        }
                        else if (i == this._selectEndDoIndex)
                        {
                            tbObj.SelectionStart = tbObj.Length - 1;
                        }
                    }
                    if (i != this._selectStartDoIndex && i != this._selectEndDoIndex)
                    {
                        tbObj.SelectAll();
                    }

                    selObj.Add(tbObj);
                    this._prevSelection.Remove(tbObj);
                }
            }

            //清除选择
            foreach (TextBlockObj item in this._prevSelection)
            {
                item.ClearSelection();
            }
            this._prevSelection = selObj;
        }
    }
}
