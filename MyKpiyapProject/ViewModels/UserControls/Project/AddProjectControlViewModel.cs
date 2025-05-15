using Microsoft.Win32;
using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MyKpiyapProject.ViewModels.UserControls.Project
{
    public class AddProjectControlViewModel : INotifyPropertyChanged
    {
        private tbEmployee myUser;
        private readonly Action _refreshCallback;
        private readonly ProjectService _projectService;
        private readonly EmployeeService _userService;

        private AdminLogService _adminLogService;
        private LoggingService _loggingService;

        public ObservableCollection<tbEmployee> Creators { get; } = new ObservableCollection<tbEmployee>();

        private string _projectName;
        private string _projectDescription;
        private tbEmployee _selectedCreator;
        private int _creatorId;
        private string _creatorName;
        private string _status = "Открыт";
        private string _selectedFilePath;
        private DateTime _startDate = DateTime.Today;
        private DateTime _endDate = DateTime.Today;
        private byte[] _doxcData;

        public string ProjectName
        {
            get => _projectName;
            set
            {
                _projectName = value;
                OnPropertyChanged();
            }
        }

        public string ProjectDescription
        {
            get => _projectDescription;
            set
            {
                _projectDescription = value;
                OnPropertyChanged();
            }
        }

        public tbEmployee SelectedCreator
        {
            get => _selectedCreator;
            set
            {
                _selectedCreator = value;
                CreatorId = _selectedCreator?.EmployeeID ?? 0; 
                CreatorName = _selectedCreator?.FullName ?? "Дефолт";
                OnPropertyChanged();
            }
        }

        public string CreatorName
        {
            get => _creatorName;
            set
            {
                _creatorName = value;
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

        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set { _selectedFilePath = value; OnPropertyChanged(); }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
            }
        }

        public int CreatorId
        {
            get => _creatorId;
            set
            {
                _creatorId = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveProjectCommand { get; }
        public ICommand SelectDocxCommand { get; }


        public AddProjectControlViewModel(Action refreshCallback, tbEmployee tbEmployee)
        {
            _projectService = new ProjectService();
            _userService = new EmployeeService();
            _refreshCallback = refreshCallback;

            myUser = tbEmployee;
            _adminLogService = new AdminLogService();
            _loggingService = new LoggingService();

            LoadCreators();

            SaveProjectCommand = new RelayCommand(_ => SaveProject(), CanSaveProject);
            SelectDocxCommand = new RelayCommand(_ => SelectDocx());
        }

        private void LoadCreators()
        {
            var creators = _userService.GetAllEmployees();
            foreach (var creator in creators)
            {
                Creators.Add(creator);
            }
        }

        private bool CanSaveProject(object parameter)
        {
            return !string.IsNullOrWhiteSpace(ProjectName) &&
                   !string.IsNullOrWhiteSpace(ProjectDescription) &&
                   !string.IsNullOrWhiteSpace(Status) &&
                   !string.IsNullOrWhiteSpace(SelectedFilePath) &&
                    SelectedCreator != null;
        }

        private void SelectDocx()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Документы (*.xls, *.xlsx, *.doc, *.docx)|*.xls;*.xlsx;*.doc;*.docx|" +
                        "Excel (*.xls, *.xlsx)|*.xls;*.xlsx|" +
                        "Word (*.doc, *.docx)|*.doc;*.docx",
                FilterIndex = 0,
                Title = "Выберите документ"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    SelectedFilePath = openFileDialog.FileName;
                    _doxcData = File.ReadAllBytes(SelectedFilePath);
                    MessageBox.Show($"Файл {Path.GetFileName(SelectedFilePath)} успешно загружен",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void SaveProject()
        {
            try
            {
                if (EndDate < StartDate)
                {
                    MessageBox.Show("Дата окончания не может быть раньше даты начала", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newProject = new tbProject
                {
                    Title = ProjectName,
                    Description = ProjectDescription,
                    Status = Status,
                    DocxData = _doxcData,
                    CreationDate = StartDate,
                    ClosingDate = EndDate,
                    CreatorID = CreatorId,
                };

                _projectService.AddProject(newProject);
                _refreshCallback?.Invoke();

                await _loggingService.LogAction(myUser.EmployeeID, "Добавление проекта", "Операции с данными", "Успех");

                MessageBox.Show("Проект успешно добавлен", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAction(myUser.EmployeeID, "Добавление проекта", "Операции с данными", "Ошибка");
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
