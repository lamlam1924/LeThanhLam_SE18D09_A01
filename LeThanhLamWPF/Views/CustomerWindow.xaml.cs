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
using Models;
using Services;

namespace LeThanhLamWPF.Views
{
    /// <summary>
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        private Customer _currentCustomer;
        private readonly ICustomerService _customerService;
        private readonly IBookingService _bookingService;

        public CustomerWindow(Customer customer)
        {
            InitializeComponent();
            _currentCustomer = customer;
            _customerService = new CustomerService();
            _bookingService = new BookingService();
            LoadCustomerData();
        }

        private void LoadCustomerData()
        {
            WelcomeTextBlock.Text = $"Welcome, {_currentCustomer.CustomerFullName}";
            FullNameTextBlock.Text = _currentCustomer.CustomerFullName;
            EmailTextBlock.Text = _currentCustomer.EmailAddress;
            PhoneTextBlock.Text = _currentCustomer.Telephone;
            BirthdayTextBlock.Text = _currentCustomer.CustomerBirthday.ToString("dd/MM/yyyy");
            CustomerIDTextBlock.Text = _currentCustomer.CustomerID.ToString();
        }

        private void UpdateProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomerDialog(_currentCustomer);
            if (dialog.ShowDialog() == true)
            {
                _customerService.UpdateCustomer(dialog.Customer);
                _currentCustomer = dialog.Customer;
                LoadCustomerData();
                MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void NewBookingButton_Click(object sender, RoutedEventArgs e)
        {
            var bookingDialog = new BookingDialog(_currentCustomer);
            bookingDialog.Owner = this;
            bookingDialog.ShowDialog();
        }

        private void ViewBookingHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new BookingHistoryWindow(_currentCustomer);
            historyWindow.Owner = this;
            historyWindow.ShowDialog();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
