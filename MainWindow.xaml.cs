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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.ComponentModel;
using System.Xml.Linq;

namespace wpf_du_7
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Employee employee;
        public MainWindow()
        {
            InitializeComponent();
            cbPozice.Items.Add(Education.žádne);
            cbPozice.Items.Add(Education.základní);
            cbPozice.Items.Add(Education.střední);
            cbPozice.Items.Add(Education.vysoké);
            if (File.Exists($"Employee/employee.txt"))
            {
                string[] data = File.ReadAllLines($"Employee/employee.txt");
                for (int i = 0; i < data.Length; i++)
                {
                    string[] line = data[i].Split(' ');
                    Employee e = new Employee();
                    e.name = line[0];
                    e.surname = line[1];
                    e.birthday = DateTime.Parse(line[2]);
                    e.education = (Education)Enum.Parse(typeof(Education), line[3]);
                    e.position = line[4];
                    e.pay = int.Parse(line[5]);
                    e._ID = Guid.Parse(line[6]);
                    Person.AllPersons.Add(e);
                }
                employee = new Employee();
                employee.birthday = DateTime.Now;
                employee._ID = Guid.NewGuid();
                DataContext = employee;               
            }
        }
        private void btPridat_Click(object sender, RoutedEventArgs e)
        {
            if ((lbSurname.Visibility == Visibility.Visible) || (lbPosition.Visibility == Visibility.Visible) || (lbPay.Visibility == Visibility.Visible))
            {
                MessageBox.Show("Jsou požadovaná povinná data!");
            }
            else
            {
                Employee q = Employee.AllPersons.Find(t => t._ID == employee._ID);
                int qIndex = Employee.AllPersons.IndexOf(q);
                if (q != null)
                {
                    Employee.AllPersons[qIndex] = Employee.PersonCopy(employee);
                }
                else
                {
                    Employee.AllPersons.Add(Employee.PersonCopy(employee));
                    Employee.Clear(employee);
                }
                lv.ItemsSource = null;
                lv.ItemsSource = Employee.AllPersons;
            }                      
        }

        private void btUlozit_Click(object sender, RoutedEventArgs e)
        {
            string[] array = new string[Employee.AllPersons.Count]; 
            Directory.CreateDirectory("Employee");
            for (int i = 0; i < Employee.AllPersons.Count; i++)
            {
                array[i] = Employee.AllPersons[i].ToString();
            }           
            File.WriteAllLines($"Employee/employee.txt", array);
        }
        private void btUpravit_Click(object sender, RoutedEventArgs e)
        {
            employee.name = (((Button)sender).DataContext as Employee).name;
            employee.surname = (((Button)sender).DataContext as Employee).surname;
            employee.birthday = (((Button)sender).DataContext as Employee).birthday;
            employee.education = (((Button)sender).DataContext as Employee).education;
            employee.position = (((Button)sender).DataContext as Employee).position;
            employee.pay = (((Button)sender).DataContext as Employee).pay;
            employee._ID = (((Button)sender).DataContext as Employee)._ID;
        }
        private void btVymazat_Click(object sender, RoutedEventArgs e)
        {
            Employee toDelete = ((Button)sender).DataContext as Employee;
            Employee.AllPersons.Remove(toDelete);
            lv.ItemsSource = null;
            lv.ItemsSource = Employee.AllPersons;
        }

        private void cbPozice_DropDownClosed(object sender, EventArgs e)
        {
            employee.education = (Education)Enum.Parse(typeof(Education), cbPozice.SelectedItem.ToString());
        }

        private void tbPrijmeni_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((employee.surname == "") || (employee.surname == null))
            {
                lbSurname.Visibility = Visibility.Visible;
            }
            else
            {
                lbSurname.Visibility = Visibility.Hidden;
            }
        }

        private void tbpozice_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((employee.position == "") || (employee.position == null))
            {
                lbPosition.Visibility = Visibility.Visible;
            }
            else
            {
                lbPosition.Visibility = Visibility.Hidden;
            }
        }

        private void tbplat_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((tbplat.Text == "0")|| (tbplat.Text == "")||(employee.pay <= 20000)||(employee.pay >= 1000000))
            {
                lbPay.Visibility = Visibility.Visible;
            }
            else
            {
                lbPay.Visibility = Visibility.Hidden;
            }
        }
    }
    public enum Education
    {
        žádne,
        základní,
        střední,
        vysoké,
    }
    class Person : INotifyPropertyChanged
    {
        public static List<Employee> AllPersons { get; set; } = new List<Employee>();
        public event PropertyChangedEventHandler PropertyChanged;
        private string _name;
        private string _surname;
        private DateTime _birthday = new DateTime();
        public Guid _ID { get; set; }
        public string name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("name");
                OnPropertyChanged("Status");
            }
        }
        public string surname
        {
            get => _surname;
            set
            {
                _surname = value;
                OnPropertyChanged("surname");
                OnPropertyChanged("Status");
            }
        }
        public DateTime birthday
        {
            get => _birthday;
            set
            {
                _birthday = value;
                OnPropertyChanged("birthday");
                OnPropertyChanged("Status");
            }
        }
        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
        public static Employee PersonCopy(Employee p)
        {
            Employee q = new Employee()
            {
                name = p.name,
                surname = p.surname,
                birthday = p.birthday,
                education = p.education,
                position = p.position,
                pay = p.pay,
                _ID = Guid.NewGuid(),
            };
           return q;
        }
        public static void Clear(Employee p)
        {
            p.name = p.surname = p.position = string.Empty;
            p.pay = 0;
            p.birthday = DateTime.Now;
            p.education = Education.žádne;
        }             
    }
    class Employee : Person
    {
        private string _position;
        private double _pay;
        private Education _education;
        public string Status
        {
            get => $"{name} {surname} {birthday.ToString("MM.dd.yyyy")} {education} {position} {pay}";
        }
        public string position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged("position");
                OnPropertyChanged("Status");
            }
        }
        public double pay
        {
            get => _pay;
            set
            {
                _pay = value;
                OnPropertyChanged("pay");
                OnPropertyChanged("Status");
            }
        }
        public Education education
        {
            get => _education;
            set
            {
                _education = value;
                OnPropertyChanged("education");
                OnPropertyChanged("Status");
            }
        }
        public override string ToString()
        {
            return $"{name} {surname} {birthday.ToString("MM.dd.yyyy")} {education} {position} {pay} {_ID}";
        }
    }
}
