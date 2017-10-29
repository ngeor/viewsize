// <copyright file="TreeListViewConverter.cs" company="CRLFLabs">
// Copyright (c) CRLFLabs. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ViewSizeWpf.Controls
{
    /// <summary>
    /// Represents a convert that can calculate the indentation of any element in a class derived from TreeView.
    /// </summary>
    public class TreeListViewConverter : IValueConverter
    {
        public const double Indentation = 10;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // If the value is null, don't return anything
            if (value == null)
            {
                return null;
            }

            // Convert the item to a double
            if (targetType == typeof(double) && typeof(DependencyObject).IsAssignableFrom(value.GetType()))
            {
                // Cast the item as a DependencyObject
                DependencyObject element = value as DependencyObject;

                // Create a level counter with value set to -1
                int level = -1;

                // Move up the visual tree and count the number of TreeViewItem's.
                for (; element != null; element = VisualTreeHelper.GetParent(element))
                {
                    // Check whether the current elemeent is a TreeViewItem
                    if (typeof(TreeViewItem).IsAssignableFrom(element.GetType()))
                    {
                        // Increase the level counter
                        level++;
                    }
                }

                // Return the indentation as a double
                return Indentation * level;
            }

            // Type conversion is not supported
            throw new NotSupportedException(
                $"Cannot convert from <{value.GetType()}> to <{targetType}> using <TreeListViewConverter>.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("This method is not supported.");
        }
    }
}
