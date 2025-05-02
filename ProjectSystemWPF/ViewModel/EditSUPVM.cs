using ChatServerDTO.DB;
using ChatServerDTO.DTO;
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
    public class EditSUPVM: BaseVM
    {
        private ObservableCollection<UserDTO> executors =new ObservableCollection<UserDTO>();
        private UserDTO executor;
        private ProjectDTO project;

        public ObservableCollection<UserDTO> Executors
        {
            get => executors;
            set { executors = value;
                Signal();
            }
        }

        public UserDTO Executor
        {
            get => executor;
            set { executor = value;
                Signal();
            }
        }

        public ProjectDTO Project
        {
            get => project;
            set { project = value;
                Signal();
            }
        }
        public event EventHandler Loaded;
        public VmCommand Save {  get; set; }
        public EditSUPVM()
        {
            GetUsers();

            Save = new VmCommand(async () =>
            {
                Project.IdCreator = Executor.Id;
                Project.Creator = Executor;
                string arg = JsonSerializer.Serialize(Project, REST.Instance.options);
                var responce = await REST.Instance.client.PutAsync($"Projects/{Project.Id}",
                    new StringContent(arg, Encoding.UTF8, "application/json"));
                try
                {
                    responce.EnsureSuccessStatusCode();
                    MessageBox.Show("Проект успешно обновлен!");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка! Обновление проекта приостановлено!");
                    return;
                }
                Loaded?.Invoke(this, null);
                editSUPP.Close();
            });
            
        }
        public async System.Threading.Tasks.Task GetUsers()
        {
            var result = await REST.Instance.client.GetAsync($"Users/GetAllUsers");
            //todo not ok

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                Executors = await result.Content.ReadFromJsonAsync<ObservableCollection<UserDTO>>(REST.Instance.options);
                if (Project != null)
                {
                    Executor = Executors.FirstOrDefault(s => s.Id == Project.IdCreator);
                }
                //Executor = Executors.FirstOrDefault();
            }

        }

        internal void GetProject(ProjectDTO project)
        {
            Project = project;
        }

        EditSUPP editSUPP;
        internal void SetWindow(EditSUPP editSUPP)
        {
            this.editSUPP = editSUPP;
        }
    }
}
