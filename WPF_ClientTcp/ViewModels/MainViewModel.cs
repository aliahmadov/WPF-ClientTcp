using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPF_ClientTcp.Commands;
using WPF_ClientTcp.Services.NetworkService;

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


        public BinaryReader BinaryReader { get; set; }

        public BinaryWriter BinaryWriter { get; set; }

        #endregion

        ///---------------------------------------------------------------

        #region Commands
        public RelayCommand ConnectCommand { get; set; }
        public RelayCommand SendCommand { get; set; }

        #endregion






        public MainViewModel()
        {
            TcpClient = new TcpClient();
            var ip = IPAddress.Parse(IPService.GetLocalIPAddress());
            var port = 27001;
            var endPoint = new IPEndPoint(ip, port);

            ConnectCommand = new RelayCommand(c =>
            {
                try
                {
                    TcpClient.Connect(endPoint);
                    var stream = TcpClient.GetStream();
                    BinaryWriter = new BinaryWriter(stream);
                    BinaryWriter.Write(ClientName);
                }
                catch (Exception)
                {
                    MessageBox.Show("Could not connect");
                }
            }, (a) =>
            {
                if(ClientName != null)
                {
                    if (ClientName.Length != 0)
                    {
                        return true;
                    }
                    else return false;
                    return true;
                }
                return false;
            });


            SendCommand = new RelayCommand(c =>
            {
                if (TcpClient.Connected)
                {

                }
            });
        }
    }
}
