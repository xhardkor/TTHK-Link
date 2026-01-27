using TTHK_Link.Models;
using TTHK_Link.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TTHK_Link.Services.Fake
{
    public class FakeNewsService : INewsService
    {
        public Task<List<News>> GetLatestNewsAsync(int count)
        {
            var fakeNews = new List<News>
            {
                new News
                {
                    Id = "1",
                    Title = "Eesti Vabariigi president Alar Karis külastas Tallinna Tööstushariduskeskust",
                    Link = "https://www.tthk.ee/news/eesti-vabariigi-president-alar-karis-kulastas-tallinna-toostushariduskeskust/",
                    Summary = "Tallinna Tööstushariduskeskus võõrustas Eesti Vabariigi presidenti Alar Karist ...",
                    PublishDate = DateTime.Now.AddDays(-1),
                    ImageUrl = "https://www.tthk.ee/wp-content/uploads/2024/11/DSC5951-1024x740.png"
                },
                new News
                {
                    Id = "2",
                    Title = "UUS !!! osale – AS Tallinna Vesi stipendiumikonkurss",
                    Link = "https://www.tthk.ee/news/as-tallinna-vesi-stipendiumikonkurss/",
                    Summary = "Ettevõte pani välja kaks stipendiumi Tallinna Tööstushariduskeskuseenergeetika ja automaatika õppekava mehhatroonika eriala II kursuse õpilastele.Stipendiumi suurus on 2000 eurot. ",
                    PublishDate = DateTime.Now.AddDays(-3),
                    ImageUrl = "https://www.tthk.ee/wp-content/uploads/2023/10/Sotsmeediasse-TVESI-stipendium-TTHK-2025.jpg "
                },
            };

            return Task.FromResult(fakeNews.Take(count).ToList());
        }

        public Task<News> GetNewsItemByIdAsync(string newsId)
            => throw new NotImplementedException();
    }
}
    