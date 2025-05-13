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
    /// Логика взаимодействия для TeacherWindow.xaml
    /// </summary>
    public partial class TeacherWindow : Window
    {
        private UniversityConsultationsModel _context;
        private int _teacherID;
        private int _userID;

        public TeacherWindow(int teacherID)
        {
            InitializeComponent();
            _context = new UniversityConsultationsModel();
            _teacherID = teacherID;
            _userID = _context.Teacher
                .Where(t => t.TeacherID == _teacherID)
                .Select(t => t.UserID)
                .FirstOrDefault();
            LoadConsultations();
        }

        private void LoadConsultations()
        {
            var consultations = (from c in _context.Consultation
                                 join r in _context.Record on c.ConsultationID equals r.ConsultationID into cr
                                 from r in cr.DefaultIfEmpty()
                                 join s in _context.Student on r.StudentID equals s.StudentID into sr
                                 from s in sr.DefaultIfEmpty()
                                 join u in _context.User on s.UserID equals u.UserID into su
                                 from u in su.DefaultIfEmpty()
                                 where c.TeacherID == _teacherID
                                 select new
                                 {
                                     c.ConsultationID,
                                     c.ConsultationDate,
                                     c.Duration,
                                     c.Topic,
                                     StudentName = u != null ? u.FirstName + " " + u.LastName : "No Student"
                                 }).ToList();

            ConsultationsDataGrid.ItemsSource = consultations;
        }

        private void AddConsultation_Click(object sender, RoutedEventArgs e)
        {
            AddEditConsultationWindow addWindow = new AddEditConsultationWindow(_teacherID);
            addWindow.ShowDialog();
            LoadConsultations();
        }

        private void EditConsultation_Click(object sender, RoutedEventArgs e)
        {
            if (ConsultationsDataGrid.SelectedItem != null)
            {
                var selectedItem = ConsultationsDataGrid.SelectedItem;
                var properties = selectedItem.GetType().GetProperties();
                var consultationIDProperty = properties.FirstOrDefault(p => p.Name == "ConsultationID");
                if (consultationIDProperty != null)
                {
                    var consultationID = consultationIDProperty.GetValue(selectedItem);
                    var consultation = _context.Consultation.Find(consultationID);
                    var record = _context.Record.FirstOrDefault(r => r.ConsultationID == consultation.ConsultationID);
                    AddEditConsultationWindow editWindow = new AddEditConsultationWindow(_teacherID, consultation, record?.StudentID);
                    editWindow.ShowDialog();
                    _context.SaveChanges();

                    LoadConsultations();
                }
            }
        }


        private void DeleteConsultation_Click(object sender, RoutedEventArgs e)
        {
            if (ConsultationsDataGrid.SelectedItem != null)
            {
                var selectedItem = ConsultationsDataGrid.SelectedItem;
                var consultationID = selectedItem.GetType().GetProperties().FirstOrDefault(p => p.Name == "ConsultationID").GetValue(selectedItem);
                var consultation = _context.Consultation.Find(consultationID);

                // Удалить все связанные записи в таблице Record
                var records = _context.Record.Where(r => r.ConsultationID == consultation.ConsultationID).ToList();
                _context.Record.RemoveRange(records);

                // Удалить запись из таблицы Consultation
                _context.Consultation.Remove(consultation);
                _context.SaveChanges();

                LoadConsultations();
            }
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            EditTeacherWindow window = new EditTeacherWindow(_userID);
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
