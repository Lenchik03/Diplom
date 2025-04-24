using ChatServerDTO.DTO;
using ProjectSystemAPI.DB;
using ProjectSystemAPI.DTO;
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
    public class TaskDescriptionVM:BaseVM
    {
        private TaskDTO task;
        private ObservableCollection<UserDTO> executors;

        public TaskDTO Task 
        { get => task;
            set { task = value;
                Signal();
            }
        }
        public UserDTO Executor { get; set; }
        public ObservableCollection<UserDTO> Executors 
        { get => executors;
            set { executors = value;
                Signal();
            }
        }

        public TaskDescriptionVM()
        {
            
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
                Executors = await result.Content.ReadFromJsonAsync<ObservableCollection<UserDTO>>(REST.Instance.options);
            }
        }

        internal void SetTask(TaskDTO task)
        {
            
            Task = task;
            if (Task != null)
                GetExecutorsByTask();
        }
        
    }
}
