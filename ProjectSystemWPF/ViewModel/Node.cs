using ProjectSystemAPI.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSystemWPF.ViewModel
{
    public class Node
    {
        public string Name { get; set; }
        public ObservableCollection<DepartmentDTO> Nodes { get; set; }
    }
}
