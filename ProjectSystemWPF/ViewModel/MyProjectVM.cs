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
    public class MyProjectVM : BaseVM
    {
        public VmCommand DeleteProject { get; set; }
        public VmCommand NewProject { get; set; }
        public ObservableCollection<ProjectDTO> Projects
        {
            get => projects1;
            set
            {
                projects1 = value;
                Signal();
            }
        }
        public ProjectDTO Project
        {
            get => project;
            set
            {
                project = value;
                Signal();
                if (project != null)
                {
                    
                    GetTasks();
                }
            }
        }
        public VmCommand DeleteTask { get; set; }
        public VmCommand NewTask { get; set; }
        public ObservableCollection<TaskDTO> Tasks
        {
            get => tasks1;
            set
            {
                tasks1 = value;
                Signal();
            }
        }
        public TaskDTO SelectedTask
        {
            get => selectedTask;
            set
            {
                selectedTask = value;
                Signal();
            }
        }
        public string Executor { get; set; }
        ObservableCollection<ProjectDTO> projects = new ObservableCollection<ProjectDTO>();
        ObservableCollection<TaskDTO> tasks = new ObservableCollection<TaskDTO>();
        private ObservableCollection<ProjectDTO> projects1;
        private ObservableCollection<TaskDTO> tasks1;
        private ProjectDTO project;
        private TaskDTO selectedTask;


        public MyProjectVM()
        {
            GetProjects();
            //GetTasks();
            NewProject = new VmCommand(async () =>
            {
                EditProjectPage editProjectPage = new EditProjectPage(new ProjectDTO());
                editProjectPage.ShowDialog();
                GetProjects();
            });

            NewTask = new VmCommand(async () =>
            {
                EditTaskPage editTaskPage = new EditTaskPage(new TaskDTO());
                editTaskPage.ShowDialog();

                await System.Threading.Tasks.Task.Delay(200).
                              ContinueWith(async s => await GetTasks());
                Signal(nameof(SelectedTask.TaskForUsers));
                Signal(nameof(SelectedTask));
                Signal(nameof(Project));
            });

            DeleteProject = new VmCommand(async () =>
            {
                //var projectTasks = Tasks.Where(s => s.IdProject == Project.Id);
                //foreach (var task in projectTasks)
                //{
                //    task.IdStatus = 4;
                //    task.StatusTitle = "Удалена";

                //    string arg1 = JsonSerializer.Serialize(task, REST.Instance.options);
                //    var responce1 = await REST.Instance.client.PutAsync($"TaskMs/{task.Id}",
                //        new StringContent(arg1, Encoding.UTF8, "application/json"));
                //    try
                //    {
                //        responce1.EnsureSuccessStatusCode();
                //        // MessageBox.Show("Проект успешно обновлен!");

                //    }
                //    catch (Exception ex)
                //    {
                //        // MessageBox.Show("Ошибка! Обновление проекта приостановлено!");
                //        return;
                //    }
                //}
                if (MessageBox.Show("Удаление проекта", "Вы уверены?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Project.IsDeleted = true;
                    string arg = JsonSerializer.Serialize(Project, REST.Instance.options);
                    var responce = await REST.Instance.client.PutAsync($"Projects/{Project.Id}",
                        new StringContent(arg, Encoding.UTF8, "application/json"));
                    try
                    {
                        responce.EnsureSuccessStatusCode();
                        // MessageBox.Show("Проект успешно обновлен!");
                        GetProjects();
                    }
                    catch (Exception ex)
                    {
                        // MessageBox.Show("Ошибка! Обновление проекта приостановлено!");
                        return;
                    }
                }
            });

            DeleteTask = new VmCommand(async () =>
            {
                if (SelectedTask != null)
                {
                    if (MessageBox.Show("Удаление отдела", "Вы уверены?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        //SelectedTask.IdStatus = 4;
                        //SelectedTask.StatusTitle = "Удалена";
                        SelectedTask.TaskForUsers.ForEach(s => s.StatusId = 4);
                        SelectedTask.TaskForUsers.ForEach(s => s.StatusTitle = "Удалена");
                        string arg = JsonSerializer.Serialize(SelectedTask, REST.Instance.options);
                        var responce = await REST.Instance.client.PutAsync($"TaskMs/{SelectedTask.Id}",
                            new StringContent(arg, Encoding.UTF8, "application/json"));
                        try
                        {
                            responce.EnsureSuccessStatusCode();
                            //MessageBox.Show("Задача успешно обновлена!");
                            await System.Threading.Tasks.Task.Delay(200).
                                  ContinueWith(async s => await GetTasks());
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show("Ошибка! Обновление З приостановлено!");
                            return;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите задачу для удаления!");
                }
            });

        }



        public async System.Threading.Tasks.Task GetProjects()
        {
            var result = await REST.Instance.client.GetAsync($"Projects/GetMyProjects/{ActiveUser.GetInstance().User.Id}");
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

        public async System.Threading.Tasks.Task GetTasks()
        {
            var result1 = await REST.Instance.client.GetAsync($"TaskMs?idProject={Project.Id}");
            //todo not ok

            if (result1.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                var str = await result1.Content.ReadAsStringAsync();
                tasks = await result1.Content.ReadFromJsonAsync<ObservableCollection<TaskDTO>>(REST.Instance.options);
            }

            Tasks = new ObservableCollection<TaskDTO>(tasks);
        }
    

         public UserDTO DefaultCreator = null;
        public async System.Threading.Tasks.Task GetUserByIdAsync(int userId)
        {
            var result1 = await REST.Instance.client.GetAsync($"Users/{userId}");
            //todo not ok

            if (result1.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                var str = await result1.Content.ReadAsStringAsync();
                DefaultCreator = await result1.Content.ReadFromJsonAsync<UserDTO>(REST.Instance.options);
            }

            
        }
    
        internal void Select(ProjectDTO p)
        {
            EditProjectPage editProjectPage = new EditProjectPage(p);
            editProjectPage.ShowDialog();
        }

        internal async void SelectTask(TaskDTO t)
        {
            EditTaskPage editTaskPage = new EditTaskPage(t);
            editTaskPage.ShowDialog();
            await GetTasks();
        }
    }
}
