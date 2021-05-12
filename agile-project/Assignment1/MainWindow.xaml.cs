/* **********************************************************************************
 * For use by students taking 60-422 (Fall, 2014) to work on assignments and project.
 * Permission required material. Contact: xyuan@uwindsor.ca 
 * **********************************************************************************/
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
using System.Data;
using System.Collections.ObjectModel;
using BookStoreDomain;
using BookStoreDAL;

namespace BookStoreGUI
{
    /// Interaction logic for MainWindow.xaml
    public partial class MainWindow : Window
    {

        BookDAL BookDAL;
        LoginDialog LoginDialog;
        UserDashboard UserDashboard;
        RegisterUser RegisterUser;


        Principle CurrentUser;
        List<Book> Catalog;
        List<Category> Categories;
        ObservableCollection<OrderItem> Order;

        public MainWindow() { InitializeComponent(); }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {

            LoginDialog.ShowDialog();

            if (LoginDialog.DialogResult == true)
            {
                CurrentUser.ID = LoginDialog.CurrentUser.ID;
                CurrentUser.Username = LoginDialog.CurrentUser.Username;

                addButton.IsEnabled = true;
                removeButton.IsEnabled = true;
                checkoutOrderButton.IsEnabled = true;
                loginButton.IsEnabled = false;

                userDashboard.IsEnabled = true;

                register.IsEnabled = false;

                statusTextBlock.Text = "You are logged in as: " + CurrentUser.Username;
                UserDashboard = new UserDashboard(CurrentUser);
            }

        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoginDialog = new LoginDialog();
            CurrentUser = new Principle();
            RegisterUser = new RegisterUser();

            BookDAL = new BookDAL();
            Catalog = BookDAL.GetAllBooks();
            booksListView.ItemsSource = Catalog;

            Order = new ObservableCollection<OrderItem>();
            orderListView.ItemsSource = Order;

            CategoryDAL CategoryDAL = new CategoryDAL();
            Categories = CategoryDAL.GetAllCategories();
            categoriesComboBox.ItemsSource = Categories.Select(c => c.Title);

            addButton.IsEnabled = false;
            removeButton.IsEnabled = false;
            checkoutOrderButton.IsEnabled = false;
            userDashboard.IsEnabled = false;
        }

        private void checkoutButton_Click(object sender, RoutedEventArgs e)
        {
            OrderDAL OrderDAL = new OrderDAL();
            int OrderID = OrderDAL.AddOrder(CurrentUser.ID);
            OrderDAL.AddOrderItems(Order, OrderID);
            MessageBox.Show("Your order has been placed. Your order id is " + OrderID.ToString());
        }
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            Book SelectedItem = (Book)booksListView.SelectedItem;

            if(SelectedItem == null)
            {
                MessageBox.Show("You must select a book! ");
            }
            else
            {
            OrderDAL OrderDAL = new OrderDAL();
            int OrderID = OrderDAL.AddOrder(CurrentUser.ID);
            OrderDAL.CalcGrandTotal(Order, CurrentUser.ID);

            OrderItemDialog OrderItemDialog = new OrderItemDialog(SelectedItem.ISBN, SelectedItem.Title, SelectedItem.Price);
            OrderItemDialog.ShowDialog();

                if (OrderItemDialog.DialogResult == true)
                {
                    for (int i = 0; i < Order.Count; i++)
                    {
                        if (Order[i].BookID == SelectedItem.ISBN)
                        {
                            Order[i] = new OrderItem(SelectedItem.ISBN, SelectedItem.Title, SelectedItem.Price, Order[i].Quantity + int.Parse(OrderItemDialog.quantityTextBox.Text));
                            return;
                        }
                    }
                    Order.Add(new OrderItem(SelectedItem.ISBN, SelectedItem.Title, SelectedItem.Price, int.Parse(OrderItemDialog.quantityTextBox.Text)));
                }
            }

        }
        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            OrderItem SelectedItem = (OrderItem)orderListView.SelectedItem;
            Order.Remove(SelectedItem);
        }

        private void categoriesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Catalog = BookDAL.GetBooksByCategory((string)categoriesComboBox.SelectedItem);
            booksListView.ItemsSource = Catalog;
        }


        private void userDashboard_Click(object sender, RoutedEventArgs e)
        {
            UserDashboard = new UserDashboard(CurrentUser);
            UserDashboard.ShowDialog();
        }

        private void register_Click(object sender, RoutedEventArgs e)
        {
            RegisterUser = new RegisterUser();
            RegisterUser.ShowDialog();

            if (RegisterUser.DialogResult == true)
            {
                CurrentUser.ID = RegisterUser.CurrentUser.ID;
                CurrentUser.Username = RegisterUser.CurrentUser.Username;

                addButton.IsEnabled = true;
                removeButton.IsEnabled = true;
                checkoutOrderButton.IsEnabled = true;
                loginButton.IsEnabled = false;
                register.IsEnabled = false;

                statusTextBlock.Text = "You are logged in as: " + CurrentUser.Username;
            }

        }
    }
}
