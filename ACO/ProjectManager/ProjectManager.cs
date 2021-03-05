﻿using ACO.ExcelHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ACO.ProjectManager
{
    class ProjectManager
    {

        public Project ActiveProject
        {
            get
            {
                if (_ActiveProject is null)
                {
                    foreach (Project project in Projects)
                    {
                        if (project.Active)
                        {
                            _ActiveProject = project;
                            break;
                        }
                    }
                    if (_ActiveProject is null && Projects.Count > 0)
                        _ActiveProject = Projects[0];
                }
                return _ActiveProject;
            }
            set
            {
                _ActiveProject = value;
            }
        }
        private Project _ActiveProject;
        public List<Project> Projects
        {
            get
            {
                if (_Projects is null)
                {
                    _Projects = new List<Project>();
                    string folder = GetFolderProjects();
                    string[] files = Directory.GetFiles(folder);
                    foreach (string file in files)
                    {
                        if (new FileInfo(file).Extension == ".xml")
                        {
                            Project project = LoadProject(file);
                            _Projects.Add(project);
                        }
                    }
                }
                return _Projects;
            }
            set
            {
                _Projects = value;
            }
        }

        private List<Project> _Projects;
        public void CreateProject(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                string filename = GetPathTo(name + ".xml");
                if (!File.Exists(filename))
                {
                    CreateNewProjectXML(name, filename);
                }
            }
        }

        public void CreateNewProjectXML(string projectname, string path)
        {
            Projects?.ForEach(x => x.Active = false);         
            XElement root = new XElement("project");
            XAttribute xaName = new XAttribute("ProjectName", projectname);
            XAttribute xaActive = new XAttribute("Active", true);
            root.Add(xaName);
            root.Add(xaActive);
            XElement xeColumns = new XElement("Columns");
            root.Add(xeColumns);         
            XDocument xdoc = new XDocument(root);
            xdoc.Save(path);
        }
        /// <summary>
        /// Генерирует путь к файлу
        /// </summary>
        /// <param name="file">Имя файла</param>
        /// <returns>Путь к файлу в AppData</returns>
        private static string GetPathTo(string file)
        {
            string path = GetFolderProjects();
            return Path.Combine(path, file);
        }
        private static string GetFolderProjects()
        {
            string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Spectrum",
            "ACO");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }

        private Project LoadProject(string file)
        {
            Project project = new Project();
            XDocument xdoc = XDocument.Load(file);
            XElement root = xdoc.Root;
            project.FileName = file;
            XAttribute xeName = root.Attribute("Name");
            project.Name = root.Attribute("ProjectName").Value?.ToString() ?? "";
            project.Active = bool.Parse(root.Attribute("Active").Value?.ToString() ?? "false");
            project.Columns = LoadColumnsFromXElement(root.Element("Columns"));
            return project;
        }

        private List<ColumnMapping> LoadColumnsFromXElement(XElement xElement)
        {
            List<ColumnMapping> columns = new List<ColumnMapping>();
            if (xElement != null)
            {
                foreach (XElement xcol in xElement.Elements())
                {
                    columns.Add(ColumnMapping.GetCellFromXElement(xcol));
                }
            }
            return columns;
        }
       
    }
}
