using ChatServerDTO.DB;
using ChatServerDTO.DB;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
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

        public string? UStatus { get; set; }


        public List<TaskUserStatus> TaskForUsers { get; set; } = new();

        public static explicit operator TaskDTO(ChatServerDTO.DB.Task task)
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
            if(task.TaskForUsers.Count > 0)
            {
                if (task.TaskForUsers.Where(s => s.IdStatus == 2).Count() == task.TaskForUsers.Count)
                {
                    result.IdStatus = 2;
                    result.StatusTitle = "В процессе";
                }
                else if (task.TaskForUsers.Where(s => s.IdStatus == 3).Count() == task.TaskForUsers.Count)
                {
                    result.IdStatus = 3;
                    result.StatusTitle = "Выполнена";
                }
                else if (task.TaskForUsers.Where(s => s.IdStatus == 4).Count() == task.TaskForUsers.Count)
                {
                    result.IdStatus = 4;
                    result.StatusTitle = "Удалена";
                }
                else
                {
                    result.IdStatus = 1;
                    result.StatusTitle = "Выдана";
                }
            }
            
                
            //if (task.IdStatusNavigation != null)
            //    result.StatusTitle = task.IdStatusNavigation.Title;

            if (task.TaskForUsers.Count > 0)
            {
                result.TaskForUsers = task.TaskForUsers.Select(s => new TaskUserStatus
                {
                    FIO = $"{s.IdUserNavigation.LastName} {s.IdUserNavigation.FirstName?.FirstOrDefault()}. {s.IdUserNavigation.Patronymic?.FirstOrDefault()}.",
                    UserId = s.IdUser,
                    StatusId = s.IdStatus,
                    StatusTitle = s.IdStatusNavigation.Title
                }).ToList();
               
            }
            

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

    public class TaskUserStatus
    {
        public int UserId { get; set; }
        public string FIO { get; set; }
        public int StatusId { get; set; }
        public string StatusTitle { get; set; }
    }
}
