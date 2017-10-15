using CRLFLabs.ViewSize.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Represents the view of the Tree Map control.
    /// </summary>
    public interface ITreeMapView
    {
        /// <summary>
        /// Raised when the view requires a redraw.
        /// </summary>
        event EventHandler RedrawNeeded;

        /// <summary>
        /// Gets the bounds of the view.
        /// </summary>
        RectangleD BoundsD { get; }

        /// <summary>
        /// Asks the view to redraw itself.
        /// </summary>
        void Redraw();
    }
}
