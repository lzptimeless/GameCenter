using CleanerLogAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CleanerLogAnalyzer.Controls
{
    public class DiffThumbView : FrameworkElement
    {
        #region properties
        #region LogItems
        /// <summary>
        /// Get or set <see cref="LogItems"/>
        /// </summary>
        public IEnumerable<CleanerLogItem> LogItems
        {
            get { return (IEnumerable<CleanerLogItem>)GetValue(LogItemsProperty); }
            set { SetValue(LogItemsProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> of <see cref="LogItems"/>
        /// </summary>
        public static readonly DependencyProperty LogItemsProperty =
            DependencyProperty.Register("LogItems", typeof(IEnumerable<CleanerLogItem>), typeof(DiffThumbView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, LogItemsChanged));

        private static void LogItemsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((DiffThumbView)obj).OnLogItemsChanged((IEnumerable<CleanerLogItem>)e.NewValue, (IEnumerable<CleanerLogItem>)e.OldValue);
        }

        protected virtual void OnLogItemsChanged(IEnumerable<CleanerLogItem> newValue, IEnumerable<CleanerLogItem> oldValue)
        {

        }
        #endregion
        #endregion

        protected override void OnRender(DrawingContext drawingContext)
        {
            double width = ActualWidth;
            double height = ActualHeight;
            // 绘制背景色
            // Draw background
            if (width > 0 && height > 0)
            {
                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(240, 240, 240)), null, new Rect(0, 0, width, height));
            }

            // 绘制CCleaner独有项标记和Cortex独有项标记
            var logItems = LogItems;
            if (LogItems != null && LogItems.Any() && width > 0 && height > 0)
            {
                int itemsCount = LogItems.Count();
                double thickness = Math.Round(Math.Max(1, height / itemsCount));
                double preccY = 0;// 上一个需要标记的CCleaner数据的的Y坐标
                double prectxY = 0;// 上一个需要标记的Cortex Cleaner数据的Y坐标
                int i = 0;
                Pen ccPen = new Pen(new SolidColorBrush(Color.FromRgb(255, 0, 0)), thickness);
                Pen ctxPen = new Pen(new SolidColorBrush(Color.FromRgb(0, 255, 0)), thickness);
                foreach (var item in logItems)
                {
                    if (item.CCleanerContains && item.CortexContains)
                    {
                        // CCleaner和Cortex同事拥有这条数据，不需要标记
                        i++;
                        continue;
                    }

                    double preY = item.CCleanerContains ? preccY : prectxY;
                    double y = (double)i / itemsCount * height;
                    if (Math.Abs(y - preY) < 0.7)
                    {
                        // 与上一个标记重叠了，忽略这个标记，解决标记过多导致WPF渲染卡死
                        i++;
                        continue;
                    }

                    Pen pen = item.CCleanerContains ? ccPen : ctxPen;
                    drawingContext.DrawLine(pen, new Point(0, y), new Point(width, y));

                    if (item.CCleanerContains) preccY = y;
                    else prectxY = y;
                    i++;
                }
            }

            base.OnRender(drawingContext);
        }
    }
}
