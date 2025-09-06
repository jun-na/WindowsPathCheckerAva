using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using WindowsPathCheckerAva.Services;

namespace WindowsPathCheckerAva.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<string> Items { get; } = new();

    public RelayCommand CheckCommand { get; } 

    public MainWindowViewModel()
    {
        CheckCommand = new RelayCommand(OnCheck);
    }

    private async void OnCheck()
    {
        string? tempExecutablePath = null;
        try
        {
            // 埋め込まれたリソースからexeを抽出
            tempExecutablePath = await EmbeddedResourceExtractor.ExtractEmbeddedExecutableAsync(
                "WindowsPathChecker.exe", 
                "WindowsPathChecker.exe");

            // ProcessStartInfo を使用してプロセスを起動する設定を行う
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = tempExecutablePath,
                // 標準出力をリダイレクトする
                RedirectStandardOutput = true,
                // ウィンドウを表示しない
                CreateNoWindow = true,
                // シェルを使用しない
                UseShellExecute = false
            };

            // プロセスを起動
            var process = System.Diagnostics.Process.Start(startInfo);
            if (process == null)
            {
                // プロセスの起動に失敗した場合の処理
                return;
            }

            // プロセスの終了を待機
            process.WaitForExit();

            // 標準出力からデータを読み取る
            var output = process.StandardOutput.ReadToEnd();

            Items.Clear();
            foreach (var line in output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                Items.Add(line);
            }
        }
        catch (Exception ex)
        {
            Items.Clear();
            Items.Add($"Error: {ex.Message}");
        }
        finally
        {
            // 一時ファイルをクリーンアップ
            if (tempExecutablePath != null)
            {
                EmbeddedResourceExtractor.CleanupTempFile(tempExecutablePath);
            }
        }
    }
}
