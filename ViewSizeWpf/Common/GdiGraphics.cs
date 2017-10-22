using System.Drawing;
using System.Drawing.Drawing2D;
using CRLFLabs.ViewSize.Drawing;

namespace CRLFLabs.ViewSizeWpf.Common
{
    class GdiGraphics : IGraphics
    {
        public GdiGraphics(Graphics graphics)
        {
            Graphics = graphics;
        }

        private Graphics Graphics { get; }

        public void DrawRect(ColorD color, RectangleD rect, int width)
        {
            using (var pen = new Pen(color.ToColor(), width))
            {
                Graphics.DrawRectangle(pen, (float)rect.Left, (float)rect.Top, (float)rect.Width, (float)rect.Height);
            }
        }

        public void FillEllipseGradient(ColorD inner, ColorD outer, RectangleD rect)
        {
            using (GraphicsPath graphicsPath = new GraphicsPath())
            {
                graphicsPath.AddEllipse(rect.ToRectangleF());
                using (PathGradientBrush gradientBrush = new PathGradientBrush(graphicsPath)
                {
                    CenterColor = inner.ToColor(),
                    SurroundColors = new[]
                    {
                        outer.ToColor()
                    }                   
                })
                {
                    Graphics.FillEllipse(gradientBrush, rect.ToRectangleF());
                }
            }
        }

        public void FillEllipseGradient(ColorD inner, ColorD outer, RectangleD rect, PointD centerPoint)
        {
            using (GraphicsPath graphicsPath = new GraphicsPath())
            {
                graphicsPath.AddEllipse(rect.ToRectangleF());
                using (PathGradientBrush gradientBrush = new PathGradientBrush(graphicsPath)
                {
                    CenterColor = inner.ToColor(),
                    SurroundColors = new[]
                    {
                        outer.ToColor()
                    },
                    CenterPoint = centerPoint.ToPointF()
                })
                {
                    Graphics.FillEllipse(gradientBrush, rect.ToRectangleF());
                }
            }
        }

        public void FillRect(ColorD color, RectangleD rect)
        {
            using (var brush = new SolidBrush(color.ToColor()))
            {
                Graphics.FillRectangle(brush, rect.ToRectangleF());
            }
        }
    }
}
