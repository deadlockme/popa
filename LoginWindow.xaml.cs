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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private UniversityConsultationsModel _context;

        public LoginWindow()
        {
            InitializeComponent();
            _context = new UniversityConsultationsModel();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            var user = _context.User.SingleOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                if (user.Role == "Teacher")
                {
                    TeacherWindow teacherWindow = new TeacherWindow(user.UserID);
                    teacherWindow.Show();
                }
                else if (user.Role == "Student")
                {
                    StudentWindow studentWindow = new StudentWindow(user.UserID);
                    studentWindow.Show();
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid email or password.");
            }
        }
    }
}
