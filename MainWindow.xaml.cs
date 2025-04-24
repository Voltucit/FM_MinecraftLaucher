using System.IO;
using System.Windows;
using Panuon.WPF.UI;
using StarLight_Core.Authentication;
using StarLight_Core.Enum;
using StarLight_Core.Launch;
using StarLight_Core.Models.Launch;
using StarLight_Core.Utilities;
using System.Management;


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


    public static ulong GetTotalMemory()
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
       
        var account = new OfflineAuthentication(PlayerName.Text).OfflineAuth();
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
                MaxMemory = (int)MemorySlider.Value,
                MinMemory = (int)MemorySlider.Minimum
            }
        };
        var launch = new MinecraftLauncher(args); // ʵ����������
        var la = await launch.LaunchAsync(ReportProgress); // ����

// ��־���
        la.ErrorReceived += (output) => Console.WriteLine($"{output}");
        la.OutputReceived += (output) => Console.WriteLine($"{output}");

        if (la.Status == Status.Succeeded)
        {
            NoticeBox.Show("�����ɹ�", "��ȴ������Extreme", MessageBoxIcon.Info);
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
        double totalMemoryMb = totalMemoryBytes / (1024.0 * 1024.0);
        int totalMemoryMbInt = (int)Math.Round(totalMemoryMb); // �������������Ϊ����
        MemorySlider.Maximum = totalMemoryMbInt;
    }
}