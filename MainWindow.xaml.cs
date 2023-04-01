using System;
using System.Windows;
using System.IO.Ports;
using System.Collections.ObjectModel;

namespace COMtest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> COMlist = new ObservableCollection<string>(); //holds available COM ports
        private static SerialPort serialPort;
        
        public MainWindow()
        {
            InitializeComponent();
            serialPort = new SerialPort();
            serialPort.DataReceived += SerialPort_DataReceived; //subscribing to the DataReceived event from SerialPort
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e) //Event handler
        {
            string indata = serialPort.ReadLine();
            SerialRxTextBox.Dispatcher.Invoke(() => { SerialRxTextBox.Text += indata + "\n"; }); //Sets text of the TextBox to the data received from serial port
        }
        private void COMSelect_Initialized(object sender, EventArgs e) //Event handler for Initialization of the COMSelect ComboBox
        {
            foreach (string s in SerialPort.GetPortNames())
            {
                COMlist.Add(s); //Add all existing ports to collection
            }

            if(COMlist.Count > 0)
            {
                COMSelect.ItemsSource = COMlist; //Bind the list of ports to the ComboBox elements
            }
            else
            {
                StatusText.Text = "No COM ports available!"; //In case of lack of COM ports
                ConnectButton.IsEnabled = false; 
                SendTxButton.IsEnabled = false;
                COMSelect.IsEnabled = false;  
            }
            
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (COMSelect.SelectedItem != null && !serialPort.IsOpen) //True if there are serial ports available and the serial port is not already opened
            {
                //Serial port initialization
                serialPort.PortName = COMSelect.SelectedItem.ToString();
                serialPort.BaudRate = 9600;

                serialPort.Open();

                StatusText.Text = "Connected to " + serialPort.PortName; //Status messaging
                ConnectButton.Content = "Disconnect";
            }
            else if (COMSelect.SelectedItem == null)
            {
                StatusText.Text = "Select COM Port!";
            }
            else if (serialPort.IsOpen) //allows to disconnect form chosen serial port
            {
                serialPort.Close();
                StatusText.Text = "Disconnected";
                ConnectButton.Content = "Connect";
                COMSelect.SelectedIndex = -1;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            serialPort.Close();
        }

        private void SendTxButton_Click(object sender, RoutedEventArgs e) //Transmits data from TextBox over the serial port
        {
            if(serialPort.IsOpen) 
            {
                serialPort.WriteLine(SerialTxTextBox.Text);
            }
        } 
    }
}
