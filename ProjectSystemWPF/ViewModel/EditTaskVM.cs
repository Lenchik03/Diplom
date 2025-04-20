using ChatServerDTO.DTO;
using MaterialDesignColors.Recommended;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectSystemWPF.ViewModel
{
    public class EditTaskVM: BaseVM
    {
        public event EventHandler Loaded;
        private string search;
        private TaskDTO task;
        private ObservableCollection<UserDTO> selectedExecutors;
        private UserDTO addExecutor;
        private UserDTO removeExecutor;

        public TaskDTO Task
        {
            get => task;
            set { task = value;
                Signal();
            }     
        }
        public ObservableCollection<ProjectDTO> Projects { get; set; }

        public ObservableCollection<UserDTO> Executors { get; set; }
        public UserDTO AddExecutor
        {
            get => addExecutor;
            set { addExecutor = value;
                Signal();
            }
        }

        public ObservableCollection<UserDTO> SelectedExecutors
        {
            get => selectedExecutors;
            set { selectedExecutors = value;
                Signal();
            }
        }
        public UserDTO RemoveExecutor
        {
            get => removeExecutor;
            set { removeExecutor = value;
                Signal();
            }
        }

        public VmCommand AddEx { get; set; }
        public VmCommand RemoveEx { get; set; }
        public VmCommand Save { get; set; }
        public ProjectDTO SelectedProject { get; set; }

        public string Search 
        { 
            get => search;
            set { search = value;
                
            }
        }

        public EditTaskVM()
        {
            GetProjects();
            GetExecutors();
            AddEx = new VmCommand(async () =>
            {
                if (AddExecutor != null)
                {
                    if (SelectedExecutors.Contains(AddExecutor))
                        MessageBox.Show("Этот сотрудник уже добавлен в список исполнителей задачи!");
                    else
                        SelectedExecutors.Add(AddExecutor);
                }
                else
                {
                    MessageBox.Show("Выберите исполнителя для задачи!");
                }
            });

            RemoveEx = new VmCommand(async () =>
            {
                if (RemoveExecutor != null)
                    SelectedExecutors.Remove(RemoveExecutor);
                else
                    MessageBox.Show("Выберите исполнителя!");
            });

            Save = new VmCommand(async () =>
            {
                Task.Creator = ActiveUser.GetInstance().User;
                Task.IdCreator = ActiveUser.GetInstance().User.Id;
                Task.Project = SelectedProject;
                Task.IdProject = SelectedProject.Id;
                Task.IdStatus = 1;
                Task.StatusTitle = "Выдана";
                if (Task.Id == 0)
                {        
                    string arg = JsonSerializer.Serialize(Task, REST.Instance.options);
                    var responce = await REST.Instance.client.PostAsync($"Tasks",
                        new StringContent(arg, Encoding.UTF8, "application/json"));
                    try
                    {
                        responce.EnsureSuccessStatusCode();
                        Task = await responce.Content.ReadFromJsonAsync<TaskDTO>(REST.Instance.options);
                        MessageBox.Show("Задача былa добавлена!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка. Заполните все данные!");
                        return;
                    }
                    string arg1 = JsonSerializer.Serialize(SelectedExecutors, REST.Instance.options);
                    var responce1 = await REST.Instance.client.PostAsync($"Tasks/AddNewExecutors/{Task.Id}",
                        new StringContent(arg1, Encoding.UTF8, "application/json"));
                    try
                    {
                        responce.EnsureSuccessStatusCode();
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка при передаче задачи исполнителям!");
                        return;
                    }
                }
                else
                {

                    string arg = JsonSerializer.Serialize(Task, REST.Instance.options);
                    var responce = await REST.Instance.client.PutAsync($"Tasks",
                        new StringContent(arg, Encoding.UTF8, "application/json"));
                    try
                    {
                        responce.EnsureSuccessStatusCode();
                        MessageBox.Show("Задача успешно обновлена!");

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка! Обновление задачи приостановлено!");
                        return;
                    }
                    string arg1 = JsonSerializer.Serialize(SelectedExecutors, REST.Instance.options);
                    var responce1 = await REST.Instance.client.PostAsync($"Tasks/AddNewExecutors/{Task.Id}",
                        new StringContent(arg1, Encoding.UTF8, "application/json"));
                    try
                    {
                        responce.EnsureSuccessStatusCode();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка при передаче задачи исполнителям!");
                        return;
                    }
                }
            });
        }

        private void GetTask(TaskDTO taskDTO)
        {
            Task = taskDTO;
        }

        public async void GetProjects()
        {
            var result = await REST.Instance.client.GetAsync($"Projects/MyProject/{ActiveUser.GetInstance().User.Id}");

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                Projects = await result.Content.ReadFromJsonAsync<ObservableCollection<ProjectDTO>>(REST.Instance.options);
            }
            Loaded?.Invoke(this, null);
        }

        public async void GetExecutors()
        {
            var users = new ObservableCollection<UserDTO>();
            var deps = new ObservableCollection<DepartmentDTO>();
            var result = await REST.Instance.client.GetAsync($"Users");

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                users = await result.Content.ReadFromJsonAsync<ObservableCollection<UserDTO>>(REST.Instance.options);
            }
            var result1 = await REST.Instance.client.GetAsync($"Users");

            if (result1.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                deps = await result1.Content.ReadFromJsonAsync<ObservableCollection<DepartmentDTO>>(REST.Instance.options);
            }
            var dep = deps.FirstOrDefault(s => s.Id == ActiveUser.GetInstance().User.IdDepartment);
            if(dep.IdMainDep == null)
            {
                foreach(var d in dep.ChildDepartments)
                {
                    foreach(var user in d.Users)
                    {
                        Executors.Add(user);
                    }
                    
                }
            }
            else
            {
               foreach(var user in dep.Users)
                {
                    Executors.Add(user);
                }
            }
        }
    }
}
