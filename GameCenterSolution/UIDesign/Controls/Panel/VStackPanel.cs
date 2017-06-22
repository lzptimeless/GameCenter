using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace UIDesign.Controls
{
    public class VStackPanel : VirtualizingPanel, IScrollInfo
    {
        #region fields
        const double ScrollLineAmount = 16;

        Point _offset;
        Size _extentSize;
        Size _viewportSize;
        Dictionary<UIElement, Size> _childDesiredSizeList = new Dictionary<UIElement, Size>();
        Dictionary<UIElement, Rect> _childLayoutList = new Dictionary<UIElement, Rect>();
        #endregion

        #region properties
        #region Orientation
        /// <summary>
        /// Get or set <see cref="Orientation"/>
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> of <see cref="Orientation"/>
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(VStackPanel), new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure, OrientationChanged));

        private static void OrientationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((VStackPanel)obj).OnOrientationChanged((Orientation)e.NewValue, (Orientation)e.OldValue);
        }

        protected virtual void OnOrientationChanged(Orientation newValue, Orientation oldValue)
        {

        }
        #endregion
        #endregion

        #region public methods

        #endregion

        #region private methods
        protected override Size MeasureOverride(Size availableSize)
        {
            // 参数说明
            // availableSize长和宽都是0到无限
            // avaialbleSize Width and Height both between 0 and infinite

            // 确定是否启用虚拟模式
            var itemsOwner = ItemsControl.GetItemsOwner(this);
            if (itemsOwner != null)
            {
                // 预测子项大小
                
            }
            else
            {
                if (Orientation == Orientation.Vertical)
                {
                    Size extentSize = new Size();
                    _childLayoutList.Clear();
                    double offsetY = 0;
                    Size itemSpace = new Size(availableSize.Width, double.PositiveInfinity);
                    foreach (UIElement child in InternalChildren)
                    {
                        child.Measure(itemSpace);
                        Size childDesiredSize = child.DesiredSize;
                        _childLayoutList.Add(child, new Rect(0, offsetY, childDesiredSize.Width, childDesiredSize.Height));

                        extentSize.Height += childDesiredSize.Height;
                        extentSize.Width = Math.Max(extentSize.Width, childDesiredSize.Width);

                        offsetY += childDesiredSize.Height;
                    }

                    return extentSize;
                }
                else
                {
                    Size extentSize = new Size();
                    _childLayoutList.Clear();
                    double offsetX = 0;
                    Size itemSpace = new Size(double.PositiveInfinity, availableSize.Height);
                    foreach (UIElement child in InternalChildren)
                    {
                        child.Measure(itemSpace);
                        Size childDesiredSize = child.DesiredSize;
                        _childLayoutList.Add(child, new Rect(offsetX, 0, childDesiredSize.Width, childDesiredSize.Height));

                        extentSize.Width += childDesiredSize.Width;
                        extentSize.Height = Math.Max(extentSize.Height, childDesiredSize.Height);

                        offsetX += childDesiredSize.Width;
                    }

                    return extentSize;
                }
            }

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                if (_childLayoutList.ContainsKey(child))
                    child.Arrange(_childLayoutList[child]);
            }

            return finalSize;
        }
        #endregion

        #region IScrollInfo
        public bool CanHorizontallyScroll { get; set; }

        public bool CanVerticallyScroll { get; set; }

        public double ExtentHeight { get { return _extentSize.Height; } }

        public double ExtentWidth { get { return _extentSize.Width; } }

        public double HorizontalOffset { get { return _offset.X; } }

        public ScrollViewer ScrollOwner { get; set; }

        public double VerticalOffset { get { return _offset.Y; } }

        public double ViewportHeight { get { return _viewportSize.Height; } }

        public double ViewportWidth { get { return _viewportSize.Width; } }

        public void LineUp()
        {
            SetVerticalOffset(VerticalOffset - ScrollLineAmount);
        }

        public void LineDown()
        {
            SetVerticalOffset(VerticalOffset + ScrollLineAmount);
        }

        public void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ScrollLineAmount);
        }

        public void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + ScrollLineAmount);
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return new Rect();
        }

        public void MouseWheelUp()
        {
            SetVerticalOffset(VerticalOffset - ScrollLineAmount * SystemParameters.WheelScrollLines);
        }

        public void MouseWheelDown()
        {
            SetVerticalOffset(VerticalOffset + ScrollLineAmount * SystemParameters.WheelScrollLines);
        }

        public void MouseWheelLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ScrollLineAmount * SystemParameters.WheelScrollLines);
        }

        public void MouseWheelRight()
        {
            SetHorizontalOffset(HorizontalOffset + ScrollLineAmount * SystemParameters.WheelScrollLines);
        }

        public void PageUp()
        {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }

        public void PageDown()
        {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        public void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }

        public void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }

        public void SetHorizontalOffset(double offset)
        {
            offset = Math.Max(Math.Min(ExtentWidth - ViewportWidth, offset), 0);
            _offset = new Point(offset, _offset.Y);

            InvalidateScrollInfo();
            InvalidateMeasure();
        }

        public void SetVerticalOffset(double offset)
        {
            offset = Math.Max(Math.Min(ExtentHeight - ViewportHeight, offset), 0);
            _offset = new Point(_offset.X, offset);

            InvalidateScrollInfo();
            InvalidateMeasure();
        }

        private void InvalidateScrollInfo()
        {
            if (ScrollOwner != null)
            {
                ScrollOwner.InvalidateScrollInfo();
            }
        }
        #endregion
    }
}
