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
    /// Логика взаимодействия для EditTeacherWindow.xaml
    /// </summary>
    public partial class EditTeacherWindow : Window
    {
        private readonly UniversityConsultationsModel _context;
        private readonly int _userId;

        public EditTeacherWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _context = new UniversityConsultationsModel();
            LoadTeacherProfile();
        }

        private void LoadTeacherProfile()
        {
            var teacher = _context.Teacher
                .Where(t => t.UserID == _userId)
                .Select(t => new { t.User.LastName, t.User.FirstName, t.User.MiddleName, t.User.Email, t.Department, t.Subject })
                .FirstOrDefault();

            if (teacher != null)
            {
                LastNameTextBox.Text = teacher.LastName;
                FirstNameTextBox.Text = teacher.FirstName;
                MiddleNameTextBox.Text = teacher.MiddleName;
                EmailTextBox.Text = teacher.Email;
                DepartmentTextBox.Text = teacher.Department;
                SubjectTextBox.Text = teacher.Subject;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var teacher = _context.Teacher.FirstOrDefault(t => t.UserID == _userId);
            if (teacher != null)
            {
                teacher.User.LastName = LastNameTextBox.Text;
                teacher.User.FirstName = FirstNameTextBox.Text;
                teacher.User.MiddleName = MiddleNameTextBox.Text;
                teacher.User.Email = EmailTextBox.Text;
                teacher.Department = DepartmentTextBox.Text;
                teacher.Subject = SubjectTextBox.Text;

                if (!string.IsNullOrEmpty(PasswordBox.Password))
                {
                    teacher.User.Password = PasswordBox.Password; // Сохраните новый пароль, если он был введен
                }

                _context.SaveChanges();
                MessageBox.Show("Profile updated successfully!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Teacher not found.");
            }
        }
    }
}
