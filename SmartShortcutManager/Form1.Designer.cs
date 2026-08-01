namespace SmartShortcutManager;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    private Label label1;
    private Label label2;
    private Label label3;
    private Label lblExample;
    private TextBox txtMappingName;
    private TextBox txtServiceName;
    private TextBox txtExePath;
    private Button btnAdd;
    private Button btnStart;
    private Button btnStop;
    private Button btnCreateShortcut;
    private Button btnDelete;
    private ListView listViewPairs;
    private ColumnHeader columnName;
    private ColumnHeader columnService;
    private ColumnHeader columnExe;
    private Label lblStatus;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        label1 = new Label();
        label2 = new Label();
        label3 = new Label();
        lblExample = new Label();
        txtMappingName = new TextBox();
        txtServiceName = new TextBox();
        txtExePath = new TextBox();
        btnAdd = new Button();
        btnDelete = new Button();
        btnStart = new Button();
        btnStop = new Button();
        btnCreateShortcut = new Button();
        listViewPairs = new ListView();
        columnName = new ColumnHeader();
        columnService = new ColumnHeader();
        columnExe = new ColumnHeader();
        lblStatus = new Label();

        SuspendLayout();

        label1.AutoSize = true;
        label1.Location = new System.Drawing.Point(20, 20);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(78, 15);
        label1.Text = "Bağlantı Adı";

        txtMappingName.Location = new System.Drawing.Point(150, 17);
        txtMappingName.Size = new System.Drawing.Size(620, 28);

        label2.AutoSize = true;
        label2.Location = new System.Drawing.Point(20, 60);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(110, 30);
        label2.Text = "Servis Adları\r\n(virgülle ayrılmış)";

        txtServiceName.Location = new System.Drawing.Point(150, 57);
        txtServiceName.Size = new System.Drawing.Size(620, 28);

        label3.AutoSize = true;
        label3.Location = new System.Drawing.Point(20, 108);
        label3.Name = "label3";
        label3.Size = new System.Drawing.Size(110, 30);
        label3.Text = "EXE Konumları\r\n(virgülle ayrılmış)";

        txtExePath.Location = new System.Drawing.Point(150, 105);
        txtExePath.Size = new System.Drawing.Size(620, 28);

        lblExample.AutoSize = true;
        lblExample.Location = new System.Drawing.Point(150, 145);
        lblExample.Name = "lblExample";
        lblExample.Size = new System.Drawing.Size(620, 36);
        lblExample.ForeColor = System.Drawing.Color.DimGray;
        lblExample.Text = "Örnek servis: wuauserv, bits\r\nÖrnek EXE: C:\\Windows\\System32\\notepad.exe, C:\\Program Files\\App\\app.exe";

        btnAdd.Location = new System.Drawing.Point(790, 17);
        btnAdd.Size = new System.Drawing.Size(120, 30);
        btnAdd.Text = "Eşleştirme Ekle";
        btnAdd.Click += btnAdd_Click;

        btnDelete.Location = new System.Drawing.Point(790, 57);
        btnDelete.Size = new System.Drawing.Size(120, 30);
        btnDelete.Text = "Eşleştirme Sil";
        btnDelete.Click += btnDelete_Click;

        btnStart.Location = new System.Drawing.Point(790, 102);
        btnStart.Size = new System.Drawing.Size(120, 30);
        btnStart.Text = "Başlat";
        btnStart.Click += btnStart_Click;

        btnStop.Location = new System.Drawing.Point(790, 142);
        btnStop.Size = new System.Drawing.Size(120, 30);
        btnStop.Text = "Durdur";
        btnStop.Click += btnStop_Click;

        btnCreateShortcut.Location = new System.Drawing.Point(790, 182);
        btnCreateShortcut.Size = new System.Drawing.Size(120, 30);
        btnCreateShortcut.Text = "Kısayol Oluştur";
        btnCreateShortcut.Click += btnCreateShortcut_Click;

        listViewPairs.Location = new System.Drawing.Point(20, 215);
        listViewPairs.Size = new System.Drawing.Size(890, 250);
        listViewPairs.View = View.Details;
        listViewPairs.FullRowSelect = true;
        listViewPairs.MultiSelect = false;
        listViewPairs.Columns.AddRange(new[] { columnName, columnService, columnExe });

        columnName.Text = "Bağlantı";
        columnName.Width = 180;

        columnService.Text = "Servisler";
        columnService.Width = 260;

        columnExe.Text = "Uygulamalar (EXE'ler)";
        columnExe.Width = 450;

        lblStatus.AutoSize = true;
        lblStatus.Location = new System.Drawing.Point(20, 475);
        lblStatus.Size = new System.Drawing.Size(100, 15);
        lblStatus.Text = "Hazır";

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(940, 510);
        Text = "Akıllı Kısayol Yöneticisi V8";

        Controls.Add(label1);
        Controls.Add(txtMappingName);
        Controls.Add(label2);
        Controls.Add(txtServiceName);
        Controls.Add(label3);
        Controls.Add(txtExePath);
        Controls.Add(lblExample);
        Controls.Add(btnAdd);
        Controls.Add(btnStart);
        Controls.Add(btnStop);
        Controls.Add(btnCreateShortcut);
        Controls.Add(btnDelete);
        Controls.Add(listViewPairs);
        Controls.Add(lblStatus);

        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}

