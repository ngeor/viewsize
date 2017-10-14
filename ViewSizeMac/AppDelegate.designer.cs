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
	partial class AppDelegate
	{
		[Outlet]
		AppKit.NSMenuItem mnuFileCountTreeMap { get; set; }

		[Outlet]
		AppKit.NSMenuItem mnuFileSizeTreeMap { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (mnuFileSizeTreeMap != null) {
				mnuFileSizeTreeMap.Dispose ();
				mnuFileSizeTreeMap = null;
			}

			if (mnuFileCountTreeMap != null) {
				mnuFileCountTreeMap.Dispose ();
				mnuFileCountTreeMap = null;
			}
		}
	}
}
