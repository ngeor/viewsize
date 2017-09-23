using System;
using AppKit;
using CRLFLabs.ViewSize;
using Foundation;

namespace ViewSizeMac
{
    class FolderOutlineDelegate : NSOutlineViewDelegate
    {
        private const string CellIdentifier = "ViewSizeCell";

        public override NSView GetView(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
        {
			// This pattern allows you reuse existing views when they are no-longer in use.
			// If the returned view is null, you instance up a new view
			// If a non-null view is returned, you modify it enough to reflect the new data
			NSTextField view = (NSTextField)outlineView.MakeView(CellIdentifier, this);
			if (view == null)
			{
				view = new NSTextField();
				view.Identifier = CellIdentifier;
				view.BackgroundColor = NSColor.Clear;
				view.Bordered = false;
				view.Selectable = false;
				view.Editable = false;
			}

			// Cast item
            var folder = item as FileSystemEntry;

			// Setup view based on the column selected
			switch (tableColumn.Title)
			{
				case "Folder":
                    view.StringValue = folder.DisplayText;
					break;
				case "Size":
                    view.StringValue = folder.DisplaySize;
					break;
                default:
                    view.StringValue = tableColumn.Title;
                    break;
			}

			return view;
        }

        public override void SelectionDidChange(NSNotification notification)
        {
            // do not call base method here
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler SelectionChanged;
    }
}
