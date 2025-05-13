using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UniversityConsultationsApp
{
    /// <summary>
    /// Логика взаимодействия для EditStudentWindow.xaml
    /// </summary>
    public partial class EditStudentWindow : Window
    {
        private readonly UniversityConsultationsModel _context;
        private readonly int _userId;

        public EditStudentWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _context = new UniversityConsultationsModel();
            LoadStudentProfile();
        }

        private void LoadStudentProfile()
        {
            var student = _context.Student
                .Where(s => s.UserID == _userId)
                .Select(s => new { s.User.LastName, s.User.FirstName, s.User.MiddleName, s.User.Email, s.Major, s.EnrollmentYear })
                .FirstOrDefault();

            if (student != null)
            {
                LastNameTextBox.Text = student.LastName;
                FirstNameTextBox.Text = student.FirstName;
                MiddleNameTextBox.Text = student.MiddleName;
                EmailTextBox.Text = student.Email;
                MajorTextBox.Text = student.Major;
                EnrollmentYearTextBox.Text = student.EnrollmentYear.ToString();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var student = _context.Student.FirstOrDefault(s => s.UserID == _userId);
            if (student != null)
            {
                student.User.LastName = LastNameTextBox.Text;
                student.User.FirstName = FirstNameTextBox.Text;
                student.User.MiddleName = MiddleNameTextBox.Text;
                student.User.Email = EmailTextBox.Text;
                student.Major = MajorTextBox.Text;
                student.EnrollmentYear = int.Parse(EnrollmentYearTextBox.Text);

                if (!string.IsNullOrEmpty(PasswordBox.Password))
                {
                    student.User.Password = PasswordBox.Password;
                }

                _context.SaveChanges();
                MessageBox.Show("Profile updated successfully!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Student not found.");
            }
        }
    }
}
