namespace SmartShortcutManager;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        try
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1(args));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Uygulama başlatılırken hata oluştu: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}