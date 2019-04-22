using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using RoslynMacros.Common.Interfaces;
using Project = Microsoft.CodeAnalysis.Project;


namespace RoslynMacros.MsBuild
{
    public class MsBuildProject : IProject
    {
        #region Private
        private static readonly Lazy<MSBuildWorkspace> _workspace = new Lazy<MSBuildWorkspace>(MSBuildWorkspace.Create);
        private Microsoft.Build.Evaluation.Project _evaluationCurrentProject;
        private readonly List<(string, string)> _postnesting = new List<(string, string)>();
        private readonly List<(string, string)> _postunnesting = new List<(string, string)>();
        private void PostChange(List<(string, string)> postunnesting, List<(string, string)> postnesting)
        {
            var prj = new XmlDocument();
            prj.PreserveWhitespace = true;
            prj.LoadXml(File.ReadAllText(ProjectFile.FullName));
            XmlNode root = prj.FirstChild;
            XmlNode updatenode = null;
            var firstupdatefile = root.SelectSingleNode("ItemGroup/Compile[@Update]");
            if (firstupdatefile != null)
            {
                updatenode = firstupdatefile.ParentNode;
            }
            else
            {
                updatenode = prj.CreateElement("ItemGroup");
                XmlWhitespace ws = prj.CreateWhitespace("\r\n");
                updatenode.AppendChild(ws.CloneNode(false));
                root.AppendChild(updatenode);
                updatenode.AppendChild(ws.CloneNode(false));
            }
            foreach (var (file, nest) in postunnesting)
            {
                var candidate = updatenode?.SelectSingleNode($"/Compile[@Update='{file}']");
                if (candidate == null) continue;
                if (candidate.FirstChild.Name != "DependentUpon") continue;
                if (candidate.Value != nest) continue;
                updatenode.RemoveChild(candidate);
            }
            foreach (var (file, nest) in postnesting)
            {

                var newnode = prj.CreateElement("Compile");
                var attr = prj.CreateAttribute("Update");
                attr.Value = file;
                newnode.Attributes.Append(attr);
                newnode.InnerXml = $"\r\n\t\t\t<DependentUpon>{nest}</DependentUpon>";
                updatenode?.AppendChild(newnode);
                XmlWhitespace ws = prj.CreateWhitespace("\r\n");
                updatenode?.AppendChild(ws.CloneNode(false));
            }
            prj.Save(ProjectFile.FullName);
        }

        protected MSBuildWorkspace Workspace => _workspace.Value;


        protected Microsoft.Build.Evaluation.Project EvaluationCurrentProject
        {
            get
            {
                if (_evaluationCurrentProject == null) 
                    _evaluationCurrentProject = ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(pr => pr.FullPath == ProjectFile.FullName) ?? new Microsoft.Build.Evaluation.Project(ProjectFile.FullName);
                // Update instance of project
                _evaluationCurrentProject.ReevaluateIfNecessary();
                return _evaluationCurrentProject;
            }
        }
        #endregion


        public IConfiguration Configuration { get; }

        public bool Changed { get; private set; }

        public FileInfo ProjectFile { get; }
        public DirectoryInfo ProjectPath { get; }

        public Project Project { get; }
    

        private Compilation _compilation { get; set; } = null;
        public Compilation Compilation => _compilation ?? (_compilation = Project.GetCompilationAsync().Result);

        public MsBuildProject(IConfiguration configuration)
        {
            Configuration = configuration;
            ProjectFile = configuration.Project;
            ProjectPath = ProjectFile.Directory;
            Project =  _workspace.Value.OpenProjectAsync(ProjectFile.FullName).Result;
            
        }

        public string StripBase(DirectoryInfo path)
        {
            return StripBase(path.FullName);
        }
        public string StripBase(FileInfo file)
        {
            return StripBase(file.FullName);
        }
        private string StripBase(string path)
        {
            if (path.StartsWith(ProjectPath.FullName))
            {
                path = path.Substring(ProjectPath.FullName.Length + 1);
            }
            return path;
        }
        public string RelativePath(FileInfo file)
        {
            return StripBase(file.Directory);
        }

        public virtual void SaveProject()
        {
            if (Changed)
            {
                EvaluationCurrentProject.Save();
                if (_postunnesting.Count != 0 || _postnesting.Count != 0)
                    PostChange(_postunnesting, _postnesting);

            }

            Changed = false;
            _postnesting.Clear();
            _postunnesting.Clear();
        }

        public bool AddCsFile(FileInfo fifile, FileInfo finest)
        {
            var changed = false;
            var file = StripBase(fifile);
            var nest = StripBase(finest);
            var neststrip = Path.GetFileName(nest);
            if (file.EndsWith(".log")) return false;
            var fileitem = EvaluationCurrentProject.Items.FirstOrDefault(i => i.EvaluatedInclude == file);
            if (fileitem == null)
            {
                changed = true;
                EvaluationCurrentProject.AddItem("Compile", file, null);
                if (!string.IsNullOrEmpty(nest))
                {
                    EvaluationCurrentProject.ReevaluateIfNecessary();
                    fileitem = EvaluationCurrentProject.Items.FirstOrDefault(i => i.EvaluatedInclude == file);
                    if (fileitem?.IsImported ?? false)
                    {
                        _postnesting.Add((file, nest));
                    }
                    else
                    {
                        fileitem?.SetMetadataValue("DependentUpon", neststrip);
                    }
                }
            }
            else
            {
                var haynest = fileitem.GetMetadataValue("DependentUpon");
                var isold = !fileitem.IsImported;
                var cmpnest = (isold) ? neststrip : nest;
                if ((haynest != cmpnest))
                {
                    changed = true;
                    if (isold)
                    {
                        if (!string.IsNullOrEmpty(haynest)) fileitem.RemoveMetadata(haynest);
                        if (!string.IsNullOrEmpty(nest)) fileitem.SetMetadataValue("DependentUpon", neststrip);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(haynest)) _postunnesting.Add((file, haynest));
                        if (!string.IsNullOrEmpty(nest)) _postnesting.Add((file, nest));
                    }
                }
            }

            Changed = Changed || changed;
            return changed;

        }

        public bool Verbose => Configuration.Verbose;
        public Project OpenProject() => Workspace.OpenProjectAsync(ProjectFile.FullName).Result;

        public void WriteResults(IVariables variables)
        {
            var fs = variables.@OUTPUT;
            var nestfs = variables.Filename;
            var fullfs =new FileInfo(Path.Combine(nestfs.DirectoryName, fs));
            var fullfslog = new FileInfo(Path.Combine(nestfs.DirectoryName, fs+".log"));
            File.WriteAllText(fullfs.FullName,variables.Output.ToString());
            var haylog = false;
            if (Verbose)
            {
                var log = variables.Log.ToString().Trim();
                if (!string.IsNullOrEmpty(log))
                {
                    File.WriteAllText(fullfslog.FullName,log);
                    haylog = true;
                }
            }

            AddCsFile(fullfs, nestfs);
            if (haylog) AddCsFile(fullfslog, nestfs);
        }
    }
}