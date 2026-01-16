namespace TTHK_Link.Services.Interfaces;
using TTHK_Link.Models;

public interface INewsService
{
    Task<List<News>> GetLatestNewsAsync(int count);
    Task<News> GetNewsItemByIdAsync(string newsId);
}
