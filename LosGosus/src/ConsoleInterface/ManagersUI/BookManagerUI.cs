using LosGosus.ConsoleInterface.ServicesUI;
using LosGosus.Controller;
using LosGosus.Services;
using LosGosus.Services.ErrorHandler.Exceptions;
using LosGosus.Services.ErrorHandler;

using Spectre.Console;

namespace LosGosus.ConsoleInterface.ManagersUI;

public class BookManagerUI
{
    private BookController bookController;
    private BookSearcherUI bookSearcherUI;

    public BookManagerUI(BookController _bookController)
    {
        bookController = _bookController;
        bookSearcherUI = new BookSearcherUI(bookController.GetBookManager());
    }

    public void Run()
    {
        while (true)
        {
            AnsiConsole.Clear();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Library Management System - Book Management[/]")
                    .PageSize(10)
                    .AddChoices(new[] {
                        "1. Add Book",
                        "2. Update Book",
                        "3. Delete Book",
                        "4. List Books",
                        "5. List Books by Genre",
                        "6. Search Books",
                        "0. Go back"
                    }));

            switch (choice)
            {
                case "1. Add Book":
                    AddBook();
                    break;
                case "2. Update Book":
                    UpdateBook();
                    break;
                case "3. Delete Book":
                    DeleteBook();
                    break;
                case "4. List Books":
                    ListBooks();
                    break;
                case "5. List Books by Genre":
                    ListBooksByGenre();
                    break;
                case "6. Search Books":
                    SearchBooks();
                    break;
                case "0. Go back":
                    return;
                default:
                    AnsiConsole.MarkupLine("[red]Invalid option. Please try again.[/]");
                    break;
            }
        }
    }

    private void AddBook()
    {
        AnsiConsole.MarkupLine("[bold yellow]Enter Book Details:[/]");
        string title = AnsiConsole.Ask<string>("Title:");
        string author = AnsiConsole.Ask<string>("Author:");
        string isbn = Generator.GenerateISBN();
        string genre = AnsiConsole.Ask<string>("Genre:");
        int publicationYear = AnsiConsole.Ask<int>("Publication Year:");

        bool success = bookController.TryAddBook(title, author, isbn, genre, publicationYear);

        if (success)
        {
            AnsiConsole.MarkupLine("[green]Book added successfully![/]");
        }
        else
        {
            Handler.HandleError(new InvalidBookException("Invalid Book Information"));
        }

        Pause();
    }

    private void UpdateBook()
    {
        AnsiConsole.MarkupLine("[bold yellow]Enter Book Details to Update:[/]");
        string title = AnsiConsole.Ask<string>("Title:");
        string author = AnsiConsole.Ask<string>("Author:");
        string isbn = AnsiConsole.Ask<string>("ISBN:");
        string genre = AnsiConsole.Ask<string>("Genre:");
        int publicationYear = AnsiConsole.Ask<int>("Publication Year:");

        try
        {
            bool success = bookController.TryUpdateBook(title, author, isbn, genre, publicationYear);
            if (success)
            {
                AnsiConsole.MarkupLine("[green]Book updated successfully![/]");
            }
            else
            {
                Handler.HandleError(new InvalidBookException("Book is not valid. Please check the details and try again."));
            }
        } catch 
        {

            Handler.HandleError(new InvalidBookException("Book is not valid or doesn't exists."));
        }

        Pause();
    }

    private void DeleteBook()
    {
        string isbn = AnsiConsole.Ask<string>("Enter ISBN of the book to delete:");
        try
        {
            if(bookController.DeleteBook(isbn))
            {
                AnsiConsole.MarkupLine("[green]Book deleted successfully![/]");
            }else
            {
                AnsiConsole.MarkupLine("[red]Error deleting the book[/]");
            }
        }
        catch (Exception)
        {
            Handler.HandleError(new InvalidBookException("Error deleting the book"));
        }
        Pause();
    }

    private void ListBooks()
    {
        AnsiConsole.MarkupLine("[bold yellow]Listing all books:[/]");
        bookController.ListBooks();
    }

    private void ListBooksByGenre()
    {
        string genre = AnsiConsole.Ask<string>("Enter genre to list books:");
        bookController.ListBooksByGenre(genre);
    }

    private void SearchBooks()
    {
        bookSearcherUI.Run();
    }

    private void Pause()
    {
        AnsiConsole.MarkupLine("[gray]Press any key to continue...[/]");
        Console.ReadKey(true);
    }
}
