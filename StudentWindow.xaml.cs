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
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;

namespace UniversityConsultationsApp
{
    /// <summary>
    /// Логика взаимодействия для StudentWindow.xaml
    /// </summary>
    public partial class StudentWindow : Window
    {
        private UniversityConsultationsModel _context;
        private int _studentID;
        private int _userID;

        public StudentWindow(int userID)
        {
            InitializeComponent();
            _context = new UniversityConsultationsModel();
            _studentID = _context.Student.SingleOrDefault(s => s.UserID == userID)?.StudentID ?? 0;
            _userID = userID;
            LoadConsultations();
        }

        private void LoadConsultations()
        {
            var records = (from r in _context.Record
                           join c in _context.Consultation on r.ConsultationID equals c.ConsultationID
                           join t in _context.Teacher on c.TeacherID equals t.TeacherID
                           join u in _context.User on t.UserID equals u.UserID
                           where r.StudentID == _studentID
                           select new
                           {
                               r.ConsultationID,
                               c.ConsultationDate,
                               c.Duration,
                               c.Topic,
                               TeacherName = u.FirstName + " " + u.LastName,
                               r.Status
                           }).ToList();

            ConsultationsDataGrid.ItemsSource = records;
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            EditStudentWindow window = new EditStudentWindow(_userID);
            window.Show();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow window = new LoginWindow();
            window.Show();
            this.Close();
        }
    }
}
