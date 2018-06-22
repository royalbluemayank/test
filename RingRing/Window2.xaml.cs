using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RingRing
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    //public partial class Window2 : Window
    //{
    //    BackgroundWorker worker;

    //    public Window2()
    //    {
    //        InitializeComponent();
    //        CancelButton.IsEnabled = false;
    //    }
    //    int i = 0;
    //    private void StartButton_Click(object sender, RoutedEventArgs e)
    //    {
    //        //ListBox1.Items.Clear();
    //        ListBox1.Items.Add(new ListBoxItem { Content = i++ + " Pressed" });

    //       // StartButton.IsEnabled = false;
    //        CancelButton.IsEnabled = true;

    //        worker = new BackgroundWorker();
    //        worker.DoWork += worker_DoWork;
    //        worker.ProgressChanged += worker_ProgressChanged;
    //        worker.RunWorkerCompleted += worker_RunWorkerCompleted;
    //        worker.WorkerReportsProgress = true;
    //        worker.WorkerSupportsCancellation = true;

    //        int maxItems = 50;
    //        ProgressBar1.Minimum = 1;
    //        ProgressBar1.Maximum = 20;

    //        StatusTextBox.Text = "Starting...";
    //        int[] o = new int[2];
    //        o[0] = maxItems;
    //        o[1] = i;
    //        worker.RunWorkerAsync(o);
    //    }

    //    void worker_DoWork(object sender, DoWorkEventArgs e)
    //    {
    //        BackgroundWorker worker = sender as BackgroundWorker;
    //        int[] o  = e.Argument as int[];
    //        int? maxItems = o[0];
    //        int? j = o[1];
    //        for (int i = 1; i <= 2; ++i)
    //        {
    //            if (worker.CancellationPending)
    //            {
    //                e.Cancel = true;
    //                break;
    //            }

    //            Thread.Sleep(200);
    //            worker.ReportProgress(i);

    //            //item added
    //        }
    //    }

    //    private void CancelButton_Click(object sender, RoutedEventArgs e)
    //    {
    //        worker.CancelAsync();
    //    }

    //    void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    //    {
    //        if (e.Cancelled)
    //        {
    //            StatusTextBox.Text = "Cancelled";
    //        }
    //        else
    //        {
    //            StatusTextBox.Text = "Completed";
    //        }
    //        StartButton.IsEnabled = true;
    //        CancelButton.IsEnabled = false;
    //    }

    //    void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    //    {
    //        double percent = (e.ProgressPercentage * 100) / 50;

    //        ProgressBar1.Value = Math.Round(percent, 0);

    //        ListBox1.Items.Add(new ListBoxItem { Content = e.ProgressPercentage + " item added" });
    //        ListBox1.Items.Add(new ListBoxItem { Content = e.UserState + " User state" });
    //        StatusTextBox.Text = Math.Round(percent, 0) + "% percent completed";
    //    }
    //}
    public partial class Window2 : Window
    {
        BackgroundWorker m_oWorker;
        private Object thisLock = new Object();
        List<Employee> oEmp;

        public Window2()
        {
            InitializeComponent();

            oEmp = new List<Employee>();
            oEmp.Add(new Employee(1, "Dipak"));
            oEmp.Add(new Employee(2, "Anuj"));
            oEmp.Add(new Employee(3, "Siddharth"));
            oEmp.Add(new Employee(4, "Parul"));
            CancelButton.IsEnabled = false;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            //ListBox1.Items.Clear();
            m_oWorker = new BackgroundWorker();
            CancelButton.IsEnabled = true;
            // Create a background worker thread that ReportsProgress &
            // SupportsCancellation
            // Hook up the appropriate events.
            m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWork);
            m_oWorker.ProgressChanged += new ProgressChangedEventHandler
                    (m_oWorker_ProgressChanged);
            m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler
                    (m_oWorker_RunWorkerCompleted);
            m_oWorker.WorkerReportsProgress = true;
            m_oWorker.WorkerSupportsCancellation = true;
            Employee emp = oEmp[j++];
            m_oWorker.RunWorkerAsync(emp);

        }

        int j = 0;
        void m_oWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // The sender is the BackgroundWorker object we need it to
            // report progress and check for cancellation.
            //NOTE : Never play with the UI thread here...
          
            Employee emp = (Employee)e.Argument;
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep( j * 500);

                // Periodically report progress to the main thread so that it can
                // update the UI.  In most cases you'll just need to send an
                // integer that will update a ProgressBar     

                //oEmp.Add(new Employee(1, "Dipak"));
                m_oWorker.ReportProgress(i, emp);
                // Periodically check if a cancellation request is pending.
                // If the user clicks cancel the line
                // m_AsyncWorker.CancelAsync(); if ran above.  This
                // sets the CancellationPending to true.
                // You must check this flag in here and react to it.
                // We react to it by setting e.Cancel to true and leaving
                if (m_oWorker.CancellationPending)
                {
                    // Set the e.Cancel flag so that the WorkerCompleted event
                    // knows that the process was cancelled.
                    e.Cancel = true;
                    m_oWorker.ReportProgress(0);
                    return;
                }
            }

            //Report 100% completion on operation completed
            m_oWorker.ReportProgress(100);
        }

        void m_oWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lock (thisLock)
            {
                Employee oEmp = (Employee)e.UserState;
                if (oEmp != null)
                  //  ContainerPanel.Controls.Add(new Label { Text = oEmp.Name });
                      ListBox1.Items.Add(new ListBoxItem { Content = oEmp.Name + " item added" + e.ProgressPercentage.ToString() + "%" });
                StatusTextBox.Text = "Processing......" + e.ProgressPercentage.ToString() + "%";
            }
        }

        void m_oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // The background process is complete. We need to inspect
            // our response to see if an error occurred, a cancel was
            // requested or if we completed successfully.  
            if (e.Cancelled)    
            {
                StatusTextBox.Text = "Task Cancelled.";
            }

            // Check to see if an error occurred in the background process.

            else if (e.Error != null)
            {
                StatusTextBox.Text = "Error while performing background operation.";
            }
            else
            {
                // Everything completed normally.
                StatusTextBox.Text = "Task Completed...";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Employee emp = new Employee(oEmp.Count + 1, textBox1.Text);
            oEmp.Add(emp);
            ListBox1.Items.Add(new ListBoxItem { Content = emp.Name + " fresh item added" });


            m_oWorker = new BackgroundWorker();
            // Create a background worker thread that ReportsProgress &
            // SupportsCancellation
            // Hook up the appropriate events.
            m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWork);
            m_oWorker.ProgressChanged += new ProgressChangedEventHandler
                    (m_oWorker_ProgressChanged);
            m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler
                    (m_oWorker_RunWorkerCompleted);
            m_oWorker.WorkerReportsProgress = true;
            m_oWorker.WorkerSupportsCancellation = true;
            m_oWorker.RunWorkerAsync(emp);


            // m_oWorker.CancelAsync();
        }
    }

    public class Employee
    {
        internal int ID = 0;
        internal string Name = "";

        public Employee()
        {

        }

        public Employee(int ID, string Name)
        {
            this.ID = ID;
            this.Name = Name;
        }
    }
}
