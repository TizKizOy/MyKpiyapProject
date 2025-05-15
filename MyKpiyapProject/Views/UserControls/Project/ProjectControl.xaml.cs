using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MyKpiyapProject.NewModels;
using MyKpiyapProject.Services;
using MyKpiyapProject.ViewModels.UserControls.Project;

namespace MyKpiyapProject.UserControls
{
    /// <summary>
    /// Логика взаимодействия для ProjectControl.xaml
    /// </summary>
    public partial class ProjectControl : UserControl
    {
        private EmployeeService employeerService = new EmployeeService();

        public ProjectControl(tbEmployee tbEmployee)
        {
            InitializeComponent();

            DataContext = new ProjectControlViewModel(tbEmployee);
            Loaded += OnLoaded;

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProjectControlViewModel vm)
            {
                vm.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(ProjectControlViewModel.CurrentControl) && vm.CurrentControl != null)
                    {
                        ShowInAnimation(vm.CurrentControl);
                    }
                };
            }
        }

        private void projectDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                if (e.Row.DataContext is tbProject project)
                {
                    int rowNumber = e.Row.GetIndex() + 1;
                    project.RowNumber = rowNumber;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке строки: {ex.Message}");
            }
        }

        private void ShowInAnimation(UserControl userControl)
        {
            // Подготовка контрола к анимации
            userControl.Margin = new Thickness(500, 0, 0, 0);
            userControl.HorizontalAlignment = HorizontalAlignment.Right;

            // Очистка и добавление нового контрола
            FormContainer.Children.Clear();
            FormContainer.Children.Add(userControl);

            // Запуск анимации
            var storyboard = (Storyboard)FindResource("SlideInFromRightStoryboard");
            storyboard.Begin(userControl);
        }
    }
}