using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Panuon.UI;
using System.Net;
using System.Windows.Threading;
using Panuon.UI.Silver;
using StarLight_Core.Authentication;
using StarLight_Core.Enum;
using StarLight_Core.Launch;
using StarLight_Core.Models.Authentication;
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
                Version = GameVersion.Text, // �����İ汾
                IsVersionIsolation = true, //�汾����
                // Nide8authPath = ".minecraft\\nide8auth.jar", // ֻ��ͳһͨ��֤��Ҫ
                // UnifiedPassServerId = "xxxxxxxxxxxxxxxxxx" // ͬ��
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
            MessageBox.Show("�����ɹ�");
            // �����ɹ�ִ�в���
        }
        else
        {
            MessageBox.Show("����ʧ��"+la.Exception);
        }
    }
    private void ReportProgress(ProgressReport progress)
    {
        Dispatcher.Invoke(() =>
        {
            Progress.Text = progress.Description + " " + progress.Percentage + "%";
        
    
            if (progress.Percentage >= 90)
            {
                Progress.Visibility= Visibility.Collapsed;
            }
        });
    }
    
    


    
}