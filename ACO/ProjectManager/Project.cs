﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACO.ProjectManager
{
    class Project
    {

        public bool Active { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public List<ColumnMapping> Columns { get; set; }

        //public ColumnsMapping MyProperty { get; set; }
        //public class SettingsProject
        public Project()
        {

        }
    }
}
