using System.Text.Encodings.Web;
using BootstrapHtmlHelpers;
using Microsoft.AspNetCore.Html;

namespace Tests;

public class PaginationTests
{
    [Fact]
    public void TestPagination_10Pages_CurrentPage0()
    {
        var output = BsPagination.Paginate(0, 10, page => $"#{page}");
        var expected = "<nav aria-label=\"Page navigation\">" +
                       "<ul class=\"pagination justify-content-center \">" +
                       "<li class=\"page-item active\"><a class=\"page-link\" href=\"#1\">1</a></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#2\">2</a></li>" +
                       "<li class=\"page-item\"><span class=\"page-text\">&hellip;</span></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#10\">10</a></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#2\">Next</a></li>" +
                       "</ul>" +
                       "</nav>";
        using var writer = new StringWriter();
        output.WriteTo(writer, HtmlEncoder.Default);
        var rendered = writer.ToString();
        Assert.Equal(expected, rendered);
    }
    
    [Fact]
    public void TestPaginationOnePage()
    {
        var output = BsPagination.Paginate(1, 1, page => $"#{page}");
        var expected = "<nav aria-label=\"Page navigation\">" +
                       "<ul class=\"pagination justify-content-center \">" +
                       "<li class=\"page-item active\"><a class=\"page-link\" href=\"#1\">1</a></li>" +
                       "</ul>" +
                       "</nav>";
        using var writer = new StringWriter();
        output.WriteTo(writer, HtmlEncoder.Default);
        var rendered = writer.ToString();
        Assert.Equal(expected, rendered);
    }
    
    [Fact]
    public void TestPagination_10Pages_CurrentPage1()
    {
        var output = BsPagination.Paginate(1, 10, page => $"#{page}");
        var expected = "<nav aria-label=\"Page navigation\">" +
                       "<ul class=\"pagination justify-content-center \">" +
                       "<li class=\"page-item active\"><a class=\"page-link\" href=\"#1\">1</a></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#2\">2</a></li>" +
                       "<li class=\"page-item\"><span class=\"page-text\">&hellip;</span></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#10\">10</a></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#2\">Next</a></li>" +
                       "</ul>" +
                       "</nav>";
        using var writer = new StringWriter();
        output.WriteTo(writer, HtmlEncoder.Default);
        var rendered = writer.ToString();
        Assert.Equal(expected, rendered);
    }
    
    [Fact]
    public void TestPagination_10Pages_CurrentPage2()
    {
        var output = BsPagination.Paginate(2, 10, page => $"#{page}");
        var expected = "<nav aria-label=\"Page navigation\">" +
                       "<ul class=\"pagination justify-content-center \">" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#1\">Previous</a></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#1\">1</a></li>" +
                       "<li class=\"page-item active\"><a class=\"page-link\" href=\"#2\">2</a></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#3\">3</a></li>" +
                       "<li class=\"page-item\"><span class=\"page-text\">&hellip;</span></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#10\">10</a></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#3\">Next</a></li>" +
                       "</ul>" +
                       "</nav>";
        using var writer = new StringWriter();
        output.WriteTo(writer, HtmlEncoder.Default);
        var rendered = writer.ToString();
        Assert.Equal(expected, rendered);
    }
    
    [Fact]
    public void TestPagination_10Pages_CurrentPage5()
    {
        var output = BsPagination.Paginate(5, 10, page => $"#{page}");
        var expected = "<nav aria-label=\"Page navigation\">" +
                       "<ul class=\"pagination justify-content-center \">" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#4\">Previous</a></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#1\">1</a></li>" +
                       "<li class=\"page-item\"><span class=\"page-text\">&hellip;</span></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#4\">4</a></li>" +
                       "<li class=\"page-item active\"><a class=\"page-link\" href=\"#5\">5</a></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#6\">6</a></li>" +
                       "<li class=\"page-item\"><span class=\"page-text\">&hellip;</span></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#10\">10</a></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#6\">Next</a></li>" +
                       "</ul>" +
                       "</nav>";
        using var writer = new StringWriter();
        output.WriteTo(writer, HtmlEncoder.Default);
        var rendered = writer.ToString();
        Assert.Equal(expected, rendered);
    }
    
