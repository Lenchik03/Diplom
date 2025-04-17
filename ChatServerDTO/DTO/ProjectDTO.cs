using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Task = ProjectSystemAPI.DB.Task;

namespace ChatServerDTO.DTO
{
    public class ProjectDTO : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public string? Title { get; set; } 

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime CompletionDate { get; set; }

        public int IdCreator { get; set; }

        public List<TaskDTO> Tasks { get; set; } = new List<TaskDTO>();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void Signal([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        public static explicit operator ProjectDTO(Project project)
        {
            var result = new ProjectDTO
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                StartDate = project.StartDate,
                CompletionDate = project.CompletionDate
            };

            if (project.Tasks != null)
                result.Tasks = project.Tasks.Select(s => (TaskDTO)s).ToList();

            return result;

        }

        public static explicit operator Project(ProjectDTO projectDTO)
        {
            var result = new Project
            {
                Id = projectDTO.Id,
                Title = projectDTO.Title,
                Description = projectDTO.Description,
                StartDate = projectDTO.StartDate,
                CompletionDate = projectDTO.CompletionDate
            };

            return result;
        }
    }

    
}
