namespace SmartShortcutManager;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    private Label label1;
    private Label label2;
    private Label label3;
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
        txtMappingName = new TextBox();
        txtServiceName = new TextBox();
        txtExePath = new TextBox();
        btnAdd = new Button();
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
        label1.Location = new System.Drawing.Point(50, 15);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(78, 15);
        label1.Text = "Bağlantı Adı";

        txtMappingName.Location = new System.Drawing.Point(150, 12);
        txtMappingName.Size = new System.Drawing.Size(320, 23);

        label2.AutoSize = true;
        label2.Location = new System.Drawing.Point(50, 46);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(120, 15);
        label2.Text = "Servis Adları (virgülle ayrılmış)";

        txtServiceName.Location = new System.Drawing.Point(150, 43);
        txtServiceName.Size = new System.Drawing.Size(320, 23);

        label3.AutoSize = true;
        label3.Location = new System.Drawing.Point(50, 77);
        label3.Name = "label3";
        label3.Size = new System.Drawing.Size(68, 15);
        label3.Text = "EXE Yolları (virgülle ayrılmış)";

        txtExePath.Location = new System.Drawing.Point(150, 74);
        txtExePath.Size = new System.Drawing.Size(320, 23);

        btnAdd.Location = new System.Drawing.Point(480, 12);
        btnAdd.Size = new System.Drawing.Size(120, 25);
        btnAdd.Text = "Eşleştirme Ekle";
        btnAdd.Click += btnAdd_Click;

        btnDelete.Location = new System.Drawing.Point(610, 12);
        btnDelete.Size = new System.Drawing.Size(120, 25);
        btnDelete.Text = "Eşleştirme Sil";
        btnDelete.Click += btnDelete_Click;

        btnStart.Location = new System.Drawing.Point(450, 43);
        btnStart.Size = new System.Drawing.Size(120, 25);
        btnStart.Text = "Başlat";
        btnStart.Click += btnStart_Click;

        btnStop.Location = new System.Drawing.Point(450, 74);
        btnStop.Size = new System.Drawing.Size(120, 25);
        btnStop.Text = "Durdur";
        btnStop.Click += btnStop_Click;

        btnCreateShortcut.Location = new System.Drawing.Point(450, 105);
        btnCreateShortcut.Size = new System.Drawing.Size(120, 25);
        btnCreateShortcut.Text = "Kısayol Oluştur";
        btnCreateShortcut.Click += btnCreateShortcut_Click;

        listViewPairs.Location = new System.Drawing.Point(12, 140);
        listViewPairs.Size = new System.Drawing.Size(760, 260);
        listViewPairs.View = View.Details;
        listViewPairs.FullRowSelect = true;
        listViewPairs.MultiSelect = false;
        listViewPairs.Columns.AddRange(new[] { columnName, columnService, columnExe });

        columnName.Text = "Bağlantı";
        columnName.Width = 150;

        columnService.Text = "Servisler";
        columnService.Width = 180;

        columnExe.Text = "Uygulamalar (EXE'ler)";
        columnExe.Width = 410;

        lblStatus.AutoSize = true;
        lblStatus.Location = new System.Drawing.Point(12, 410);
        lblStatus.Size = new System.Drawing.Size(100, 15);
        lblStatus.Text = "Hazır";

        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(784, 441);
        Text = "Akıllı Kısayol Yöneticisi V8";

        Controls.Add(label1);
        Controls.Add(txtMappingName);
        Controls.Add(label2);
        Controls.Add(txtServiceName);
        Controls.Add(label3);
        Controls.Add(txtExePath);
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

