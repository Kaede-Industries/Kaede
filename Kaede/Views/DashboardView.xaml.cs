﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using static Kaede.ViewModels.AppointmentListingViewModel;

namespace Kaede.Views
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        private void TextBox_NumberPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
            e.Handled |= e.Text == "";
        }

        private static bool IsTextNumeric(string text)
            => Regex.IsMatch(text, "^[0-9]+$");


        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                if (sender == NameTextBox)
                    DescriptionTextBox.Focus();
                else if (sender == DescriptionTextBox)
                    PriceTextBox.Focus();
                else if (sender == PriceTextBox)
                    HoursTextBox.Focus();
                else if (sender == HoursTextBox)
                    MinutesTextBox.Focus();
                else if (sender == MinutesTextBox)
                    SubmitItemButton.Focus();
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton tb)
            {
                var vis = (tb.IsChecked ?? false) ? Visibility.Visible : Visibility.Collapsed;
                switch (tb.Name)
                {
                    case "AddShopItemTB":
                        AddShopItemGrid.Visibility = vis;
                        break;
                    case "ShopItemListTB":
                        ShopItemListGrid.Visibility = vis;
                        break;
                    case "AddAppointmentTB":
                        AddAppointmentGrid.Visibility = vis;
                        break;
                    case "AppointmentListTB":
                        AppointmentListPanel.Visibility = vis;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                return $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}";
            }
            return "00:00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string timeString)
            {
                var parts = timeString.Split(':');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int hours) &&
                    int.TryParse(parts[1], out int minutes))
                {
                    return new TimeSpan(hours, minutes, 0);
                }
            }
            return TimeSpan.Zero;
        }

    }

    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.Parse(targetType, value.ToString());
        }
    }

    public static class GridViewColumnExtensions
    {
        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.RegisterAttached("IsVisible", typeof(bool), typeof(GridViewColumnExtensions), new PropertyMetadata(true, OnIsVisibleChanged));

        public static bool GetIsVisible(DependencyObject obj) => (bool)obj.GetValue(IsVisibleProperty);
        public static void SetIsVisible(DependencyObject obj, bool value) => obj.SetValue(IsVisibleProperty, value);

        private static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GridViewColumn column)
            {
                column.Width = (bool)e.NewValue ? double.NaN : 0;
            }
        }
    }


}
