using CRLFLabs.ViewSize.Drawing;
using System;

namespace CRLFLabs.ViewSize.Mvp
{
    /// <summary>
    /// Represents the view of the Tree Map control.
    /// </summary>
    public interface ITreeMapView : IView<IMainModel>
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

        void SelectionChanging();
        void SelectionChanged();
    }
}
