using MyKpiyapProject.NewModels;
using System.Data.Entity;

namespace MyKpiyapProject.Services
{
    public class EmployeeService
    {
        private readonly AppDbContext _context;
        private static List<tbEmployee> _cachedUsers;
        private static DateTime _lastCacheUpdate;
        private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(10);

        public EmployeeService()
        {
            _context = new AppDbContext();
        }

        // Create
        public void AddEmployee(tbEmployee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        // Read
        public tbEmployee GetEmployeeById(int id)
        {
            return _context.Employees.FirstOrDefault(e => e.EmployeeID == id);
        }

        public async Task<tbEmployee> GetUserByLoginAsyncWithCash(string login)
        {
            if (_cachedUsers == null || DateTime.Now - _lastCacheUpdate > CacheLifetime)
            {
                _cachedUsers = await _context.Employees
                    .AsNoTracking()
                    .ToListAsync();
                _lastCacheUpdate = DateTime.Now;
            }

            return _cachedUsers.FirstOrDefault(u => u.Login == login);
        }

        public async Task<tbEmployee> GetUserByLoginAsync(string login)
        {
            return _context.Employees.FirstOrDefault(u => u.Login == login);
        }

        public List<tbEmployee> GetAllEmployees()
        {
            using (var context = new AppDbContext())
            {
                return context.Employees.AsNoTracking().ToList();
            }
        }

        // Update
        public void UpdateEmployee(tbEmployee employee)
        {
            var existingEmployee = _context.Employees.Find(employee.EmployeeID);

            if (existingEmployee != null)
            {
                _context.Entry(existingEmployee).CurrentValues.SetValues(employee);
            }
            else
            {
                _context.Employees.Attach(employee);
                _context.Entry(employee).State = System.Data.Entity.EntityState.Modified;
            }

            _context.SaveChanges();
        }

        // Delete
        public void DeleteEmployee(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.EmployeeID == id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
        }
    }
}
