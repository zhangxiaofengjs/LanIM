using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Com.LanIM.UI.Components
{
    class ScrollableList : UserControl
    {
        private static readonly Pen BORDER_PEN = new Pen(Color.FromArgb(214, 214, 214));

        class ItemInfo
        {
            public int Index = -1;
            public Rectangle Bounds = Rectangle.Empty;
            public ScrollableListItem Item = null;
        }

        private const int SCROLLBAR_WIDTH = 8;
        private const int MIN_SCROLLBAR_HEIGHT = (int)(SCROLLBAR_WIDTH * 1.5);
        private const int DEFAULT_MOUSE_WHEEL_OFFSET = 20;

        private static readonly Brush SCROLLBAR_HANDLE_BRUSH_NORMAL = new SolidBrush(Color.FromArgb(200, 210, 210, 210));
        private static readonly Brush SCROLLBAR_HANDLE_BRUSH_FOCUSED = new SolidBrush(Color.FromArgb(200, 186, 186, 186));

        private Rectangle _scrollBarHandleBounds = Rectangle.Empty;//滑块位置大小
        private Brush _scrollBarHandleBrush = SCROLLBAR_HANDLE_BRUSH_NORMAL;//滑块绘制笔刷

        private int _offset = 0;
        private int Offset
        {
            get
            {
                return this._offset;
            }
            set
            {
                if (this.ClientSize.Height >= this._totleItemHeight)
                {
                    this._offset = 0;
                }
                else
                {
                    //移动到第一个项目
                    this._offset = Math.Min(value, 0);
                    //移动到最后一个项目，最多this.ClientSize.Height - this._totleItemHeight
                    this._offset = Math.Max(this._offset, this.ClientSize.Height - this._totleItemHeight);
                }
                //计算滑块位置
                int handleOffset = -(int)(1.0 * this._offset * (this.ClientSize.Height - this._scrollBarHandleBounds.Height) / (this._totleItemHeight - this.ClientSize.Height));
                this._scrollBarHandleBounds.Y = handleOffset;
            }
        }

        public Borders Borders { get; set; }

        private int _totleItemHeight = 0;
        private int TotleItemHeight
        {
            get
            {
                return _totleItemHeight;
            }
            set
            {
                _totleItemHeight = value;

                //滑块高度
                this._scrollBarHandleBounds.X = this.ClientSize.Width - SCROLLBAR_WIDTH - 1;
                this._scrollBarHandleBounds.Height = Math.Max((int)(this.ClientSize.Height * this.ClientSize.Height * 1.0 / _totleItemHeight), MIN_SCROLLBAR_HEIGHT);
            }
        }
        private bool _mouseEnter = false;
        private bool _scrollBarHandleDrag = false;
        private int _scrollBarHandleDragMouseOffsetY = -1;

        [Browsable(false)]
        public ScrollableListItemCollection Items { get; set; }

        private SortedList<int, ScrollableListItem> _selectedIndexes = new SortedList<int, ScrollableListItem>();
        private ScrollableListItemCollection _selectedItems;

        [Browsable(false)]
        public ScrollableListItemCollection SelectedItems
        {
            get
            {
                _selectedItems.Clear();
                _selectedItems.AddRange(_selectedIndexes.Values);
                return _selectedItems;
            }
        }

        [Browsable(false)]
        public ScrollableListItem SelectedItem
        {
            get
            {
                if(SelectedItems.Count != 0)
                {
                    return SelectedItems[0];
                }
                return null;
            }
        }

        [DefaultValue(true)]
        public bool MultipleSelect { get; set; }
        [DefaultValue(true)]
        public bool HighlightFocus { get; set; }

        public event MeasureItemEventHandler MeasureItem;
        public event ItemClickedEventHandler ItemClicked;
        public event EventHandler SelectionChanged;
        
        public ScrollableList()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            this.Borders = Borders.Left | Borders.Top | Borders.Right | Borders.Bottom;
            this.Items = new ScrollableListItemCollection(this);
            this._selectedItems = new ScrollableListItemCollection(this);
            this._scrollBarHandleBounds = new Rectangle(this.ClientSize.Width - SCROLLBAR_WIDTH - 1,
                    0, SCROLLBAR_WIDTH, MIN_SCROLLBAR_HEIGHT);
            this.Offset = 0;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            OnDrawItems(e.Graphics, e.ClipRectangle);
            OnDrawScrollBar(e.Graphics, e.ClipRectangle);
            DrawBorder(e);
        }

        private void DrawBorder(PaintEventArgs e)
        {
            if((this.Borders & Borders.Left) != 0)
            {
                e.Graphics.DrawLine(BORDER_PEN, 0, 0, 0, this.ClientSize.Height);
            }
            if ((this.Borders & Borders.Top) != 0)
            {
                e.Graphics.DrawLine(BORDER_PEN, 0, 0, this.ClientSize.Width - 1, 0);
            }
            if ((this.Borders & Borders.Right) != 0)
            {
                e.Graphics.DrawLine(BORDER_PEN, this.ClientSize.Width - 1, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            }
            if ((this.Borders & Borders.Bottom) != 0)
            {
                e.Graphics.DrawLine(BORDER_PEN, 0, this.ClientSize.Height - 1, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            }
        }

        protected virtual void OnDrawScrollBar(Graphics g, Rectangle clipRect)
        {
            //Item高于当前尺度&&
            //鼠标在内部 || 在拖拽中
            //当前绘制区域和滚动条相交
            if (_totleItemHeight > this.ClientSize.Height &&
                clipRect.IntersectsWith(this._scrollBarHandleBounds) &&
                (this._mouseEnter || this._scrollBarHandleDrag))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                //上下半圆，中间矩形填充
                Rectangle bounds = this._scrollBarHandleBounds;
                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddPie(bounds.X, bounds.Y, bounds.Width, bounds.Width, 180, 180);
                    gp.AddRectangle(new Rectangle(bounds.X, bounds.Y + bounds.Width / 2, bounds.Width, bounds.Height - bounds.Width));
                    gp.AddPie(bounds.X, bounds.Y + bounds.Height - bounds.Width, bounds.Width, bounds.Width, 0, 180);
                    gp.CloseAllFigures();
                    g.FillPath(this._scrollBarHandleBrush, gp);
                }
            }
        }

        protected virtual void OnDrawItems(Graphics g, Rectangle clipRect)
        {
            Rectangle rect = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            int height = 0;
            for (int i = 0; i < this.Items.Count; i++)
            {
                ScrollableListItem item = this.Items[i];

                rect.Height = item.Height;

                //计算Y，当前项目累计的高度+滑块相对偏移量
                rect.Y = height + _offset;

                if (rect.IntersectsWith(clipRect))
                {
                    //不在拖拽滑块，并且鼠标移动到上面就定为Focus
                    bool isFocus = (this.HighlightFocus || !this.HighlightFocus && this.Focused ) && 
                        !this._scrollBarHandleDrag && rect.Contains(PointToClient(MousePosition));
                    DrawItemEventArgs args = new DrawItemEventArgs(i, item, g, rect, isFocus, _selectedIndexes.ContainsKey(i),
                        this.Font, this.ForeColor, this.BackColor);
                    OnDrawItem(args);
                }

                height += item.Height;
            }
        }

        protected virtual void OnDrawItem(DrawItemEventArgs args)
        {
            args.DrawBackground();

            TextRenderer.DrawText(args.Graphics, args.Index + ":" + args.Item.ToString(), this.Font,
                args.Bounds, this.ForeColor,
                TextFormatFlags.EndEllipsis | TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            //滚动条区域重绘
            this._mouseEnter = true;
            this.Offset = this.Offset;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            //滚动条区域重绘
            this._mouseEnter = false;

            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                if (this._scrollBarHandleBounds.Contains(e.Location))
                {
                    //滚动bar上按下
                    this._scrollBarHandleDrag = true;
                    this._scrollBarHandleDragMouseOffsetY = e.Y;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                //鼠标释放结束拖动
                this._scrollBarHandleDrag = false;
                this._scrollBarHandleDragMouseOffsetY = -1;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            //滚动条区域重绘
            if (this._scrollBarHandleDrag)
            {
                //鼠标按下时拖动，更新滑块偏移
                this.Offset -= (int)(1.0 * (e.Y - this._scrollBarHandleDragMouseOffsetY) * this._totleItemHeight / this.ClientSize.Height);
                this._scrollBarHandleDragMouseOffsetY = e.Y;
                this.Invalidate();
            }
            else
            {
                //更新滑块的画刷
                bool focusHandle = this._scrollBarHandleBounds.Contains(e.Location);
                if (focusHandle &&
                    this._scrollBarHandleBrush != SCROLLBAR_HANDLE_BRUSH_FOCUSED)
                {
                    //鼠标移动到上面时候绘制Foused的深色
                    this._scrollBarHandleBrush = SCROLLBAR_HANDLE_BRUSH_FOCUSED;
                }
                else if (!focusHandle &&
                    this._scrollBarHandleBrush != SCROLLBAR_HANDLE_BRUSH_NORMAL)
                {
                    this._scrollBarHandleBrush = SCROLLBAR_HANDLE_BRUSH_NORMAL;
                }
                this.Invalidate();
            }
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            //鼠标滚轮重新设定滑块位置,默认滚动1/3的项目高度
            this.Offset += (e.Delta / SystemInformation.MouseWheelScrollDelta) * DEFAULT_MOUSE_WHEEL_OFFSET;
            
            this.Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            ItemInfo itemInfo = HitTest(e.Location);
            if(itemInfo == null)
            {
                return;
            }

            //设置选择状态
            if (this._selectedIndexes.ContainsKey(itemInfo.Index))
            {
                this._selectedIndexes.Remove(itemInfo.Index);
            }
            else
            {
                if (!this.MultipleSelect)
                {
                    //只能单选时
                    this._selectedIndexes.Clear();
                }
                this._selectedIndexes.Add(itemInfo.Index, itemInfo.Item);
            }

            //触发点击事件
            ItemClickedEventArgs args = new ItemClickedEventArgs(itemInfo.Item);
            OnItemClicked(args);

            //触发选择变化事件
            OnSelectionChanged(new EventArgs());

            //描绘选择状态
            this.Invalidate(itemInfo.Bounds);
        }

        private void OnSelectionChanged(EventArgs args)
        {
            SelectionChanged?.Invoke(this, args);
        }

        private ItemInfo HitTest(Point mousePosition)
        {
            Rectangle rect = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            int height = 0;
            for (int i = 0; i < this.Items.Count; i++)
            {
                ScrollableListItem item = this.Items[i];

                rect.Height = item.Height;

                //计算Y，当前项目累计的高度+滑块相对偏移量
                rect.Y = height + this.Offset;

                if (rect.Contains(mousePosition))
                {
                    ItemInfo ii = new ItemInfo();
                    ii.Index = i;
                    ii.Item = item;
                    ii.Bounds = rect;
                    return ii;
                }
                height += item.Height;
            }

            return null;
        }

        protected virtual void OnItemClicked(ItemClickedEventArgs args)
        {
            ItemClicked?.Invoke(this, args);
        }

        //计算所有Item的大小
        public void MeasureItems()
        {
            //计算每个项目的高度
            int totleItemHeight = 0;
            using (Graphics g = this.CreateGraphics())
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    ScrollableListItem item = this.Items[i];
                    OnMeasureItem(new MeasureItemEventArgs(i, item, g));

                    totleItemHeight += item.Height;
                }
            }

            this.TotleItemHeight = totleItemHeight;
        }

        internal void MeasureItemOnAdd(ScrollableListItem item)
        {
            List<ScrollableListItem> list = new List<ScrollableListItem>(1);
            list.Add(item);

            MeasureItemOnAdd(list);
        }

        internal void MeasureItemOnAdd(IEnumerable<ScrollableListItem> collection)
        {
            int totleItemHeight = 0;
            using (Graphics g = this.CreateGraphics())
            {
                foreach (var item in collection)
                {
                    OnMeasureItem(new MeasureItemEventArgs(this.Items.IndexOf(item), item, g));

                    totleItemHeight += item.Height;
                }
            }

            this.TotleItemHeight = totleItemHeight;
        }

        internal void MeasureItemOnRemove(ScrollableListItem item)
        {
            using (Graphics g = this.CreateGraphics())
            {
                OnMeasureItem(new MeasureItemEventArgs(this.Items.IndexOf(item), item, g));
                this.TotleItemHeight -= item.Height;
            }
        }

        protected virtual void OnMeasureItem(MeasureItemEventArgs args)
        {
            MeasureItem?.Invoke(this, args);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            //计算项目区域
            this.MeasureItems();

            //绘制所有
            this.Invalidate();
        }
    }
}
