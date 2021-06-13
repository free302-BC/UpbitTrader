using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Model
{
    public interface IMarketInfo : IApiModel
    {
        string EnglishName { get; set; }
        string KoreanName { get; set; }
        string Market { get; set; }
        string MarketWarning { get; set; }

        string ToString();
    }
}