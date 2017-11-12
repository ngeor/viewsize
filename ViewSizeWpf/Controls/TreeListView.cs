// <copyright file="TreeListView.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;

namespace ViewSizeWpf.Controls
{
    /// <summary>
    /// Represents a control that displays hierarchical data in a tree structure
    /// that has items that can expand and collapse.
    /// </summary>
    public class TreeListView : TreeView
    {
        static TreeListView()
        {
            // Override the default style and the default control template
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata(typeof(TreeListView)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeListView"/> class.
        /// </summary>
        public TreeListView()
        {
            Columns = new GridViewColumnCollection();
        }

        #region Properties

        /// <summary>
        /// Gets or sets the collection of System.Windows.Controls.GridViewColumn
        /// objects that is defined for this TreeListView.
        /// </summary>
        public GridViewColumnCollection Columns
        {
            get { return (GridViewColumnCollection)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether columns in a TreeListView can be
        /// reordered by a drag-and-drop operation. This is a dependency property.
        /// </summary>
        public bool AllowsColumnReorder
        {
            get { return (bool)GetValue(AllowsColumnReorderProperty); }
            set { SetValue(AllowsColumnReorderProperty, value); }
        }
        #endregion

        #region Static Dependency Properties

        // Using a DependencyProperty as the backing store for AllowsColumnReorder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowsColumnReorderProperty =
            DependencyProperty.Register("AllowsColumnReorder", typeof(bool), typeof(TreeListView), new UIPropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(
                "Columns",
                typeof(GridViewColumnCollection),
                typeof(TreeListView),
                new UIPropertyMetadata(null));

        /// <summary>
        /// This dependency property enables selected items to be scrolled into visible view automatically.
        /// </summary>
        public static readonly DependencyProperty BringIntoViewWhenSelectedProperty =
            DependencyProperty.RegisterAttached(
                "BringIntoViewWhenSelected",
                typeof(bool),
                typeof(TreeListView),
                new UIPropertyMetadata(false, OnBringIntoViewWhenSelected));
        #endregion

        /// <summary>
        /// Called when the property <c>BringIntoViewWhenSelected</c> changes.
        /// </summary>
        private static void OnBringIntoViewWhenSelected(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItem item = depObj as TreeViewItem;
            if (item == null)
            {
                return;
            }

            if (e.NewValue is bool == false)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                // scroll the item into the view
                item.BringIntoView();
            }
        }

        /// <summary>
        /// Getter of the <c>BringIntoViewWhenSelected</c> property.
        /// </summary>
        public static bool GetBringIntoViewWhenSelected(TreeViewItem treeViewItem)
        {
            return (bool)treeViewItem.GetValue(BringIntoViewWhenSelectedProperty);
        }

        /// <summary>
        /// Setter of the <c>BringIntoViewWhenSelected</c> property.
        /// </summary>
        public static void SetBringIntoViewWhenSelected(TreeViewItem treeViewItem, bool value)
        {
            treeViewItem.SetValue(BringIntoViewWhenSelectedProperty, value);
        }
    }
}
