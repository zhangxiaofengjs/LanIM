using System;
using System.Collections.Generic;
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
        private const int SCROLLBAR_WIDTH = 8;
        private const int MIN_SCROLLBAR_HEIGHT = (int)(SCROLLBAR_WIDTH * 1.5);
        private const int DEFAULT_MOUSE_WHEEL_OFFSET = 10;
        
        private static readonly Brush SCROLLBAR_HANDLE_BRUSH_NORMAL = new SolidBrush(Color.FromArgb(200, 210, 210, 210));
        private static readonly Brush SCROLLBAR_HANDLE_BRUSH_FOCUSED = new SolidBrush(Color.FromArgb(200, 186, 186, 186));

        private Rectangle _scrollBarHandleBounds = Rectangle.Empty;//滑块位置大小
        private Rectangle _scrollBarBounds = Rectangle.Empty;//滚动条区域
        private Brush _scrollBarHandleBrush = SCROLLBAR_HANDLE_BRUSH_NORMAL;//滑块绘制笔刷
        private int _scrollBarOffset = 0;//滑块的偏移量
        private int ScrollBarOffset
        {
            get
            {
                return _scrollBarOffset;
            }
            set
            {
                //更新偏移量，注意最上端和最下端
                this._scrollBarOffset = Math.Min(Math.Max(0, value), this.ClientSize.Height - this._scrollBarHandleHeight - 1);
                //更新滑块的位置
                this._scrollBarHandleBounds = new Rectangle(this.ClientSize.Width - SCROLLBAR_WIDTH - 1,
                    ScrollBarOffset, SCROLLBAR_WIDTH, this._scrollBarHandleHeight);
            }
        }

        private int _scrollBarHandleHeight = MIN_SCROLLBAR_HEIGHT;
        private int _totleItemHeight = 0;

        private bool _mouseEnter = false;
        private bool _scrollBarHandleDrag = false;
        private int _scrollBarHandleDragMouseOffsetY = -1;

        public ScrollableListItemCollection Items { get; set; }

        public event MeasureItemEventHandler MeasureItem;

        public ScrollableList()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            this.Items = new ScrollableListItemCollection(this);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            OnDrawItems(e.Graphics, e.ClipRectangle);
            OnDrawScrollBar(e.Graphics, e.ClipRectangle);
        }

        protected virtual void OnDrawScrollBar(Graphics g, Rectangle clipRect)
        {
            //Item高于当前尺度&&
            //鼠标在内部 || 在拖拽中
            //当前绘制区域和滚动条相交
            if (_totleItemHeight > this.ClientSize.Height &&
                clipRect.IntersectsWith(this._scrollBarBounds) &&
                (this._mouseEnter || this._scrollBarHandleDrag ))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                //上下半圆，中间矩形填充
                Rectangle bounds = this._scrollBarHandleBounds;
                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddPie(bounds.X, bounds.Y, bounds.Width, bounds.Width, 180, 180);
                    gp.AddRectangle(new Rectangle(bounds.X, bounds.Y + bounds.Width / 2, bounds.Width, bounds.Height - bounds.Width));
                    gp.AddPie(bounds.X, bounds.Y + _scrollBarHandleHeight - bounds.Width, bounds.Width, bounds.Width, 0, 180);
                    gp.CloseAllFigures();
                    g.FillPath(this._scrollBarHandleBrush, gp);
                }
            }
        }

        protected virtual void OnDrawItems(Graphics g, Rectangle clipRect)
        {
            Rectangle rect = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            for (int i = 0; i < this.Items.Count; i++)
            {
                ScrollableListItem item = this.Items[i];
                //rect.Top = i * item;

                if (rect.IntersectsWith(clipRect))
                {
                    OnDrawItem(item, g, rect);
                }
            }
        }

        protected virtual void OnDrawItem(ScrollableListItem item, Graphics g, Rectangle itemBounds)
        {
            g.FillRectangle(Brushes.Blue, itemBounds);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            //滚动条区域重绘
            this._mouseEnter = true;
            this.ScrollBarOffset = this.ScrollBarOffset;//为了更新滚动条bar的大小
            this._scrollBarBounds = new Rectangle(this._scrollBarHandleBounds.X, 0, 
                this.ClientSize.Width - this._scrollBarHandleBounds.X + 1, this.ClientSize.Height);

            this.Invalidate(this._scrollBarBounds);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            //滚动条区域重绘
            this._mouseEnter = false;
            this.Invalidate(this._scrollBarBounds);
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
                this.ScrollBarOffset += e.Y - this._scrollBarHandleDragMouseOffsetY;
                this._scrollBarHandleDragMouseOffsetY = e.Y;
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
            }

            this.Invalidate(this._scrollBarBounds);
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta > 0)
            {
                this.ScrollBarOffset -= DEFAULT_MOUSE_WHEEL_OFFSET;
            }
            else
            {
                this.ScrollBarOffset += DEFAULT_MOUSE_WHEEL_OFFSET;
            }
            this.Invalidate(this._scrollBarBounds);
        }
        //计算所有Item的大小
        public void MeasureItems()
        {
            //计算每个项目的高度
            _totleItemHeight = 0;
            using (Graphics g = this.CreateGraphics())
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    ScrollableListItem item = this.Items[i];
                    OnMeasureItem(new MeasureItemEventArgs(i, item, g));

                    _totleItemHeight += item.Height;
                }
            }

            //滑块高度
            this._scrollBarHandleHeight = Math.Max((int)(this.ClientSize.Height * this.ClientSize.Height * 1.0 / _totleItemHeight), MIN_SCROLLBAR_HEIGHT);
        }

        protected virtual void OnMeasureItem(MeasureItemEventArgs args)
        {
            MeasureItem?.Invoke(this, args);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            //尺寸发生变化，重新计算所有
            this.MeasureItems();
            this.Invalidate();
        }
    }
}
