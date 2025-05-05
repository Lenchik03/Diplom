using ChatServerDTO.DTO;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace ProjectSystemWPF.ViewModel
{
    public class SignalR
    {
        HubConnection _connection;
        public string Address { get; set; } = "http://localhost:5147";
        public HubConnection CreateConnection()
        {
            if (_connection?.State == HubConnectionState.Connected)
                return _connection;
            _connection = new HubConnectionBuilder().
                            WithAutomaticReconnect().
                            AddJsonProtocol(s =>
                            {
                                s.PayloadSerializerOptions.ReferenceHandler =
                                System.Text.Json.Serialization.ReferenceHandler.Preserve;
                                s.PayloadSerializerOptions.DefaultIgnoreCondition =
                                System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                            }
            ).
                        WithUrl(Address + "/chat").
            Build();
            
            _connection.StartAsync();

            HubMethods();
            return _connection;

            //Unloaded += async (s, e) => await _connection.StopAsync();
        }
        public void StopConnection()
        {
            _connection.StopAsync();
        }

        public event EventHandler<int> OnMessage;

        private void HubMethods()
        {
            _connection.On<string>("welcome", (username) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Notifier notifier = new Notifier(cfg =>
                    {
                        cfg.PositionProvider = new WindowPositionProvider(
                            parentWindow: System.Windows.Application.Current.MainWindow,
                            corner: Corner.TopRight,
                            offsetX: 10,
                            offsetY: 10);

                        cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                            notificationLifetime: TimeSpan.FromSeconds(3),
                            maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                        cfg.Dispatcher = System.Windows.Application.Current.Dispatcher;
                    });

                    //OnMessage?.Invoke(this, chatId);

                    //var notify = new ToastContentBuilder();
                    //notify.AddText("Новое сообщение!");
                    //notify.AddArgument($"Вам пришло новое сообщение в чате {chat.Title}");
                    //notify.AddArgument(mess);
                    //notify.GetToastContent();
                    //var toast = notify.GetToastContent();
                    notifier.ShowInformation($"Привет, {username}!");
                });
            });
            _connection.On<MessageDTO, int, string>("newMessage", (mess, chatId, chatTitle) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Notifier notifier = new Notifier(cfg =>
                    {
                        cfg.PositionProvider = new WindowPositionProvider(
                            parentWindow: System.Windows.Application.Current.MainWindow,
                            corner: Corner.TopRight,
                            offsetX: 10,
                            offsetY: 10);

                        cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                            notificationLifetime: TimeSpan.FromSeconds(3),
                            maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                        cfg.Dispatcher = System.Windows.Application.Current.Dispatcher;
                    });

                    OnMessage?.Invoke(this, chatId);

                    //var notify = new ToastContentBuilder();
                    //notify.AddText("Новое сообщение!");
                    //notify.AddArgument($"Вам пришло новое сообщение в чате {chat.Title}");
                    //notify.AddArgument(mess);
                    //notify.GetToastContent();
                    //var toast = notify.GetToastContent();
                    notifier.ShowInformation($"Вам пришло новое сообщение в чате {chatTitle}");
                });
            });
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
