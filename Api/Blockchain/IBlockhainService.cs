namespace Api;

    public interface IBlockchainService
    {
        Task<decimal> GetBalanceAsync(string sensorWalletAddress);
        Task<Dictionary<int, decimal>> GetBalanceAsync();
        Task RewardSensorAsync(string sensorWalletAddress, decimal tokenAmount);
    }