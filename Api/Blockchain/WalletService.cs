﻿using Nethereum.Web3.Accounts;
using Nethereum.HdWallet;

namespace Api
{
    public class WalletService : IWalletService
    {
        private readonly string _mnemonic;
        private event EventHandler<LogEventArgs>? OnLog;

        public WalletService(string mnemonic)
        {
            OnLog += Logger.Instance.Log;
            _mnemonic = mnemonic;
            OnLog?.Invoke(this, new LogEventArgs("Wallet service started", LogLevel.Success));
        }

        public Account GenerateSensorWallet(int sensorId)
        {
            var wallet = new Wallet(_mnemonic, null);
            var account = wallet.GetAccount(sensorId);

            return account;
        }

        public string GetSensorWalletAddress(int sensorId)
        {
            var account = GenerateSensorWallet(sensorId);
            return account.Address;
        }

        public string GetSensorWalletPrivateKey(int sensorId)
        {
            var account = GenerateSensorWallet(sensorId);
            return account.PrivateKey;
        }

        
    }
}
