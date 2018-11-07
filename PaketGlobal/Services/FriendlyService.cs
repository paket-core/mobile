using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Xamarin.Forms;

namespace PaketGlobal
{

    public class ConnectivityTypeChangedEventArgs : EventArgs
    {
        public bool IsConnected { get; set; }
        public IEnumerable<ConnectionType> ConnectionTypes { get; set; }
    }
    public delegate void ConnectivityTypeChangedEventHandler(object sender, ConnectivityTypeChangedEventArgs e);

    public class FriendlyService
    {
        HttpClient httpClient = new HttpClient();

        private System.Timers.Timer timer;
        public bool Paused = false;
        public bool IsRouteWorking = true;
        public bool IsBridgeWorking = true;
        public bool IsFundWorking = true;
        int count = 0;

        public FriendlyService()
        {
            Run();
            MonitorConnection();
        }

        private void MonitorConnection()
        {
            CrossConnectivity.Current.ConnectivityTypeChanged += (sender, args) =>
            {
                if (args.IsConnected == false)
                {
                    MessagingCenter.Send<string, string>(Constants.NOTIFICATION, Constants.NO_INERNET_CONNECTION, "");
                }
            };
        }

        public bool IsConnected()
        {
            return CrossConnectivity.Current.IsConnected;
        }

        private async void Run()
        {
            StopTimer();

            bool isWorking = await CheckServers();

            StartTimer();

            //count++;

            if (!isWorking)
            {
                MessagingCenter.Send<string, string>(Constants.NOTIFICATION, Constants.SERVERS_NOT_WORKING, "");
            }

            //if(count>=8)
            //{
            //    MessagingCenter.Send<string, string>(Constants.NOTIFICATION, Constants.SERVERS_NOT_WORKING, "");
            //}
        }

        private void StartTimer()
        {
            if (Paused == false)
            {
                if (timer != null)
                {
                    timer.Enabled = true;
                    timer.Start();
                }
                else
                {
                    CreateTimer();
                }
            }
        }

        private void StopTimer()
        {
            if (timer != null)
            {
                timer.Close();
                timer.Stop();
                timer.Enabled = false;
                timer = null;
            }
        }

        public void Pause()
        {
            Paused = true;

            StopTimer();
        }

        public void Resume()
        {
            Paused = false;

            StartTimer();
        }

        private void CreateTimer()
        {
            if (timer == null)
            {
                timer = new System.Timers.Timer();
                //Execute the function every 10 seconds.
                timer.Interval = 10000;
                //Don't start automaticly the timer.
                timer.AutoReset = false;
                //Attach a function to handle.
                timer.Elapsed += (sender, e) => Run();
                //Start timer.
                timer.Start();
            }
        }


        public async Task<bool> CheckServers()
        {

            try
            {
                HttpRequestMessage bridgeRequest = new HttpRequestMessage(HttpMethod.Head,
                               new Uri(Config.BridgeServerUrl + "/"));

                HttpResponseMessage bridgeResponse = await httpClient.SendAsync(bridgeRequest);

                if (bridgeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    IsBridgeWorking = false;
                    
                    return false;
                }

                HttpRequestMessage routeRequest = new HttpRequestMessage(HttpMethod.Head,
                                      new Uri(Config.RouteServerUrl + "/"));

                HttpResponseMessage routeResponse = await httpClient.SendAsync(routeRequest);

                if (routeResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    IsRouteWorking = false;
                    
                    return false;
                }

                HttpRequestMessage idReqieust = new HttpRequestMessage(HttpMethod.Head,
                                      new Uri(Config.IdentityServerUrl + "/"));

                HttpResponseMessage idResponse = await httpClient.SendAsync(idReqieust);

                if (idResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    IsFundWorking = false;
                }
                else{
                    IsFundWorking = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
            IsRouteWorking = true;
            IsBridgeWorking = true;

            return true;
        }
    }
}
