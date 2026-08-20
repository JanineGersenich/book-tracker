using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BookTracker.Models;
using BookTracker.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookTracker.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly BookRepository _repository;
    private List<Book> _allBooks = new();

    [ObservableProperty]
    private ObservableCollection<Book> books = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string? selectedGenreFilter;

    [ObservableProperty]
    private ReadingStatus? selectedStatusFilter;

    [ObservableProperty]
    private ObservableCollection<string> availableGenres = new();

    [ObservableProperty]
    private Book? selectedBook;

    public AnalyticsViewModel Analytics { get; }

    public BookRepository Repository => _repository;

    public MainViewModel()
    {
        _repository = new BookRepository();
        Analytics = new AnalyticsViewModel(_repository);
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        _allBooks = await _repository.GetAllBooksAsync();

        AvailableGenres = new ObservableCollection<string>(
            _allBooks.Select(b => b.Genre)
                     .Where(g => !string.IsNullOrWhiteSpace(g))
                     .Distinct()
                     .OrderBy(g => g));

        ApplyFilter();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    partial void OnSelectedGenreFilterChanged(string? value) => ApplyFilter();

    partial void OnSelectedStatusFilterChanged(ReadingStatus? value) => ApplyFilter();

    private void ApplyFilter()
    {
        IEnumerable<Book> filtered = _allBooks;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var term = SearchText.Trim().ToLowerInvariant();
            filtered = filtered.Where(b =>
                b.Title.ToLowerInvariant().Contains(term) ||
                (b.Author?.Name.ToLowerInvariant().Contains(term) ?? false));
        }

        if (!string.IsNullOrWhiteSpace(SelectedGenreFilter))
        {
            filtered = filtered.Where(b => b.Genre == SelectedGenreFilter);
        }

        if (SelectedStatusFilter != null)
        {
            filtered = filtered.Where(b => b.Status == SelectedStatusFilter);
        }

        Books = new ObservableCollection<Book>(filtered);
    }

    [RelayCommand]
    private async Task DeleteBookAsync(Book? book)
    {
        if (book == null) return;

        await _repository.DeleteBookAsync(book.Id);
        await LoadAsync();
    }

    /// <summary>Wird nach dem Schließen des Bearbeiten-Dialogs aufgerufen.</summary>
    public async Task RefreshAfterEditAsync()
    {
        await LoadAsync();
        await Analytics.LoadAsync();
    }
}
