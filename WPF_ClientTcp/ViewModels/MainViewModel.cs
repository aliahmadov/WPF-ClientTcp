using System;
using System.Collections.Generic;
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
                    
                }
                catch (Exception)
                {
                    MessageBox.Show("Could not connect");
                }
            });


            SendCommand = new RelayCommand(c =>
            {
                if(TcpClient.Connected)
                {

                }
            });
        }
    }
}
