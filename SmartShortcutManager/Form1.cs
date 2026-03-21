using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace SmartShortcutManager
{
    public partial class Form1 : Form
    {
        private readonly string _configPath;
        private readonly Config _config = new();

        public Form1(string[] args)
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

            if (args != null && args.Length > 0)
            {
                HandleCommandLine(args);
            }
        }

        public Form1() : this(Array.Empty<string>()) { }


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
                var item = new ListViewItem(new[] { p.Name, string.Join(", ", p.ServiceNames), p.ExePath });
                listViewPairs.Items.Add(item);
            }
        }

        private void HandleCommandLine(string[] args)
        {
            try
            {
                if (args.Length >= 2)
                {
                    var command = args[0].Trim();
                    var name = args[1].Trim('"');

                    if (command.Equals("--start", StringComparison.OrdinalIgnoreCase))
                    {
                        RunScenarioByName(name, true);
                        UpdateStatus($"{name} için başlatma komutu tamamlandı.", false);
                        Close();
                        return;
                    }
                    if (command.Equals("--stop", StringComparison.OrdinalIgnoreCase))
                    {
                        RunScenarioByName(name, false);
                        UpdateStatus($"{name} için durdurma komutu tamamlandı.", false);
                        Close();
                        return;
                    }
                    if (command.Equals("--toggle", StringComparison.OrdinalIgnoreCase))
                    {
                        RunToggleScenarioByName(name);
                        Close();
                        return;
                    }
                }

                UpdateStatus("Komut satırı parametresi bulunamadı veya geçersiz.", true);
            }
            catch (Exception ex)
            {
                UpdateStatus("Komut satırı işleme hatası: " + ex.Message, true);
            }
        }

        private void RunToggleScenarioByName(string mapName)
        {
            var pair = _config.Pairs.FirstOrDefault(x => x.Name.Equals(mapName, StringComparison.OrdinalIgnoreCase));
            if (pair == null)
            {
                UpdateStatus($"Eşleşme bulunamadı: {mapName}", true);
                return;
            }

            if (IsPairRunning(pair))
            {
                RunStopScenario(pair);
            }
            else
            {
                RunStartScenario(pair);
            }
        }

        private bool IsPairRunning(ServiceExePair pair)
        {
            foreach (var svcName in pair.ServiceNames)
            {
                try
                {
                    using var service = new ServiceController(svcName);
                    if (service.Status != ServiceControllerStatus.Running)
                        return false;
                }
                catch
                {
                    return false;
                }
            }

            var exeName = Path.GetFileNameWithoutExtension(pair.ExePath);
            return Process.GetProcessesByName(exeName).Any();
        }

        private void RunScenarioByName(string mapName, bool start)
        {
            var pair = _config.Pairs.FirstOrDefault(x => x.Name.Equals(mapName, StringComparison.OrdinalIgnoreCase));
            if (pair == null)
            {
                UpdateStatus($"Eşleşme bulunamadı: {mapName}", true);
                return;
            }

            if (start)
                RunStartScenario(pair);
            else
                RunStopScenario(pair);
        }

        private ServiceExePair? GetSelectedPair()
        {
            if (listViewPairs.SelectedItems.Count == 0)
                return null;

            var sel = listViewPairs.SelectedItems[0];
            var serviceNames = sel.SubItems[1].Text.Split(',').Select(s => s.Trim()).ToList();
            var exePath = sel.SubItems[2].Text;
            return _config.Pairs.FirstOrDefault(x => x.ServiceNames.SequenceEqual(serviceNames) && x.ExePath == exePath);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var serviceNames = txtServiceName.Text.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            var exePath = txtExePath.Text.Trim();
            var name = txtMappingName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || serviceNames.Count == 0 || string.IsNullOrWhiteSpace(exePath))
            {
                UpdateStatus("Tüm alanlar gereklidir.", true);
                return;
            }

            if (!File.Exists(exePath))
            {
                UpdateStatus("Executable dosyası bulunamadı.", true);
                return;
            }

            var newPair = new ServiceExePair { Name = name, ServiceNames = serviceNames, ExePath = exePath };
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
                UpdateStatus($"{string.Join(", ", pair.ServiceNames)} servisleri başlatılıyor...", false);

                // Tüm servisler için start komutu ver
                foreach (var svcName in pair.ServiceNames)
                {
                    using var service = new ServiceController(svcName);
                    if (service.Status != ServiceControllerStatus.Running)
                    {
                        service.Start();
                    }
                }

                // Tüm servisler Running olana kadar bekle ve kontrol et
                var timeout = TimeSpan.FromSeconds(10);
                var startTime = DateTime.Now;
                while (DateTime.Now - startTime < timeout)
                {
                    bool allRunning = true;
                    foreach (var svcName in pair.ServiceNames)
                    {
                        using var service = new ServiceController(svcName);
                        if (service.Status != ServiceControllerStatus.Running)
                        {
                            allRunning = false;
                            break;
                        }
                    }
                    if (allRunning)
                        break;
                    Thread.Sleep(1000); // 1 saniye bekle
                }

                // Son kontrol: hepsi başlamış mı?
                bool finalCheck = pair.ServiceNames.All(svcName =>
                {
                    using var service = new ServiceController(svcName);
                    return service.Status == ServiceControllerStatus.Running;
                });

                if (!finalCheck)
                    throw new InvalidOperationException("Bir veya daha fazla servis Running durumuna ulaşamadı.");

                UpdateStatus("Servisler çalışıyor. Uygulama başlatılıyor...", false);

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

                // Uygulama kapat komutu ver
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

                // Uygulamanın kapatıldığını kontrol et (process kalmayana kadar bekle)
                var appTimeout = TimeSpan.FromSeconds(10);
                var appStartTime = DateTime.Now;
                while (DateTime.Now - appStartTime < appTimeout)
                {
                    processes = Process.GetProcessesByName(exeName);
                    if (processes.Length == 0)
                        break;
                    Thread.Sleep(1000);
                }

                if (Process.GetProcessesByName(exeName).Length > 0)
                    throw new InvalidOperationException("EXE süreci kapatılamadı.");

                UpdateStatus("Uygulama kapatıldı. Servisler durduruluyor...", false);

                // Tüm servisler için stop komutu ver
                foreach (var svcName in pair.ServiceNames.AsEnumerable().Reverse())
                {
                    using var service = new ServiceController(svcName);
                    if (service.Status != ServiceControllerStatus.Stopped)
                    {
                        service.Stop();
                    }
                }

                // Tüm servisler Stopped olana kadar bekle ve kontrol et
                var svcTimeout = TimeSpan.FromSeconds(10);
                var svcStartTime = DateTime.Now;
                while (DateTime.Now - svcStartTime < svcTimeout)
                {
                    bool allStopped = true;
                    foreach (var svcName in pair.ServiceNames)
                    {
                        using var service = new ServiceController(svcName);
                        if (service.Status != ServiceControllerStatus.Stopped)
                        {
                            allStopped = false;
                            break;
                        }
                    }
                    if (allStopped)
                        break;
                    Thread.Sleep(1000);
                }

                // Son kontrol: hepsi durmuş mu?
                bool finalSvcCheck = pair.ServiceNames.All(svcName =>
                {
                    using var service = new ServiceController(svcName);
                    return service.Status == ServiceControllerStatus.Stopped;
                });

                if (!finalSvcCheck)
                    throw new InvalidOperationException("Bir veya daha fazla servis başarılı şekilde durmadı.");
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

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                dynamic shell = Activator.CreateInstance(shellType);
#pragma warning restore CS8600
                if (shell == null)
                    throw new InvalidOperationException("Shell instance oluşturulamadı.");

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                dynamic shortcut = shell.CreateShortcut(shortcutPath);
#pragma warning restore CS8600
                if (shortcut == null)
                    throw new InvalidOperationException("Shortcut oluşturulamadı.");

                var managerExe = Application.ExecutablePath;
                var cmdArgs = $"--toggle \"{pair.Name}\"";

                shortcut.TargetPath = managerExe;
                shortcut.Arguments = cmdArgs;
                shortcut.WorkingDirectory = Path.GetDirectoryName(managerExe) ?? string.Empty;
                shortcut.Description = "Akıllı Kısayol Yöneticisi tarafından oluşturuldu (Toggle Start/Stop).";
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

