﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit
{
    public enum ApiId
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

    //public enum MarketId
    //{
    //    BTC, ETH, NEO, MTL, LTC, XRP, ETC, OMG, SNT, WAVES, XEM, QTUM, LSK, STEEM, XLM, ARDR, KMD, ARK, STORJ, GRS,
    //    REP, EMC2, ADA, SBD, POWR, BTG, ICX, EOS, TRX, SC, IGNIS, ONT, ZIL, POLY, ZRX, LOOM, BCH, ADX, BAT, IOST,
    //    DMT, RFR, CVC, IQ, IOTA, MFT, ONG, GAS, UPP, ELF, KNC, BSV, THETA, EDR, QKC, BTT, MOC, ENJ, TFUEL, MANA, ANKR,
    //    AERGO, ATOM, TT, CRE, SOLVE, MBL, TSHP, WAXP, HBAR, MED, MLK, STPT, ORBS, VET, CHZ, PXL, STMX, DKA, HIVE, KAVA,
    //    AHT, LINK, XTZ, BORA, JST, CRO, TON, SXP, LAMB, HUNT, MARO, PLA, DOT, SRM, MVL, PCI, STRAX, AQT, BCHA, GLM,
    //    QTCON, SSX, META, OBSR, FCT2, LBC, CBK, SAND, HUM, DOGE, STRK, PUNDIX, FLOW, DAWN, AXS, STX
    //}
    //public enum MarketIdBtc
    //{
    //    ETH, LTC, XRP, ETC, OMG, CVC, DGB, SC, SNT, WAVES, NMR, XEM, LBC, QTUM, NXT, BAT, LSK, RDD, STEEM, DOGE, BNT,
    //    XLM, ARDR, KMD, ARK, ADX, SYS, ANT, STORJ, GRS, REP, RLC, EMC2, ADA, MANA, SBD, RCN, POWR, DNT, IGNIS, ZRX,
    //    TRX, TUSD, LRC, DMT, POLY, PRO, BCH, MFT, LOOM, RFR, RVN, BFT, GO, UPP, ENJ, EDR, MTL, PAX, MOC, ZIL, BSV,
    //    IOST, NCASH, DENT, ELF, BTT, VITE, IOTX, SOLVE, NKN, META, ANKR, CRO, FSN, ORBS, AERGO, PI, ATOM, STPT, LAMB,
    //    EOS, LUNA, DAI, MKR, BORA, TSHP, WAXP, MED, MLK, PXL, VET, CHZ, FX, OGN, ITAM, XTZ, HIVE, HBD, OBSR, DKA, STMX,
    //    AHT, PCI, RINGX, LINK, KAVA, JST, CHR, DAD, TON, CTSI, DOT, COMP, SXP, HUNT, ONIT, CRV, ALGO, RSR, OXT, PLA,
    //    MARO, SAND, SUN, SRM, QTCON, MVL, GXC, AQT, AXS, STRAX, BCHA, GLM, FCT2, SSX, FIL, UNI, BASIC, INJ, PROM, VAL,
    //    PSG, JUV, CBK, FOR, BFC, LINA, HUM, PICA, CELO, IQ, STX, NEAR, AUCTION, DAWN, FLOW, STRK, PUNDIX, GRT, SNX
    //}

    public enum CurrencyId
    {
        KRW, USDT, BTC
    }
    
    //public class JsonRes : List<Dictionary<string, JsonElement>> { }


}
