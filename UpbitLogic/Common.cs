using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit
{
    public enum Api
    {
        APIKeyInfo, AccountInfo, 
        AccountWallet, 
        CandleDays, CandleMinutes, CandleMonth, CandleWeeks, 
        DepositCoinAddress, DepositCoinAddresses, DepositGenerateCoinAddress, DepositInfo, DepositInfoAll, 
        MarketInfoAll, 
        OrderCancel, OrderChance, OrderInfo, OrderInfoAll, OrderNew, OrderOrderbook, 
        TradeTicker, TradeTicks, 
        WithdrawChance, WithdrawCoin, WithdrawInfo, WithdrawInfoAll, WithdrawKrw
    }

    //public class JsonRes : List<Dictionary<string, JsonElement>> { }


}
