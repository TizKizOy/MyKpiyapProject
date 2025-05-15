using MyKpiyapProject.NewModels;
using System.Collections.Generic;
using System.Linq;

namespace MyKpiyapProject.Services
{
    public class ProjectService
    {
        private readonly AppDbContext _context;

        public ProjectService()
        {
            _context = new AppDbContext();
        }

        // Create
        public void AddProject(tbProject project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
        }

        // Read
        public tbProject GetProjectById(int id)
        {
            return _context.Projects.FirstOrDefault(p => p.ProjectID == id);
        }

        public List<tbProject> GetAllProjects()
        {
            return _context.Projects.ToList();
        }

        // Update
        public void UpdateProject(tbProject project)
        {
            _context.Entry(project).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
        }

        // Delete
        public void DeleteProject(int id)
        {
            var project = _context.Projects.FirstOrDefault(p => p.ProjectID == id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                _context.SaveChanges();
            }
        }
    }
}
