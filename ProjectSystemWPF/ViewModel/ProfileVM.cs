using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using ProjectSystemWPF.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectSystemWPF.ViewModel
{
    public class ProfileVM
    {
        public UserDTO User { get; set; }
        public VmCommand ResetPassword {  get; set; }
        public VmCommand Save {  get; set; }

        public ProfileVM()
        {
            User = ActiveUser.GetInstance().User;
            Save = new VmCommand(async () =>
            {
                string arg = JsonSerializer.Serialize(User, REST.Instance.options);
                var responce = await REST.Instance.client.PutAsync($"Users/UpdateUser",
                    new StringContent(arg, Encoding.UTF8, "application/json"));
                try
                {
                    responce.EnsureSuccessStatusCode();
                    MessageBox.Show("Пользователь успешно обновлен!");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка! Обновление пользователя приостановлено!");
                    return;
                }
            });
            ResetPassword = new VmCommand(async () =>
            {
                ResetPasswordWindow passwordWindow = new ResetPasswordWindow();
                passwordWindow.Show();
            });
        }
    }
}

