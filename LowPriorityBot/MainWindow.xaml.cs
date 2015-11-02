using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LowPriorityBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ShitPoster shitposter;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            shitposter = new ShitPoster();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            button_start.IsEnabled = false;
            shitposter.login(username.Text, password.Password);
            shitposter.shitpostMessage = shitpostContent.Text;
            username.IsEnabled = false;
            password.IsEnabled = false;
            worker.RunWorkerAsync();
            //this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
            //{
            //    shitposter.login(username.Text, password.Password);
            //    shitposter.shitpostMessage = shitpostContent.Text;
            //    shitposter.startShitposting();
            //}));
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //this.Dispatcher.BeginInvoke(DispatcherPriority.Background ,(Action)(() =>
            //{
            //    shitposter.login(username.Text, password.Password);
            //    shitposter.shitpostMessage = shitpostContent.Text;
            //    shitposter.startShitposting();
            //}));
            
            shitposter.startShitposting();

        }

        private void worker_RunWorkerCompleted(object sender,
                                               RunWorkerCompletedEventArgs e)
        {
            username.IsEnabled = true;
            password.IsEnabled = true;
            button_start.IsEnabled = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            shitposter.isJobDone = true;
            shitposter.stop();
        }
    }
}
