namespace Api.Entities.Config
{
    public class BlockchainSettings
    {
        public required string ContractAddress { get;set; }
        public required string PrivateKey { get;set; }
        public required string InfuraUrl { get;set; }
    }
}

