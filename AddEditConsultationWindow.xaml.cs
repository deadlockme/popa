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
    /// Логика взаимодействия для AddEditConsultationWindow.xaml
    /// </summary>
    public partial class AddEditConsultationWindow : Window
    {
        private UniversityConsultationsModel _context;
        private Consultation _consultation;
        private int _teacherID;
        private int? _selectedStudentID;

        public AddEditConsultationWindow(int teacherID, Consultation consultation = null, int? selectedStudentID = null)
        {
            InitializeComponent();
            _context = new UniversityConsultationsModel();
            _teacherID = teacherID;
            _consultation = consultation;
            _selectedStudentID = selectedStudentID;

            LoadStudents();

            if (_consultation != null)
            {
                TopicTextBox.Text = _consultation.Topic;
                ConsultationDatePicker.SelectedDate = _consultation.ConsultationDate;
                DurationTextBox.Text = _consultation.Duration.ToString();

                if (_selectedStudentID.HasValue)
                {
                    var student = _context.Student.Find(_selectedStudentID.Value);
                    if (student != null)
                    {
                        StudentComboBox.SelectedItem = new { student.StudentID, FullName = student.User.FirstName + " " + student.User.LastName };
                    }
                }
            }
        }

        private void LoadStudents()
        {
            var students = _context.Student
                .Select(s => new
                {
                    s.StudentID,
                    FullName = s.User.FirstName + " " + s.User.LastName
                })
                .ToList();

            StudentComboBox.ItemsSource = students;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_consultation == null)
            {
                _consultation = new Consultation
                {
                    Topic = TopicTextBox.Text,
                    ConsultationDate = ConsultationDatePicker.SelectedDate.Value,
                    Duration = int.Parse(DurationTextBox.Text),
                    TeacherID = _teacherID
                };
                _context.Consultation.Add(_consultation);
                _context.SaveChanges(); // сохранить изменения, чтобы получить ID консультации
                _consultation = _context.Consultation.Local.FirstOrDefault(c => c.Topic == TopicTextBox.Text); // обновить _consultation с полученным ID
            }
            else
            {
                _consultation.Topic = TopicTextBox.Text;
                _consultation.ConsultationDate = ConsultationDatePicker.SelectedDate.Value;
                _consultation.Duration = int.Parse(DurationTextBox.Text);
                _context.SaveChanges();
            }

            if (StudentComboBox.SelectedItem != null)
            {
                var selectedStudent = (dynamic)StudentComboBox.SelectedItem;
                var records = _context.Record.Where(r => r.ConsultationID == _consultation.ConsultationID).ToList();

                if (records.Count == 0)
                {
                    var record = new Record
                    {
                        ConsultationID = _consultation.ConsultationID,
                        StudentID = selectedStudent.StudentID,
                        RecordDate = DateTime.Now,
                        Status = "Confirmed"
                    };
                    _context.Record.Add(record);
                }
                else
                {
                    var record = records.FirstOrDefault();
                    record.StudentID = selectedStudent.StudentID;
                }

                _context.SaveChanges();
            }

            LoadStudents(); // вызвать LoadStudents() после сохранения изменений

            this.Close();
        }
    }
}
