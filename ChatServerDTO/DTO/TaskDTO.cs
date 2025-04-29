using ChatServerDTO.DB;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = ChatServerDTO.DB.Task;

namespace ChatServerDTO.DTO
{
    public class TaskDTO
    {
        public int Id { get; set; }

        public string? Title { get; set; } 

        public string? Description { get; set; }

        public int IdProject { get; set; }

        public int IdStatus { get; set; }

        public int IdCreator { get; set; }

        public UserDTO? Creator { get; set; }

        //public ProjectDTO? Project { get; set; }

        public string? StatusTitle { get; set; }

        public List<TaskForUser> TaskForUsers { get; set; } = new List<TaskForUser>();

        public static explicit operator TaskDTO (ChatServerDTO.DB.Task task)
        {
            var result = new TaskDTO
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IdProject = task.IdProject,
                IdStatus = task.IdStatus,
                IdCreator = task.IdCreator,


            };
            if (task.IdCreatorNavigation != null)
                result.Creator = (UserDTO)task.IdCreatorNavigation;

            //if (task.IdProjectNavigation != null)
            //    result.Project = (ProjectDTO)task.IdProjectNavigation;

            if (task.IdStatusNavigation != null)
                result.StatusTitle = task.IdStatusNavigation.Title;

            if (task.TaskForUsers != null)
                result.TaskForUsers = task.TaskForUsers.Select(s => (TaskForUser)s).ToList();

            return result;
        }

        public static explicit operator Task(TaskDTO taskDTO)
        {
            var result = new Task
            {
                Id = taskDTO.Id,
                Title = taskDTO.Title,
                Description = taskDTO.Description,
                IdProject = taskDTO.IdProject,
                IdStatus = taskDTO.IdStatus,
                IdCreator = taskDTO.IdCreator,
            };

            return result;
        }
    }
}
