using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSystemWPF.ViewModel
{
    public class SignalR
    {
        HubConnection _connection;
        public string Address { get; set; } = "http://localhost:5147";
        public void CreateConnection()
        {
            _connection = new HubConnectionBuilder().
                            AddJsonProtocol(s =>
                            {
                                s.PayloadSerializerOptions.ReferenceHandler =
                                System.Text.Json.Serialization.ReferenceHandler.Preserve;
                            }
            ).
                        WithUrl(Address + "/chat").
            Build();

            _connection.StartAsync();

            //Unloaded += async (s, e) => await _connection.StopAsync();
        }
        public void StopConnection()
        {
            _connection.StopAsync();
        }

        static SignalR instance = new SignalR();
        public static SignalR Instance
        {
            get
            {
                if (instance == null)
                    instance = new SignalR();
                return instance;
            }
        }

    }
}
