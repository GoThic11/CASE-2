using System.Windows;
using WpfApp1;

namespace Case2
{
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = txtNewUsername.Text;
            string password = txtNewPassword.Password;

            if (username == "Username" || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите имя пользователя и пароль.");
                return;
            }

            if (!DbHelper.RegisterUser(username, password))
            {
                MessageBox.Show("Пользователь уже существует.");
                return;
            }

            MessageBox.Show("Вы успешно зарегистрированы!");
            this.Close();
        }

        private void TxtNewUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtNewUsername.Text == "Username")
            {
                txtNewUsername.Text = "";
                txtNewUsername.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TxtNewUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewUsername.Text))
            {
                txtNewUsername.Text = "Username";
                txtNewUsername.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}