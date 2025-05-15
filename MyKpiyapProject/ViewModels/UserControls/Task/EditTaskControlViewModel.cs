using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MyKpiyapProject.ViewModels.UserControls.Task
{
    public class EditTaskControlViewModel : INotifyPropertyChanged
    {
        private readonly tbEmployee _user;
        private readonly tbTask _task;
        private readonly Action _refreshCallback;
        private readonly TaskService _taskService;
        private readonly EmployeeService _userService;
        private readonly ProjectService _projectService;
        private readonly EmployeeService _employeerService;

        private AdminLogService _adminLogService;
        private LoggingService _loggingService;

        public ObservableCollection<tbEmployee> Executors { get; } = new ObservableCollection<tbEmployee>();
        public ObservableCollection<tbProject> Projects { get; } = new ObservableCollection<tbProject>();

        private string _nameTasks;
        private string _descriptionTasks;
        private tbProject _selectedProject;
        private tbEmployee _selectedExecutor;
        private string _priority;
        private string _status;
        private DateTime _deadLineDate;


        public DateTime DeadLineDate
        {
            get => _deadLineDate;
            set
            {
                _deadLineDate = value;
                OnPropertyChanged();
            }
        }

        public string NameTasks
        {
            get => _nameTasks;
            set
            {
                _nameTasks = value;
                OnPropertyChanged();
            }
        }

        public string DescriptionTasks
        {
            get => _descriptionTasks;
            set
            {
                _descriptionTasks = value;
                OnPropertyChanged();
            }
        }

        public tbProject SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                OnPropertyChanged();
            }
        }

        public tbEmployee SelectedExecutor
        {
            get => _selectedExecutor;
            set
            {
                _selectedExecutor = value;
                OnPropertyChanged();
            }
        }

        public string Priority
        {
            get => _priority;
            set
            {
                _priority = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveTaskCommand { get; }

        public EditTaskControlViewModel()
        {

        }

        public EditTaskControlViewModel(tbTask task, Action refreshData, tbEmployee tbEmployee)
        {
            _user = tbEmployee;
            _task = task;
            _refreshCallback = refreshData;
            _taskService = new TaskService();
            _userService = new EmployeeService();
            _projectService = new ProjectService();
            _employeerService = new EmployeeService();

            _adminLogService = new AdminLogService();
            _loggingService = new LoggingService();

            // Инициализация свойств из задачи
            NameTasks = task.Title;
            DescriptionTasks = task.Description;
            Priority = task.Priority;
            Status = task.Status;
            DeadLineDate = task.DeadLineDate;

            SaveTaskCommand = new RelayCommand(SaveTask);

            LoadExecutors();
            LoadProjects();
            LoadCurrentSelections();
        }

        private void LoadCurrentSelections()
        {
            // Загрузка текущих значений проекта и исполнителя
            if (_task.ProjectID > 0)
            {
                SelectedProject = Projects.FirstOrDefault(p => p.ProjectID == _task.ProjectID);
            }

            if (_task.ExecutorID > 0)
            {
                SelectedExecutor = Executors.FirstOrDefault(e => e.EmployeeID == _task.ExecutorID);
            }
        }

        private void LoadExecutors()
        {
            var executors = _employeerService.GetAllEmployees();
            Executors.Clear();
            foreach (var creator in executors)
            {
                Executors.Add(creator);
            }
        }

        private void LoadProjects()
        {
            var projects = _projectService.GetAllProjects();
            Projects.Clear();
            foreach (var project in projects)
            {
                Projects.Add(project);
            }
        }

        private async void SaveTask(object parameter)
        {
            // Обновляем свойства задачи
            _task.Title = NameTasks;
            _task.Description = DescriptionTasks;
            _task.ProjectID = SelectedProject?.ProjectID ?? 0;
            _task.ExecutorID = SelectedExecutor?.EmployeeID ?? 0;
            _task.Priority = Priority;
            _task.Status = Status;
            _task.DeadLineDate = DeadLineDate;

            try
            {
                _taskService.UpdateTask(_task);
                _refreshCallback?.Invoke();

                await _loggingService.LogAction(_user.EmployeeID, "Изменение задачи", "Операции с данными", "Успех");

                MessageBox.Show("Задача успешно обновлена!");
            }
            catch (Exception ex)
            {
                await _loggingService.LogAction(_user.EmployeeID, "Изменение задачи", "Операции с данными", "Ошибка");
                MessageBox.Show($"Ошибка при обновлении задачи: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}