using ChatServerDTO.DTO;
using ProjectSystemAPI.DB;
using ProjectSystemWPF.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSystemWPF.ViewModel
{
    public class SUProjectPageVM: BaseVM
    {
        private ObservableCollection<ProjectDTO> projects;

        public ObservableCollection<ProjectDTO> Projects
        {
            get => projects;
            set { projects = value;
                Signal();
            }
        }
        public SUProjectPageVM()
        {
            GetProjects();
        }

        public ProjectDTO Project { get; set; }

        public async System.Threading.Tasks.Task GetProjects()
        {
            var result = await REST.Instance.client.GetAsync($"Projects/");
            //todo not ok

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                Projects = await result.Content.ReadFromJsonAsync<ObservableCollection<ProjectDTO>>(REST.Instance.options);
            }           
            
        }

        

        internal void Select(ProjectDTO p)
        {
            EditSUPP editSUPP = new EditSUPP(p);
            editSUPP.ShowDialog();
        }
    }
}
