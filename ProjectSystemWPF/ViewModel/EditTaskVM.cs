using ChatServerDTO.DTO;
using MaterialDesignColors.Recommended;
using Microsoft.AspNetCore.SignalR.Protocol;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using ProjectSystemWPF.View;
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
    public class EditTaskVM : BaseVM
    {
        public event EventHandler Loaded;
        private string search = "";
        private TaskDTO task;
        private ObservableCollection<UserDTO> selectedExecutors = new();
        private UserDTO addExecutor;
        private UserDTO removeExecutor;
        private ObservableCollection<ProjectDTO> projects;
        private ProjectDTO selectedProject;
        private Visibility visibility = Visibility.Visible;

        public TaskDTO Task
        {
            get => task;
            set
            {
                task = value;
                Signal();
            }
        }
        public ObservableCollection<ProjectDTO> Projects
        {
            get => projects;
            set
            {
                projects = value;
                Signal();
            }
        }

        public ObservableCollection<UserDTO> Executors
        {
            get => executors;
            set { executors = value;
                Signal();
            }
        }
        public UserDTO AddExecutor
        {
            get => addExecutor;
            set
            {
                addExecutor = value;
                Signal();
            }
        }

        public ObservableCollection<UserDTO> SelectedExecutors
        {
            get => selectedExecutors;
            set
            {
                selectedExecutors = value;
                Signal();
            }
        }
        public UserDTO RemoveExecutor
        {
            get => removeExecutor;
            set
            {
                removeExecutor = value;
                Signal();
            }
        }

        public Visibility ExecutorsVisible
        {
            get => visibility;
            set { visibility = value;
                Signal();
            }
        }

        public VmCommand AddEx { get; set; }
        public VmCommand RemoveEx { get; set; }
        public VmCommand Save { get; set; }
        public ProjectDTO SelectedProject
        {
            get => selectedProject; set
            {
                selectedProject = value;
                Signal();
            }
        }

        public string Search
        {
            get => search;
            set
            {
                search = value;
                Signal();
                GetExecutors();
            }
        }

        public EditTaskVM()
        {
            
            GetProjects();
           
            GetExecutors();
            if(ActiveUser.GetInstance().User.IdRole == 3)
                ExecutorsVisible = Visibility.Collapsed;
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
                if (ActiveUser.GetInstance().User.IdRole == 3)
                    SelectedExecutors.Add(ActiveUser.GetInstance().User);
                Task.Creator = ActiveUser.GetInstance().User;
                Task.IdCreator = ActiveUser.GetInstance().User.Id;
                //Task.Project = SelectedProject;
                if(SelectedProject != null)
                    Task.IdProject = SelectedProject.Id;
                else
                {
                    MessageBox.Show("Выберите проект!");
                    return;
                }    
                Task.IdStatus = 1;
                Task.StatusTitle = "Выдана";
                if (Task.Id == 0)
                {
                    string arg = JsonSerializer.Serialize(Task, REST.Instance.options);
                    var responce = await REST.Instance.client.PostAsync($"TaskMs",
                        new StringContent(arg, Encoding.UTF8, "application/json"));
                    try
                    {
                        responce.EnsureSuccessStatusCode();
                        Task = await responce.Content.ReadFromJsonAsync<TaskDTO>(REST.Instance.options);
                        MessageBox.Show("Задача былa добавлена!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка");
                        return;
                    }
                    string arg1 = JsonSerializer.Serialize(SelectedExecutors, REST.Instance.options);
                    var responce1 = await REST.Instance.client.PostAsync($"TaskMs/AddNewExecutors/{Task.Id}",
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
                    var responce = await REST.Instance.client.PutAsync($"TaskMs/{Task.Id}",
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
                    var responce1 = await REST.Instance.client.PostAsync($"TaskMs/AddNewExecutors/{Task.Id}",
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
                editTaskPage.Close();
             });
            
        }

        public void SetTask(TaskDTO taskDTO)
        {
            Task = taskDTO;
            if (Task != null)
                GetExecutorsByTask();
        }

        public async System.Threading.Tasks.Task GetExecutorsByTask()
        {
            var result = await REST.Instance.client.GetAsync($"Users/GetExecutorsByTask/{Task.Id}");
            //todo not ok

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                SelectedExecutors = await result.Content.ReadFromJsonAsync<ObservableCollection<UserDTO>>(REST.Instance.options);
            }
        }

        public async void GetProjects()
        {
            var user = ActiveUser.GetInstance().User;
            var result = await REST.Instance.client.GetAsync($"Projects/GetMyProjects/{user.Id}");

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                Projects = await result.Content.ReadFromJsonAsync<ObservableCollection<ProjectDTO>>(REST.Instance.options);
                if (Task != null)
                {
                    SelectedProject = Projects.FirstOrDefault(s => s.Id == Task.IdProject);
                }
            }
            Loaded?.Invoke(this, null);
        }

        public async void GetExecutors()
        {
                string arg1 = JsonSerializer.Serialize(Search, REST.Instance.options);
                var responce1 = await REST.Instance.client.PostAsync($"Users/GetExecutorsForTask?userId={ActiveUser.GetInstance().User.Id}",
                    new StringContent(arg1, Encoding.UTF8, "application/json"));
                try
                {
                    responce1.EnsureSuccessStatusCode();
                    Executors = await responce1.Content.ReadFromJsonAsync<ObservableCollection<UserDTO>>(REST.Instance.options);

                }
                catch (Exception ex)
                {

                    return;
                }
        }

        
        EditTaskPage editTaskPage;
        private ObservableCollection<UserDTO> executors = new();

        internal void SetWindow(EditTaskPage editTaskPage)
        {
            this.editTaskPage = editTaskPage;
        }
    }
}
