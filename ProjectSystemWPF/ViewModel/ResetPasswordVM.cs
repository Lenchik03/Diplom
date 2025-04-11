using ProjectSystemAPI.DB;
using ProjectSystemWPF.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static MaterialDesignThemes.Wpf.Theme;

namespace ProjectSystemWPF.ViewModel
{
    public class ResetPasswordVM:BaseVM
    {
        public VmCommand Save {  get; set; }
        private System.Windows.Controls.PasswordBox oldpasswrdBox;
        private System.Windows.Controls.PasswordBox newpasswrdBox;
        public string OldPassword 
        {
            get { return oldpasswrdBox.Password; }
            set
            {
                oldpasswrdBox.Password = value;      
            }
        }
        public string NewPassword
        {
            get { return newpasswrdBox.Password; }
            set
            {
                newpasswrdBox.Password = value;
            }
        }  

        public ResetPasswordVM()
        {
            Save = new VmCommand(async () =>
            {
                if (OldPassword != null)
                {
                    if (ActiveUser.GetInstance().User.Password == Md5.HashPassword(OldPassword))
                    {
                        if (NewPassword != null)
                        {
                            ActiveUser.GetInstance().User.Password = Md5.HashPassword(NewPassword);
                            string arg = JsonSerializer.Serialize(ActiveUser.GetInstance().User, REST.Instance.options);
                            var responce = await REST.Instance.client.PutAsync($"Users/UpdateUser",
                                new StringContent(arg, Encoding.UTF8, "application/json"));
                            try
                            {
                                responce.EnsureSuccessStatusCode();
                                MessageBox.Show("Пароль успешно изменен!");

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Ошибка! Обновление пароля приостановлено!");
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Введите новый пароль!");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Старый пароль неверный");
                    }
                }
                else
                {
                    MessageBox.Show("Введите старый пароль");
                    return;
                    
                }
                passwordWindow.Close();
            });
        }

        internal void SetPasswords(System.Windows.Controls.PasswordBox oldpass, System.Windows.Controls.PasswordBox newpass)
        {
            oldpasswrdBox = oldpass;
            newpasswrdBox = newpass;
            oldpass.PasswordChanged += EventPassword;
            newpass.PasswordChanged += EventPassword;
        }

        private void EventPassword(object sender, RoutedEventArgs e)
        {
            Signal(nameof(OldPassword));
            Signal(nameof(NewPassword));
        }

        ResetPasswordWindow passwordWindow;
        
        private string oldPassword;
        private string newPassword;

        internal void SetPage(ResetPasswordWindow resetPasswordWindow)
        {
            passwordWindow = resetPasswordWindow;
            
        }
    }
}
