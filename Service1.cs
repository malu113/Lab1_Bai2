using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CheckProcess
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds
            timer.Enabled = true;

        }
        //hàm kiểm tra một “process” ở trạng thái hoạt động run/stop hay không
        //Nếu có=>Ghi xuống logs trạng thái của process, đồng thời gọi phương thức Kill() để dừng process
        //Nếu không=>Ghi xuống logs trạng thái của process, đồng thời gọi phương thức Strart("processname") để khởi chạy process đó.
        private void IsProcessRunning(string sProcessName)
        {
            System.Diagnostics.Process[] proc = System.Diagnostics.Process.GetProcessesByName(sProcessName);
            if (proc.Length > 0)
            {
                WriteToFile(sProcessName + " is running");
                foreach (var p in proc)
                    p.Kill();
            }
            else
            {
                WriteToFile(sProcessName + " stopped");
                Process.Start(sProcessName);
            }
        }
        //Hàm xử lí nghiệp vụ kiểm tra process(cụ thể là notepad), được gọi 1 cách tuần hoàn theo lịch biểu ở hàm OnStart() của service
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WriteToFile("Service is recall at " + DateTime.Now);
            IsProcessRunning("notepad");
        }
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory +
           "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') +
           ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);
        }
    }
}

