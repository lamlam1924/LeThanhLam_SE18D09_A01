﻿using System;
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
    /// Interaction logic for RoomDialog.xaml
    /// </summary>
    public partial class RoomDialog : Window
    {
        public RoomInformation Room { get; private set; }
        private readonly IRoomService _roomService;

        public RoomDialog(RoomInformation room = null)
        {
            InitializeComponent();
            _roomService = new RoomService();

            // Load room types
            RoomTypeComboBox.ItemsSource = _roomService.GetAllRoomTypes();

            if (room != null)
            {
                Room = new RoomInformation
                {
                    RoomID = room.RoomID,
                    RoomNumber = room.RoomNumber,
                    RoomDescription = room.RoomDescription,
                    RoomMaxCapacity = room.RoomMaxCapacity,
                    RoomPricePerDate = room.RoomPricePerDate,
                    RoomTypeID = room.RoomTypeID,
                    RoomStatus = room.RoomStatus
                };

                RoomNumberTextBox.Text = Room.RoomNumber;
                DescriptionTextBox.Text = Room.RoomDescription;
                CapacityTextBox.Text = Room.RoomMaxCapacity.ToString();
                PriceTextBox.Text = Room.RoomPricePerDate.ToString();
                RoomTypeComboBox.SelectedValue = Room.RoomTypeID;

                Title = "Edit Room";
            }
            else
            {
                Room = new RoomInformation();
                Title = "Add Room";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(RoomNumberTextBox.Text) ||
                string.IsNullOrEmpty(CapacityTextBox.Text) ||
                string.IsNullOrEmpty(PriceTextBox.Text) ||
                RoomTypeComboBox.SelectedValue == null)
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(CapacityTextBox.Text, out int capacity) || capacity <= 0)
            {
                MessageBox.Show("Please enter a valid capacity.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Room.RoomNumber = RoomNumberTextBox.Text;
            Room.RoomDescription = DescriptionTextBox.Text;
            Room.RoomMaxCapacity = capacity;
            Room.RoomPricePerDate = price;
            Room.RoomTypeID = (int)RoomTypeComboBox.SelectedValue;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
