using System;
using Nethereum.Web3.Accounts;
using Nethereum.HdWallet;

namespace Api;

    public interface IWalletService
    {
        public static readonly int WALLETS_CNT = 16;
        Account GenerateSensorWallet(int sensorId);
        string GetSensorWalletAddress(int sensorId);
        string GetSensorWalletPrivateKey(int sensorId);
    }

