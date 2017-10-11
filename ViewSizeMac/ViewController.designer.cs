// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace ViewSizeMac
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnScan { get; set; }

		[Outlet]
		AppKit.NSButton btnSelectFolder { get; set; }

		[Outlet]
		AppKit.NSTableColumn colFolder { get; set; }

		[Outlet]
		AppKit.NSTableColumn colSize { get; set; }

		[Outlet]
		ViewSizeMac.NSFolderGraph folderGraph { get; set; }

		[Outlet]
		AppKit.NSTextField lblDuration { get; set; }

		[Outlet]
		AppKit.NSTextField lblStatus { get; set; }

		[Outlet]
		AppKit.NSOutlineView outlineView { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator pbScan { get; set; }

		[Outlet]
		AppKit.NSTextField txtFolder { get; set; }

		[Action ("OnCancelScan:")]
		partial void OnCancelScan (Foundation.NSObject sender);

		[Action ("OnScan:")]
		partial void OnScan (Foundation.NSObject sender);

		[Action ("OnSelectFolder:")]
		partial void OnSelectFolder (Foundation.NSObject sender);

		[Action ("OnShowInFinder:")]
		partial void OnShowInFinder (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnScan != null) {
				btnScan.Dispose ();
				btnScan = null;
			}

			if (btnSelectFolder != null) {
				btnSelectFolder.Dispose ();
				btnSelectFolder = null;
			}

			if (colFolder != null) {
				colFolder.Dispose ();
				colFolder = null;
			}

			if (colSize != null) {
				colSize.Dispose ();
				colSize = null;
			}

			if (folderGraph != null) {
				folderGraph.Dispose ();
				folderGraph = null;
			}

			if (lblDuration != null) {
				lblDuration.Dispose ();
				lblDuration = null;
			}

			if (lblStatus != null) {
				lblStatus.Dispose ();
				lblStatus = null;
			}

			if (outlineView != null) {
				outlineView.Dispose ();
				outlineView = null;
			}

			if (pbScan != null) {
				pbScan.Dispose ();
				pbScan = null;
			}

			if (txtFolder != null) {
				txtFolder.Dispose ();
				txtFolder = null;
			}
		}
	}
}
