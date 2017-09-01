using System;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnSelectFolder(object sender, EventArgs e)
    {
        MessageDialog m = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, " Select a folder");
        m.Run();
        m.Destroy();
    }

    protected void OnScanFolder(object sender, EventArgs e)
    {
    }
}
