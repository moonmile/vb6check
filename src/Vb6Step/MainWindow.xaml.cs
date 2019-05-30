using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vb6Step.ViewModels;

namespace Vb6Step
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        MainViewModel _vm;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = new MainViewModel();
            this.DataContext = _vm;
        }

        /// <summary>
        /// 単一の VBP を選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickOpenVBP(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "VB6プロジェクトファイル(*.vbp)|*.vbp";
            if ( dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK )
            {
                _vm.Filename = dlg.FileName;
            }
        }
        /// <summary>
        /// フォルダを指定して、複数の VBP を選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickOpenFolder(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            if ( dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK )
            {
                _vm.Filename = dlg.SelectedPath;
            }

        }

        /// <summary>
        /// ステップカウントを実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickRun(object sender, RoutedEventArgs e)
        {
            _vm.StepCount();
        }

        /// <summary>
        /// CSV形式で保存する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickSave(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "CSV形式(*.csv)|*.csv";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _vm.SaveCsv(dlg.FileName);
                System.Windows.MessageBox.Show("CSV形式で保存しました");
            }
        }
    }
}
