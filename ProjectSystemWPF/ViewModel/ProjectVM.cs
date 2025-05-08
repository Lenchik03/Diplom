using ChatServerDTO.DB;
using ChatServerDTO.DTO;
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
    public class ProjectVM: BaseVM
    {

        public ObservableCollection<ProjectDTO> Projects 
        { 
            get => projects1;
            set { projects1 = value;
                Signal();
            }
        }
        public ProjectDTO Project 
        { get => project;
            set { project = value;
                Signal();
                if (project != null)
                {
                    GetTasks();
                }
            }
        }
       
        public ObservableCollection<TaskDTO> Tasks 
        { get => tasks1;
            set { tasks1 = value;
                Signal();
            }
        }
        public TaskDTO SelectedTask 
        { get => selectedTask;
            set { selectedTask = value;
                Signal();
                
            }
        }

        public ObservableCollection<Status> AllStatuses
        {
            get => allStatuses;
            set { allStatuses = value;
                Signal();
            }
        }
        public Status SelectedStatus 
        { get => selectedStatus;
            set { selectedStatus = value;
                Signal();
                if(selectedStatus != null)
                {
                    if (selectedStatus.Id == 0)
                        GetTasks();
                    else
                        Filter();
                }
                
            }

        }

        ObservableCollection<ProjectDTO> projects = new ObservableCollection<ProjectDTO>(); 
        ObservableCollection<TaskDTO> tasks = new ObservableCollection<TaskDTO>();
        private ObservableCollection<ProjectDTO> projects1 = new();
        private ObservableCollection<TaskDTO> tasks1;
        private ProjectDTO project;
        private TaskDTO selectedTask;
        private Status selectedStatus;
        private ObservableCollection<Status> allStatuses;

        public CommandParameter<TaskDTO> TakeTask { get; set; }

        public CommandParameter<TaskDTO> CompleteTask { get; set; }

        public ProjectVM()
        {
            GetProjects();
            System.Threading.Tasks.Task.Delay(500);
            //GetUsers();
            //GetTasks();
            GetStatuses();

            TakeTask = new CommandParameter<TaskDTO>(async (TaskDTO task) =>
            {
                task.IdStatus = 2;
                task.StatusTitle = "В процессе";
                task.TaskForUsers.FirstOrDefault(s => s.UserId == ActiveUser.GetInstance().User.Id).StatusId = 2;
                task.TaskForUsers.FirstOrDefault(s => s.UserId == ActiveUser.GetInstance().User.Id).StatusTitle = "В процессе";
                string arg = JsonSerializer.Serialize(task, REST.Instance.options);
                var responce = await REST.Instance.client.PutAsync($"TaskMs/{task.Id}",
                    new StringContent(arg, Encoding.UTF8, "application/json"));
                try
                {
                    responce.EnsureSuccessStatusCode();
                    //MessageBox.Show("Проект успешно обновлен!");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка!");
                    return;
                }
            });

            CompleteTask = new CommandParameter<TaskDTO>(async (TaskDTO task) =>
            {
                task.IdStatus = 3;
                task.StatusTitle = "Выполнена";
                task.TaskForUsers.FirstOrDefault(s => s.UserId == ActiveUser.GetInstance().User.Id).StatusId = 3;
                task.TaskForUsers.FirstOrDefault(s => s.UserId == ActiveUser.GetInstance().User.Id).StatusTitle = "Выполнена";
                string arg = JsonSerializer.Serialize(task, REST.Instance.options);
                var responce = await REST.Instance.client.PutAsync($"TaskMs/{task.Id}",
                    new StringContent(arg, Encoding.UTF8, "application/json"));
                try
                {
                    responce.EnsureSuccessStatusCode();
                    //MessageBox.Show("Проект успешно обновлен!");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка!");
                    return;
                }
            });




        }


        
        public async System.Threading.Tasks.Task GetProjects()
        {
            var result = await REST.Instance.client.GetAsync($"Projects/MyProject/{ActiveUser.GetInstance().User.Id}");
            //todo not ok

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                projects = await result.Content.ReadFromJsonAsync<ObservableCollection<ProjectDTO>>(REST.Instance.options);
            }
            
            Projects = new ObservableCollection<ProjectDTO>(projects);
            Project = Projects.FirstOrDefault();
        }
        public async System.Threading.Tasks.Task GetStatuses()
        {
            var result = await REST.Instance.client.GetAsync($"Statuses");
            //todo not ok

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                AllStatuses = await result.Content.ReadFromJsonAsync<ObservableCollection<Status>>(REST.Instance.options);
            }
            AllStatuses.Insert(0, new Status { Id = 0, Title = "Все статусы" });
            SelectedStatus = AllStatuses[0];

        }

        //public async void GetUsers()
        //{
        //    var result1 = await REST.Instance.client.GetAsync($"Users/GetAllUsers");
        //    //todo not ok

        //    if (result1.StatusCode != System.Net.HttpStatusCode.OK)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        var users = await result1.Content.ReadFromJsonAsync<ObservableCollection<UserDTO>>(REST.Instance.options);
        //        foreach (var item in Projects)
        //        {
        //            item.Creator = users.FirstOrDefault(s => s.Id == item.IdCreator);
        //        }
        //    }
        //}

        public async void GetTasks()
        {
            var result1 = await REST.Instance.client.GetAsync($"TaskMs/My/{ActiveUser.GetInstance().User.Id}");
            //todo not ok

            if (result1.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                tasks = await result1.Content.ReadFromJsonAsync<ObservableCollection<TaskDTO>>(REST.Instance.options);
            }

            Tasks = new ObservableCollection<TaskDTO>(tasks.Where(s => s.IdProject == Project.Id));
            
        }

        public async void Filter()
        {
            var result1 = await REST.Instance.client.GetAsync($"TaskMs/Filter/{SelectedStatus.Id}");
            //todo not ok

            if (result1.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                var alltasks = await result1.Content.ReadFromJsonAsync<ObservableCollection<TaskDTO>>(REST.Instance.options);
                Tasks = new ObservableCollection<TaskDTO>(alltasks.Where(s => s.IdProject == Project.Id));

            }

        }

        internal void Select(TaskDTO p)
        {
            TaskDescriptionWindow taskDescWin = new TaskDescriptionWindow(p);
           taskDescWin.ShowDialog();
        }
    }
}
