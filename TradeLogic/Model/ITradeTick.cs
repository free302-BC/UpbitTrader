
namespace Universe.Coin.TradeLogic.Model
{
    public interface ITradeTick : IApiModel
    {
        string AskBid { get; set; }
        decimal ChangePrice { get; set; }
        
        string Market { get; set; }
        string Code { get; set; }

        decimal PrevPrice { get; set; }
        long SequentialId { get; set; }
        long Timestamp { get; set; }
        string TradeDateUtc { get; set; }
        decimal TradePrice { get; set; }
        string TradeTimeUtc { get; set; }
        decimal TradeVolume { get; set; }

        string ToString();
    }
}