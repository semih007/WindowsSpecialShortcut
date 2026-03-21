using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text.Json;
using System.Windows.Forms;

namespace SmartShortcutManager
{
    public partial class Form1 : Form
    {
        private readonly string _configPath;
        private readonly Config _config = new();

        public Form1()
        {
            InitializeComponent();
            _configPath = Path.Combine(Application.StartupPath, "config.json");

            if (!IsAdministrator())
            {
                lblStatus.Text = "Uyarı: Yönetici yetkisi yok. UAC ile çalıştırın.";
                lblStatus.ForeColor = System.Drawing.Color.DarkRed;
            }
            else
            {
                lblStatus.Text = "Yönetici olarak çalışıyor.";
                lblStatus.ForeColor = System.Drawing.Color.DarkGreen;
            }

            LoadConfig();
            RefreshPairs();
        }

        private bool IsAdministrator()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private void LoadConfig()
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    _config.Pairs.Clear();
                    return;
                }

                var json = File.ReadAllText(_configPath);
                var loaded = JsonSerializer.Deserialize<Config>(json);
                if (loaded != null)
                {
                    _config.Pairs = loaded.Pairs ?? new List<ServiceExePair>();
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Config yüklenemedi: {ex.Message}", true);
            }
        }

        private void SaveConfig()
        {
            try
            {
                var json = JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_configPath, json);
            }
            catch (Exception ex)
            {
                UpdateStatus($"Config kaydedilemedi: {ex.Message}", true);
            }
        }

        private void RefreshPairs()
        {
            listViewPairs.Items.Clear();
            foreach (var p in _config.Pairs)
            {
                var item = new ListViewItem(new[] { p.Name, p.ServiceName, p.ExePath });
                listViewPairs.Items.Add(item);
            }
        }

        private ServiceExePair? GetSelectedPair()
        {
            if (listViewPairs.SelectedItems.Count == 0)
                return null;

            var sel = listViewPairs.SelectedItems[0];
            var serviceName = sel.SubItems[1].Text;
            var exePath = sel.SubItems[2].Text;
            return _config.Pairs.FirstOrDefault(x => x.ServiceName == serviceName && x.ExePath == exePath);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var newPair = new ServiceExePair { Name = txtMappingName.Text.Trim(), ServiceName = txtServiceName.Text.Trim(), ExePath = txtExePath.Text.Trim() };
            if (string.IsNullOrWhiteSpace(newPair.Name) || string.IsNullOrWhiteSpace(newPair.ServiceName) || string.IsNullOrWhiteSpace(newPair.ExePath))
            {
                UpdateStatus("Tüm alanlar gereklidir.", true);
                return;
            }

            if (!File.Exists(newPair.ExePath))
            {
                UpdateStatus("Executable dosyası bulunamadı.", true);
                return;
            }

            _config.Pairs.Add(newPair);
            SaveConfig();
            RefreshPairs();
            UpdateStatus("Yeni eşleştirme eklendi.", false);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            var pair = GetSelectedPair();
            if (pair == null)
            {
                UpdateStatus("Başlatmak için bir eşleştirme seçin.", true);
                return;
            }

            RunStartScenario(pair);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            var pair = GetSelectedPair();
            if (pair == null)
            {
                UpdateStatus("Durdurmak için bir eşleştirme seçin.", true);
                return;
            }

            RunStopScenario(pair);
        }

        private void btnCreateShortcut_Click(object sender, EventArgs e)
        {
            var pair = GetSelectedPair();
            if (pair == null)
            {
                UpdateStatus("Kısayol oluşturmak için bir eşleştirme seçin.", true);
                return;
            }

            CreateShortcut(pair);
        }

        private void RunStartScenario(ServiceExePair pair)
        {
            try
            {
                UpdateStatus($"{pair.ServiceName} servisi başlatılıyor...", false);
                using var service = new ServiceController(pair.ServiceName);

                if (service.Status != ServiceControllerStatus.Running)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(5));
                }

                if (service.Status != ServiceControllerStatus.Running)
                    throw new InvalidOperationException("Servis Running durumuna ulaşamadı.");

                UpdateStatus("Servis çalışıyor. Uygulama başlatılıyor...", false);

                if (!File.Exists(pair.ExePath))
                    throw new FileNotFoundException("EXE dosyası bulunamıyor.", pair.ExePath);

                var procName = Path.GetFileName(pair.ExePath);
                var existing = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(procName));
                if (existing.Length > 0)
                {
                    UpdateStatus("Uygulama zaten çalışıyor.", false);
                }
                else
                {
                    Process.Start(new ProcessStartInfo(pair.ExePath) { UseShellExecute = true });
                    UpdateStatus("Uygulama başlatıldı.", false);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus("Başlatma hatası: " + ex.Message, true);
                return;
            }

            UpdateStatus("Başlatma senaryosu tamamlandı.", false);
        }

        private void RunStopScenario(ServiceExePair pair)
        {
            try
            {
                UpdateStatus($"{pair.ExePath} süreci kapatılıyor...", false);
                var exeName = Path.GetFileNameWithoutExtension(pair.ExePath);
                var processes = Process.GetProcessesByName(exeName);

                foreach (var p in processes)
                {
                    if (!p.HasExited)
                    {
                        if (p.CloseMainWindow())
                        {
                            p.WaitForExit(5000);
                        }

                        if (!p.HasExited)
                        {
                            p.Kill(true);
                            p.WaitForExit(5000);
                        }
                    }
                }

                processes = Process.GetProcessesByName(exeName);
                if (processes.Length > 0)
                    throw new InvalidOperationException("EXE süreci kapatılamadı.");

                UpdateStatus("Uygulama kapatıldı. Servis durduruluyor...", false);

                using var service = new ServiceController(pair.ServiceName);

                if (service.Status != ServiceControllerStatus.Stopped)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                }

                if (service.Status != ServiceControllerStatus.Stopped)
                    throw new InvalidOperationException("Servis başarılı şekilde durmadı.");
            }
            catch (Exception ex)
            {
                UpdateStatus("Durdurma hatası: " + ex.Message, true);
                return;
            }

            UpdateStatus("Durdurma senaryosu tamamlandı.", false);
        }

        private void CreateShortcut(ServiceExePair pair)
        {
            try
            {
                if (!File.Exists(pair.ExePath))
                    throw new FileNotFoundException("EXE dosyası bulunamadı.", pair.ExePath);

                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                var shortcutPath = Path.Combine(desktopPath, $"{pair.Name}-{Path.GetFileNameWithoutExtension(pair.ExePath)}.lnk");

                var shellType = Type.GetTypeFromProgID("WScript.Shell");
                if (shellType == null)
                    throw new PlatformNotSupportedException("WScript.Shell kullanılamıyor.");

                dynamic shell = Activator.CreateInstance(shellType);
                if (shell == null)
                    throw new InvalidOperationException("Shell instance oluşturulamadı.");

                dynamic shortcut = shell.CreateShortcut(shortcutPath);
                if (shortcut == null)
                    throw new InvalidOperationException("Shortcut oluşturulamadı.");

                shortcut.TargetPath = pair.ExePath;
                shortcut.WorkingDirectory = Path.GetDirectoryName(pair.ExePath) ?? string.Empty;
                shortcut.Description = "Akıllı Kısayol Yöneticisi tarafından oluşturuldu.";
                shortcut.Save();

                UpdateStatus("Kısayol masaüstüne yaratıldı: " + shortcutPath, false);
            }
            catch (Exception ex)
            {
                UpdateStatus("Kısayol oluşturulamadı: " + ex.Message, true);
            }
        }

        private void UpdateStatus(string text, bool isError)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = isError ? System.Drawing.Color.DarkRed : System.Drawing.Color.DarkGreen;
        }
    }
}

