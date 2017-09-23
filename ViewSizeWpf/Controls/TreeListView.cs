using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

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
            //Override the default style and the default control template
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata(typeof(TreeListView)));
        }

        /// <summary>
        /// Initialize a new instance of TreeListView.
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
            DependencyProperty.Register("Columns", typeof(GridViewColumnCollection),
            typeof(TreeListView),
            new UIPropertyMetadata(null));

        /// <summary>
        /// This dependency property enables selected items to be scrolled into visible view automatically.
        /// </summary>
        public static readonly DependencyProperty BringIntoViewWhenSelectedProperty =
            DependencyProperty.RegisterAttached("BringIntoViewWhenSelected", typeof(bool),
                typeof(TreeListView), new UIPropertyMetadata(false, OnBringIntoViewWhenSelected));
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

    /// <summary>
    /// Represents a control that can switch states in order to expand a node of a TreeListView.
    /// </summary>
    public class TreeListViewExpander : ToggleButton { }

    /// <summary>
    /// Represents a convert that can calculate the indentation of any element in a class derived from TreeView.
    /// </summary>
    public class TreeListViewConverter : IValueConverter
    {
        public const double Indentation = 10;

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //If the value is null, don't return anything
            if (value == null) return null;
            //Convert the item to a double
            if (targetType == typeof(double) && typeof(DependencyObject).IsAssignableFrom(value.GetType()))
            {
                //Cast the item as a DependencyObject
                DependencyObject Element = value as DependencyObject;
                //Create a level counter with value set to -1
                int Level = -1;
                //Move up the visual tree and count the number of TreeViewItem's.
                for (; Element != null; Element = VisualTreeHelper.GetParent(Element))
                    //Check whether the current elemeent is a TreeViewItem
                    if (typeof(TreeViewItem).IsAssignableFrom(Element.GetType()))
                        //Increase the level counter
                        Level++;
                //Return the indentation as a double
                return Indentation * Level;
            }
            //Type conversion is not supported
            throw new NotSupportedException(
                string.Format("Cannot convert from <{0}> to <{1}> using <TreeListViewConverter>.",
                value.GetType(), targetType));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("This method is not supported.");
        }

        #endregion
    }

}
