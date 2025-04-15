using ProjectSystemAPI.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Task = ProjectSystemAPI.DB.Task;

namespace ProjectSystemWPF.ViewModel
{
    public class ProjectVM
    {
        public VmCommand DeleteProject {  get; set; }
        public VmCommand NewProject { get; set; }
        public ObservableCollection<Project> Projects { get; set; }
        public Project Project { get; set; }
        public VmCommand DeleteTask { get; set; }
        public VmCommand NewTask { get; set; }
        public ObservableCollection<Task> Tasks { get; set; }
        public Task SelectedTask { get; set; }
        public string Executor { get; set; }
        public string Status { get; set; }

        public ProjectVM()
        {
            NewProject = new VmCommand(async () =>
            {

            });
        }
    }
}
