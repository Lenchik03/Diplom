using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using ProjectSystemWPF.Model;
using ProjectSystemWPF.View;
using MaterialDesignThemes.Wpf;

namespace ProjectSystemWPF.ViewModel
{
    public class LoginVM : BaseVM, INotifyDataErrorInfo
    {
        MainVM mainVM;
        public VmCommand OpenPage { get; }

        Dictionary<string, List<string>> Errors = new Dictionary<string, List<string>>();
        public bool HasErrors => Errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

       

        public LoginVM()
        {
            // команда на открытие страницы сотрудника или руководителя
            OpenPage = new VmCommand(async () =>
            {
                var result = await REST.Instance.client.GetAsync($"Users/ActiveUser{Login} {Password}");
                //todo not ok

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return;
                }
                else
                {
                    ActiveUser.GetInstance().User = await result.Content.ReadFromJsonAsync<User>(REST.Instance.options);
               
                }

                if (Validator.TryValidateObject(this, new ValidationContext(this), null))
                {
                    //var customer = UserRepository.Instance.Customer(Login, passwrdBox.Password);
                    var user = ActiveUser.GetInstance().User;
                    if (user.Id == 0)
                    {
                        MessageBox.Show("Неверный логин или пароль");
                    }

                    else
                    {
                        if (user.IdRole == 3) // открытие страницы сотрудника
                        {

                            //страница полученных проектов 
                        }
                        else if(user.IdRole == 2)
                        {
                            //страница полученных проектов (от директора)
                        }
                        else if(user.IdRole == 4)
                        {
                            SuperUserPage superUserPage = new SuperUserPage();
                            MainVM.Instance.CurrentPage = superUserPage;
                        }

                        else // открытие страницы руководителя
                        {
                            //страница отправленных проектов (ты директор)
                        }

                    }
                }


            });
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            if (Errors.ContainsKey(propertyName))
            {
                return Errors[propertyName];
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }

        public void Validate(string propertyName, object propertyValue) // валидация
        {
            var results = new List<ValidationResult>();

            Validator.TryValidateProperty(propertyValue, new ValidationContext(this) { MemberName = propertyName }, results);

            if (results.Any())
            {
                if (Errors.ContainsKey(propertyName))
                    Errors.Remove(propertyName);
                Errors.Add(propertyName, results.Select(r => r.ErrorMessage).ToList());
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
            else
            {
                Errors.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            OpenPage.RaiseCamExecuteChanged();
        }

        internal void SetPasswordBox(PasswordBox passwrdBox)
        {
            this.passwrdBox = passwrdBox;
            passwrdBox.PasswordChanged += EventPassword;
        }
        internal void SetMainVM(MainVM mainVM)
        {
            this.mainVM = mainVM;
        }

        private void EventPassword(object sender, RoutedEventArgs e)
        {
            Signal(nameof(Password));
            Validate(nameof(Password), passwrdBox.Password);
        }

        private string loginBox;

        [Required(ErrorMessage = "Требуется указать логин")]
        public string Login // введенный пользователем логин
        {
            get { return loginBox; }
            set
            {
                loginBox = value;

                Validate(nameof(Login), value);
            }
        }

        private PasswordBox passwrdBox;

        [Required(ErrorMessage = "Требуется указать пароль")]
        public string Password // введенный пользователем пароль
        {
            get { return passwrdBox.Password; }
            set
            {
                passwrdBox.Password = value;

                Validate(nameof(Password), value);
            }
        }
    }
}
