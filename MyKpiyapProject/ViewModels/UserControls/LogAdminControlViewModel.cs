using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyKpiyapProject.ViewModels.UserControls
{
    class LogAdminControlViewModel : INotifyPropertyChanged
    {
        private tbEmployee myUser;
        private ObservableCollection<tbAdminLog> adminLog;
        private ObservableCollection<tbAdminLog> _allAdminLog;
        private AdminLogService _adminLogService;
        private LoggingService _loggingService;
        private string _searchText;


        public ObservableCollection<tbAdminLog> AdminLog
        {
            get => adminLog;
            set
            {
                adminLog = value;
                OnPropertyChanged(nameof(AdminLog));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public ICommand LoadDataCommand { get; }

        public LogAdminControlViewModel() { }

        public LogAdminControlViewModel(tbEmployee tbEmployee)
        {
            myUser = tbEmployee;
            _adminLogService = new AdminLogService();
            _loggingService = new LoggingService();
            LoadDataCommand = new RelayCommand(_ => LoadAdminLog());
            LoadAdminLog();
        }

        private async System.Threading.Tasks.Task LoadAdminLog()
        {
            try
            {
                var logi = await System.Threading.Tasks.Task.Run(() => _adminLogService.GetAllAdminLogs());
                _allAdminLog = new ObservableCollection<tbAdminLog>(logi);
                ApplyFilters();
                await _loggingService.LogAction(myUser.EmployeeID, "Загрузка логов", "Операции с данными", "Успех");
            }
            catch (Exception ex)
            {
                await _loggingService.LogAction(myUser.EmployeeID, "Загрузка логов", "Операции с данными", "Ошибка");
                Console.WriteLine($"Ошибка при загрузке логов: {ex.Message}");
            }
        }

        private void ApplyFilters()
        {
            if (_allAdminLog == null) return;

            IEnumerable<tbAdminLog> filtered = _allAdminLog;

            if (!string.IsNullOrEmpty(SearchText))
            {
                var searchTextLower = SearchText.ToLower();
                filtered = filtered.Where(e =>
                    e.Employee.FullName.ToLower().Contains(searchTextLower) ||
                    e.Status.ToLower().Contains(searchTextLower) ||
                    e.Action.ToLower().Contains(searchTextLower) ||
                    e.EventType.ToLower().Contains(searchTextLower));
            }

            AdminLog = new ObservableCollection<tbAdminLog>(filtered);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
