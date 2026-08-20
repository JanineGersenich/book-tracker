using System.Windows;
using System.Windows.Controls;
using BookTracker.Models;
using BookTracker.ViewModels;

namespace BookTracker.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        DataContext = _viewModel;
        Loaded += async (_, _) => await _viewModel.LoadAsync();
    }

    private async void AnalyticsTab_GotFocus(object sender, RoutedEventArgs e)
    {
        await _viewModel.Analytics.LoadAsync();
    }

    private async void AddBook_Click(object sender, RoutedEventArgs e)
    {
        await OpenEditDialogAsync(existing: null);
    }

    private async void BooksGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_viewModel.SelectedBook != null)
        {
            await OpenEditDialogAsync(_viewModel.SelectedBook);
        }
    }

    private async void DeleteBook_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Book book })
        {
            var result = MessageBox.Show(
                $"\"{book.Title}\" wirklich löschen?",
                "Buch löschen",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await _viewModel.DeleteBookCommand.ExecuteAsync(book);
            }
        }
    }

    private async System.Threading.Tasks.Task OpenEditDialogAsync(Book? existing)
    {
        var editViewModel = new BookEditViewModel(_viewModel.Repository, existing);
        var dialog = new BookEditWindow(editViewModel) { Owner = this };

        dialog.ShowDialog();

        if (editViewModel.DialogResult)
        {
            await _viewModel.RefreshAfterEditAsync();
        }
    }
}
