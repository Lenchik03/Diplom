using ChatServerDTO.DTO;
using ProjectSystemAPI.DB;
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
    public class EditProjectVM: BaseVM
    {
        public event EventHandler Loaded;
        private ProjectDTO project;

        public ProjectDTO Project
        {
            get => project;
            set { project = value;
                Signal();
            }
        }
        public VmCommand Save {  get; set; }

        public EditProjectVM()
        {
            Save = new VmCommand(async () =>
            {
                project.IdCreator = ActiveUser.GetInstance().User.Id;
                if (Project.Id == 0)
                {
                   
                    string arg = JsonSerializer.Serialize(Project, REST.Instance.options);
                    var responce = await REST.Instance.client.PostAsync($"Projects",
                        new StringContent(arg, Encoding.UTF8, "application/json"));
                    try
                    {
                        responce.EnsureSuccessStatusCode();
                        MessageBox.Show("Проект успешно создан!");

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка!");
                        return;
                    }
                }
                else
                {
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
                }
                Loaded?.Invoke(this, null);
            });
        }
        internal void GetProject(ProjectDTO project)
        {
            Project = project;
        }
    }
}
