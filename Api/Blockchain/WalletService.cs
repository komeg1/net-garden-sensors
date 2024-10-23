using Nethereum.Web3.Accounts;
using Nethereum.HdWallet;

namespace Api
{
    public class WalletService
    {
        private readonly string _mnemonic;

        public WalletService(string mnemonic)
        {
            _mnemonic = mnemonic;
        }

        public Account GenerateSensorWallet(int sensorId)
        {
            // Utwórz portfel na podstawie podanego mnemonika
            var wallet = new Wallet(_mnemonic, null);

            // Generuj adres portfela dla konkretnego sensora
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
