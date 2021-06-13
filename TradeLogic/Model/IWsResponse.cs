namespace Universe.Coin.TradeLogic.Model
{
    public interface IWsResponse : IApiModel
    {
        string Market { get; set; }
        string StreamType { get; set; }
        TradeEvent Event { get; set; }
    }

}