using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Panuon.WPF.UI;
using StarLight_Core.Authentication;
using StarLight_Core.Enum;
using StarLight_Core.Launch;
using StarLight_Core.Models.Launch;
using StarLight_Core.Utilities;
using System.Management;
using System.Windows.Controls;
using Panuon.WPF.UI.Configurations;
using StarLight_Core.Models.Authentication;
using Newtonsoft.Json;

namespace ��ȴ������_EP;
using System.Net;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : WindowX
{
   
    public MainWindow()
    {
      InitializeComponent();
      
      
      GetGameVer();
      GetJava();
      ConfigSet();
      var setting = Application.Current.FindResource("toastSetting") as ToastSetting;

    }

    void GetGameVer()
    {
        GameVersion.DisplayMemberPath = "Id";
        GameVersion.SelectedValuePath = "Id";
        GameVersion.ItemsSource = GameCoreUtil.GetGameCores();
        
    }
    
    
    void GetJava()
    {
        JavaPath.DisplayMemberPath = "JavaPath";
        JavaPath.SelectedValuePath = "JavaPath";
        JavaPath.ItemsSource = JavaUtil.GetJavas();
        
        
    }


    void ConfigSet()
    {
        var config = JsonUtil.Load();
        LoginMod.SelectedIndex = config.LoginMode;
        PlayerName.Text  = config.Playername;
        MemorySlider.Value = config.Memory;
        if (!string.IsNullOrEmpty(config.GameVersion))
        {
            GameVersion.SelectedItem = GameVersion.Items
                .Cast<dynamic>()
                .FirstOrDefault(i => i.Id == config.GameVersion);
        }

        // ���� Java ·��
        if (!string.IsNullOrEmpty(config.JavaPath))
        {
            JavaPath.SelectedItem = JavaPath.Items
                .Cast<dynamic>()
                .FirstOrDefault(j => j.JavaPath == config.JavaPath);
        }
    }

    private static ulong GetTotalMemory()
    {
        try
        {
            var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
            foreach (ManagementObject obj in searcher.Get())
            {
                return Convert.ToUInt64(obj["TotalPhysicalMemory"]);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"��ȡ�ڴ�ʧ��: {ex.Message}");
        }
        return 0;
    }
    

    
    
    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        BaseAccount account;
        
        if (LoginMod.SelectedIndex==0)
        {
            var auth=new MicrosoftAuthentication("a9088867-a8c4-4d8d-a4a1-48a4eacb137b");
            var code =await  auth.RetrieveDeviceCodeInfo();
            Clipboard.Clear();
            Clipboard.SetText(code.UserCode);
            MessageBoxX.Show("��¼����:" + code.UserCode + "  �Ѹ��Ƶ�������");
            try
            {
                Process.Start(new ProcessStartInfo(code.VerificationUri)
                {
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�޷��������: {ex.Message}");
            }
            var token = await auth.GetTokenResponse(code);
            account = await auth.MicrosoftAuthAsync(token, x =>
            {
                UserToken.Text = x;
            });
        }
        else
        {
              account = new OfflineAuthentication(PlayerName.Text).OfflineAuth();
        }
        LaunchConfig args = new() // ������������
        {
            Account = new()
            {
                BaseAccount = account // �˻�
            },
            GameCoreConfig = new()
            {
                Root = ".minecraft", // ��Ϸ��Ŀ¼(�����Ǿ��Ե�Ҳ��������Ե�,�Զ��ж�)
                Version =GameVersion.Text, // �����İ汾
                IsVersionIsolation = true, //�汾����
            
            },
            JavaConfig = new()
            {
                JavaPath = JavaPath.Text, // Java ·��(����·��)
                MaxMemory = (int)(MemorySlider.Value*1024),
                MinMemory = (int)(MemorySlider.Minimum*1024)
            }
        };
        var launch = new MinecraftLauncher(args); // ʵ����������
        var la = await launch.LaunchAsync(ReportProgress); // ����

// ��־���
        la.ErrorReceived += (output) => Console.WriteLine($"{output}");
        la.OutputReceived += (output) => Console.WriteLine($"{output}");

        if (la.Status == Status.Succeeded)
        {
            Panuon.WPF.UI.Toast.Show("�����ɹ�");

            // �����ɹ�ִ�в���
        }
        else
        {
            MessageBoxX.Show("����ʧ��"+la.Exception);
        }
    
    }
    private void ReportProgress(ProgressReport progress)
    {

    }


    private void MemorySlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        ulong totalMemoryBytes = GetTotalMemory();
        double totalMemoryMb = totalMemoryBytes / (1024.0 * 1024.0*1024);
        int totalMemoryMbInt = (int)Math.Round(totalMemoryMb); // �������������Ϊ����
        MemorySlider.Maximum = totalMemoryMbInt;
    }

    private void LoginMod_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LoginMod.SelectedIndex==0)
        {
            PlayerName.Visibility = Visibility.Collapsed;
            PlayerNames.Visibility = Visibility.Collapsed;
        }
        else
        {
            PlayerName.Visibility = Visibility.Visible;
            PlayerNames.Visibility = Visibility.Visible;
        }
    }

    private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        var config = new LauncherSettings()
        {
            LoginMode = LoginMod.SelectedIndex,
            Playername = PlayerName.Text,
            GameVersion = (GameVersion.SelectedItem as dynamic)?.Id ?? "",
            JavaPath = (JavaPath.SelectedItem as dynamic)?.JavaPath ?? "",
            Memory =  MemorySlider.Value
        };
        JsonUtil.Save(config);
    }
}