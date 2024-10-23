using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace Api
{
    public class BlockchainService
    {
        private static string contractAddress = "";
        private static string privateKey = "";
        private static string infuraUrl = "";
        private static Web3 web3;
        private static string contractABI = @"[
			{
				""inputs"": [],
				""stateMutability"": ""nonpayable"",
				""type"": ""constructor""
			},
			{
				""anonymous"": false,
				""inputs"": [
					{
						""indexed"": true,
						""internalType"": ""address"",
						""name"": ""tokenOwner"",
						""type"": ""address""
					},
					{
						""indexed"": true,
						""internalType"": ""address"",
						""name"": ""spender"",
						""type"": ""address""
					},
					{
						""indexed"": false,
						""internalType"": ""uint256"",
						""name"": ""tokens"",
						""type"": ""uint256""
					}
				],
				""name"": ""Approval"",
				""type"": ""event""
			},
			{
				""inputs"": [
					{
						""internalType"": ""address"",
						""name"": ""spender"",
						""type"": ""address""
					},
					{
						""internalType"": ""uint256"",
						""name"": ""tokens"",
						""type"": ""uint256""
					}
				],
				""name"": ""approve"",
				""outputs"": [
					{
						""internalType"": ""bool"",
						""name"": ""success"",
						""type"": ""bool""
					}
				],
				""stateMutability"": ""nonpayable"",
				""type"": ""function""
			},
			{
				""inputs"": [
					{
						""internalType"": ""address"",
						""name"": ""to"",
						""type"": ""address""
					},
					{
						""internalType"": ""uint256"",
						""name"": ""tokens"",
						""type"": ""uint256""
					}
				],
				""name"": ""transfer"",
				""outputs"": [
					{
						""internalType"": ""bool"",
						""name"": ""success"",
						""type"": ""bool""
					}
				],
				""stateMutability"": ""nonpayable"",
				""type"": ""function""
			},
			{
				""anonymous"": false,
				""inputs"": [
					{
						""indexed"": true,
						""internalType"": ""address"",
						""name"": ""from"",
						""type"": ""address""
					},
					{
						""indexed"": true,
						""internalType"": ""address"",
						""name"": ""to"",
						""type"": ""address""
					},
					{
						""indexed"": false,
						""internalType"": ""uint256"",
						""name"": ""tokens"",
						""type"": ""uint256""
					}
				],
				""name"": ""Transfer"",
				""type"": ""event""
			},
			{
				""inputs"": [
					{
						""internalType"": ""address"",
						""name"": ""from"",
						""type"": ""address""
					},
					{
						""internalType"": ""address"",
						""name"": ""to"",
						""type"": ""address""
					},
					{
						""internalType"": ""uint256"",
						""name"": ""tokens"",
						""type"": ""uint256""
					}
				],
				""name"": ""transferFrom"",
				""outputs"": [
					{
						""internalType"": ""bool"",
						""name"": ""success"",
						""type"": ""bool""
					}
				],
				""stateMutability"": ""nonpayable"",
				""type"": ""function""
			},
			{
				""inputs"": [],
				""name"": ""_totalSupply"",
				""outputs"": [
					{
						""internalType"": ""uint256"",
						""name"": """",
						""type"": ""uint256""
					}
				],
				""stateMutability"": ""view"",
				""type"": ""function""
			},
			{
				""inputs"": [
					{
						""internalType"": ""address"",
						""name"": ""tokenOwner"",
						""type"": ""address""
					},
					{
						""internalType"": ""address"",
						""name"": ""spender"",
						""type"": ""address""
					}
				],
				""name"": ""allowance"",
				""outputs"": [
					{
						""internalType"": ""uint256"",
						""name"": ""remaining"",
						""type"": ""uint256""
					}
				],
				""stateMutability"": ""view"",
				""type"": ""function""
			},
			{
				""inputs"": [
					{
						""internalType"": ""address"",
						""name"": ""tokenOwner"",
						""type"": ""address""
					}
				],
				""name"": ""balanceOf"",
				""outputs"": [
					{
						""internalType"": ""uint256"",
						""name"": ""balance"",
						""type"": ""uint256""
					}
				],
				""stateMutability"": ""view"",
				""type"": ""function""
			},
			{
				""inputs"": [],
				""name"": ""decimals"",
				""outputs"": [
					{
						""internalType"": ""uint8"",
						""name"": """",
						""type"": ""uint8""
					}
				],
				""stateMutability"": ""view"",
				""type"": ""function""
			},
			{
				""inputs"": [],
				""name"": ""name"",
				""outputs"": [
					{
						""internalType"": ""string"",
						""name"": """",
						""type"": ""string""
					}
				],
				""stateMutability"": ""view"",
				""type"": ""function""
			},
			{
				""inputs"": [],
				""name"": ""symbol"",
				""outputs"": [
					{
						""internalType"": ""string"",
						""name"": """",
						""type"": ""string""
					}
				],
				""stateMutability"": ""view"",
				""type"": ""function""
			},
			{
				""inputs"": [],
				""name"": ""totalSupply"",
				""outputs"": [
					{
						""internalType"": ""uint256"",
						""name"": """",
						""type"": ""uint256""
					}
				],
				""stateMutability"": ""view"",
				""type"": ""function""
			}
		]";

        public BlockchainService()
        {
            var account = new Account(privateKey);
            web3 = new Web3(account, infuraUrl);
        }

        public async Task RewardSensorAsync(string sensorWalletAddress, decimal tokenAmount)
        {
            var contract = web3.Eth.GetContract(contractABI, contractAddress);
            var transferFunction = contract.GetFunction("transfer");

            var decimals = 18;
            var amountInWei = Web3.Convert.ToWei(tokenAmount, decimals);

            try
            {
                var transactionHash = await transferFunction.SendTransactionAsync(
                    web3.TransactionManager.Account.Address,
                    sensorWalletAddress,
                    new HexBigInteger(21000),
                    new HexBigInteger(amountInWei)
                );
                Console.WriteLine($"Tokens sent! Transaction hash: {transactionHash}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending tokens: {ex.Message}");
            }
        }
    }

}