    [Fact]
    public void TestPagination_10Pages_CurrentPage9()
    {
        var output = BsPagination.Paginate(9, 10, page => $"#{page}");
        var expected = "<nav aria-label=\"Page navigation\">" +
                       "<ul class=\"pagination justify-content-center \">" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#8\">Previous</a></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#1\">1</a></li>" +
                       "<li class=\"page-item\"><span class=\"page-text\">&hellip;</span></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#8\">8</a></li>" +
                       "<li class=\"page-item active\"><a class=\"page-link\" href=\"#9\">9</a></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#10\">10</a></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#10\">Next</a></li>" +
                       "</ul>" +
                       "</nav>";
        using var writer = new StringWriter();
        output.WriteTo(writer, HtmlEncoder.Default);
        var rendered = writer.ToString();
        Assert.Equal(expected, rendered);
    }
    
    [Fact]
    public void TestPagination_10Pages_CurrentPage10()
    {
        var output = BsPagination.Paginate(10, 10, page => $"#{page}");
        var expected = "<nav aria-label=\"Page navigation\">" +
                       "<ul class=\"pagination justify-content-center \">" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#9\">Previous</a></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#1\">1</a></li>" +
                       "<li class=\"page-item\"><span class=\"page-text\">&hellip;</span></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#9\">9</a></li>" +
                       "<li class=\"page-item active\"><a class=\"page-link\" href=\"#10\">10</a></li>" +
                       "</ul>" +
                       "</nav>";
        using var writer = new StringWriter();
        output.WriteTo(writer, HtmlEncoder.Default);
        var rendered = writer.ToString();
        Assert.Equal(expected, rendered);
    }
    
    [Fact]
    public void TestPagination_10Pages_CurrentPage11()
    {
        var output = BsPagination.Paginate(11, 10, page => $"#{page}");
        var expected = "<nav aria-label=\"Page navigation\">" +
                       "<ul class=\"pagination justify-content-center \">" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#9\">Previous</a></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#1\">1</a></li>" +
                       "<li class=\"page-item\"><span class=\"page-text\">&hellip;</span></li>" +
                       "<li class=\"page-item \"><a class=\"page-link\" href=\"#9\">9</a></li>" +
                       "<li class=\"page-item active\"><a class=\"page-link\" href=\"#10\">10</a></li>" +
                       "</ul>" +
                       "</nav>";
        using var writer = new StringWriter();
        output.WriteTo(writer, HtmlEncoder.Default);
        var rendered = writer.ToString();
        Assert.Equal(expected, rendered);
    }

    [Theory]
    [InlineData(1, 20, 1)]
    [InlineData(5, 20, 1)]
    [InlineData(24, 20, 2)]
    [InlineData(30, 20, 2)]
    [InlineData(100, 20, 5)]
    public void Test_TotalPages(int totalItems, int perPage, int expectedPages)
    {
        var actualPages = BsPagination.TotalPages(totalItems, perPage);
        Assert.Equal(expectedPages, actualPages);
    }

    [Theory]
    [InlineData(1, 10, 1)]
    [InlineData(0, 10, 1)]
    [InlineData(11, 10, 10)]
    public void Test_MinMax(int page, int totalPages, int expected)
    {
        var actual = BsPagination.MinMax(page, totalPages);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1, 10, 10, 0)]
    [InlineData(5, 10, 10, 40)]
    [InlineData(12, 10, 10, 90)]
    [InlineData(0, 10, 10, 0)]
    public void Test_Offset(int page, int perPage, int totalPages, int expected)
    {
        var actual = BsPagination.Offset(page, perPage, totalPages);
        Assert.Equal(expected, actual);
    }
}