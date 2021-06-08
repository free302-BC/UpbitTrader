
namespace Universe.Coin.TradeLogic.Model
{
    public interface ITicker : IApiModel
    {
        string Market { get; set; }
        string Code { get; set; }

        decimal AccTradePrice { get; set; }
        decimal AccTradePrice24h { get; set; }
        decimal AccTradeVolume { get; set; }
        decimal AccTradeVolume24h { get; set; }
        string Change { get; set; }
        decimal ChangePrice { get; set; }
        decimal ChangeRate { get; set; }
        string Highest52WeekDate { get; set; }
        decimal Highest52WeekPrice { get; set; }
        decimal HighPrice { get; set; }
        string Lowest52WeekDate { get; set; }
        decimal Lowest52WeekPrice { get; set; }
        decimal LowPrice { get; set; }
        decimal OpeningPrice { get; set; }
        decimal PrevClosingPrice { get; set; }
        decimal SignedChangePrice { get; set; }
        decimal SignedChangeRate { get; set; }
        long Timestamp { get; set; }
        string TradeDate { get; set; }
        string TradeDateKst { get; set; }
        decimal TradePrice { get; set; }
        string TradeTime { get; set; }
        string TradeTimeKst { get; set; }
        decimal TradeVolume { get; set; }

        string ToString();
    }
}