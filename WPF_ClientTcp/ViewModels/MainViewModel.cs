using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WPF_ClientTcp.Commands;
using WPF_ClientTcp.Services.NetworkService;
using WPF_ClientTcp.Views;

namespace WPF_ClientTcp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Properties

        public TcpClient TcpClient { get; set; }


        private string clientMessage;

        public string ClientMessage
        {
            get { return clientMessage; }
            set { clientMessage = value; OnPropertyChanged(); }
        }


        private string clientName;

        public string ClientName
        {
            get { return clientName; }
            set { clientName = value; OnPropertyChanged(); }
        }


        private string displayName;

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; OnPropertyChanged(); }
        }


        public BinaryReader BinaryReader { get; set; }

        public BinaryWriter BinaryWriter { get; set; }

        public bool IsConnected { get; set; }


        private string connectContent;

        public string ConnectContent
        {
            get { return connectContent; }
            set { connectContent = value; OnPropertyChanged(); }
        }

        public StackPanel MessagePanel { get; set; }

        #endregion

        ///---------------------------------------------------------------

        #region Commands
        public RelayCommand ConnectCommand { get; set; }
        public RelayCommand SendCommand { get; set; }
        public RelayCommand DisconnectCommand { get; set; }


        #endregion

        public DispatcherTimer messageReceiverTimer { get; set; }




        public async void ReceiveMessage()
        {
            messageReceiverTimer.Stop();
            await Task.Run(() =>
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {

                    try
                    {
                        var stream = TcpClient.GetStream();
                        BinaryReader = new BinaryReader(stream);


                        App.Current.Dispatcher.Invoke((Action)async delegate
                        {
                            while (true)
                            {
                                var view = new MessageUC();
                                var viewModel = new MessageViewModel();
                                view.DataContext = viewModel;

                                await Task.Run(() =>
                                {

                                    try
                                    {
                                        viewModel.ClientMessage = BinaryReader.ReadString();

                                        App.Current.Dispatcher.Invoke((Action)delegate
                                        {
                                            view.HorizontalAlignment = HorizontalAlignment.Left;
                                            MessagePanel.Children.Add(view);
                                        });

                                    }
                                    catch (Exception)
                                    {


                                    }


                                });
                            }

                        });

                    }
                    catch (Exception)
                    {

                        throw;
                    }

                });
            });
        }



        public MainViewModel()
        {
            ConnectContent = "Connect";
            TcpClient = new TcpClient();
            var ip = IPAddress.Parse(IPService.GetLocalIPAddress());
            var port = 27001;
            var endPoint = new IPEndPoint(ip, port);

            messageReceiverTimer = new DispatcherTimer();
            messageReceiverTimer.Interval = TimeSpan.FromSeconds(3);
            messageReceiverTimer.Tick += MessageReceiverTimer_Tick;
            ConnectCommand = new RelayCommand((c) =>
            {
                Task.Run(() =>
                {
                    try
                    {
                        TcpClient.Connect(endPoint);
                        var stream = TcpClient.GetStream();
                        BinaryWriter = new BinaryWriter(stream);
                        BinaryWriter.Write(ClientName);
                        DisplayName = ClientName;
                        ClientName = "";
                        IsConnected = true;
                        ConnectContent = "Connected";

                        messageReceiverTimer.Start();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("You are already connected or connection was not sucessfull");
                        ClientName = "";
                    }
                });
            }, (a) =>
            {
                if (!IsConnected)
                {
                    if (ClientName != null)
                    {
                        if (ClientName.Length != 0)
                        {
                            return true;
                        }
                        return false;
                    }
                    return false;
                }
                else if (IsConnected)
                    return false;
                return false;
            });



            SendCommand = new RelayCommand((c) =>
            {
                if (TcpClient.Connected)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        var stream = TcpClient.GetStream();
                        BinaryWriter = new BinaryWriter(stream);
                        BinaryWriter.Write(ClientMessage);

                        var view = new MessageUC();
                        var viewModel = new MessageViewModel();
                        view.DataContext = viewModel;
                        viewModel.ClientMessage = ClientMessage;


                        view.HorizontalAlignment = HorizontalAlignment.Right;
                        MessagePanel.Children.Add(view);
                        ClientMessage = "";
                    });


                }
            });
        }

        private void MessageReceiverTimer_Tick(object sender, EventArgs e)
        {
            ReceiveMessage();
        }
    }
}
