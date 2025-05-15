using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MyKpiyapProject.NewModels
{
    public class tbReport : INotifyPropertyChanged
    {
        private int reportID;
        private DateTime creationDate;
        private string title;
        private string description;
        private int employeeID;

        [Key]
        public int ReportID
        {
            get { return reportID; }
            set { reportID = value; OnPropertyChanged(); }
        }

        public DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; OnPropertyChanged(); }
        }

        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get { return description; }
            set { description = value; OnPropertyChanged(); }
        }

        [ForeignKey("Employee")]
        public int EmployeeID
        {
            get { return employeeID; }
            set { employeeID = value; OnPropertyChanged(); }
        }

        public virtual tbEmployee Employee { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
