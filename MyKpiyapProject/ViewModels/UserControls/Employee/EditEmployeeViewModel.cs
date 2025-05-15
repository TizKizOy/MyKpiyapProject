using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MyKpiyapProject.ViewModels.UserControls.Employee
{
    internal class EditEmployeeViewModel : INotifyPropertyChanged
    {
        private tbEmployee myUser;
        private readonly EmployeeService _employeerService = new EmployeeService();
        private readonly Action _refreshCallback;
        private readonly tbEmployee _originalEmployee;
        public event Action RequestClose;
        private tbEmployee updatedEmployee;

        private AdminLogService _adminLogService;
        private LoggingService _loggingService;

        public int Id { get; }
        public string _name;
        public string _email;
        public string _phone;
        public string _gender;
        public string _position;
        public string _experience;
        public string _password;
        public string SelectedFilePath;
        public byte[] Photo;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }
        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
        }
        public string Gender
        {
            get => _gender;
            set { _gender = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsMale)); OnPropertyChanged(nameof(IsFemale)); }
        }
        public string Position
        {
            get => _position;
            set { _position = value; OnPropertyChanged(); }
        }
        public string Experience
        {
            get => _experience;
            set { _experience = value; OnPropertyChanged(); }
        }
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public bool IsMale
        {
            get => Gender == "Мужской";
            set => Gender = value ? "Мужской" : "Женский";
        }

        public bool IsFemale
        {
            get => Gender == "Женский";
            set => Gender = value ? "Женский" : "Мужской";
        }

        public ICommand SaveCommand { get; }
        public ICommand SelectImageCommand { get; }

        public EditEmployeeViewModel(tbEmployee employeer, Action action, tbEmployee tbEmployee)
        {
            myUser = tbEmployee;
            _originalEmployee = employeer;
            _refreshCallback = action;

            _adminLogService = new AdminLogService();
            _loggingService = new LoggingService();

            Id = employeer.EmployeeID;
            Name = employeer.FullName;
            Email = employeer.Email;
            Phone = employeer.Phone;
            Gender = employeer.Gender;
            Position = employeer.PositionAndRole;
            Experience = employeer.Experience;
            Photo = employeer.Photo;

            SaveCommand = new RelayCommand(_ => SaveEmployee());
            SelectImageCommand = new RelayCommand(_ => SelectImage());
        }

        private void SelectImage()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFilePath = openFileDialog.FileName;
                Photo = System.IO.File.ReadAllBytes(SelectedFilePath);
                OnPropertyChanged(nameof(SelectedFilePath));
            }
        }

        private bool ValidateData(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Name))
            {
                errorMessage = "Имя и фамилия не могут быть пустыми.";
                return false;
            }

            string[] nameParts = Name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length < 2)
            {
                errorMessage = "Имя и фамилия должны содержать два слова.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@") || !Email.Contains("."))
            {
                errorMessage = "Некорректный адрес электронной почты.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Phone))
            {
                errorMessage = "Номер телефона не может быть пустым.";
                return false;
            }

            if (!Phone.StartsWith("+"))
            {
                errorMessage = "Номер телефона должен начинаться со знака '+'.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Gender))
            {
                errorMessage = "Выберите нужный пол";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Position))
            {
                errorMessage = "Должность не может быть пустой.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Experience))
            {
                errorMessage = "Опыт не может быть пустым.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(Password))
            {
                if (Password.Length < 5)
                {
                    errorMessage = "Пароль не может быть меньше пяти символов.";
                    return false;
                }
            }

            return true;
        }


        private async void SaveEmployee()
        {
            try
            {
                if (!ValidateData(out string errorMessage))
                {
                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                updatedEmployee = new tbEmployee
                {
                    EmployeeID = Id,
                    FullName = Name,
                    Email = Email,
                    Phone = Phone,
                    Gender = Gender,
                    PositionAndRole = Position,
                    Experience = Experience,
                    Login = Email.Split("@")[0],
                    Password = string.IsNullOrEmpty(Password) ? _originalEmployee.Password : HashPassword(Password),
                    Photo = Photo ?? _originalEmployee.Photo
                };

                _employeerService.UpdateEmployee(updatedEmployee);
                _refreshCallback?.Invoke();
                RequestClose?.Invoke();

                await _loggingService.LogAction(myUser.EmployeeID, "Изменение рабочих", "Операции с данными", "Успех");

                MessageBox.Show("Данные сотрудника успешно обновлены", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                await _loggingService.LogAction(myUser.EmployeeID, "Изменение рабочих", "Операции с данными", "Ошибка");
                MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
