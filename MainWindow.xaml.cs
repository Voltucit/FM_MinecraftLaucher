using System.Windows;
using Panuon.WPF.UI;
using StarLight_Core.Authentication;
using StarLight_Core.Enum;
using StarLight_Core.Launch;
using StarLight_Core.Models.Launch;
using StarLight_Core.Utilities;


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
                MaxMemory = 4096,
                MinMemory = 1000
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
    


    
}