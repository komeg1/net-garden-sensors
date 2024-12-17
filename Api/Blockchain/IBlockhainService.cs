namespace Api;

    public interface IBlockchainService
    {
        Task<decimal> GetBalanceAsync(string sensorWalletAddress);
        Task RewardSensorAsync(string sensorWalletAddress, decimal tokenAmount);
    }