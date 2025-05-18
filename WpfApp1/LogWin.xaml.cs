using System.Windows;
using System.Windows.Input;
using WpfApp1;

namespace Case2
{
    public partial class LogWin : Window
    {
        public LogWin()
        {
            InitializeComponent();
        }

        private void BtnSignIn_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (username == "Username" || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите имя пользователя и пароль.");
                return;
            }

            if (DbHelper.ValidateUser(username, password))
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль.");
            }
        }

        private void TxtRegister_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.ShowDialog();
        }

        private void TxtUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text == "Username")
            {
                txtUsername.Text = "";
                txtUsername.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TxtUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtUsername.Text = "Username";
                txtUsername.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void TxtPasswordPlaceholder_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPassword.Focus();
        }

        private void TxtPasswordPlaceholder_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {

            }
        }
    }
}