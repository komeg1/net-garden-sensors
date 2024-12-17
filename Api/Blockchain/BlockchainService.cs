using Api.Entities.Config;
using Microsoft.Extensions.Options;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Numerics;

namespace Api
{
    public class BlockchainService : IBlockchainService
    {

		private string contractAddress;
		private string privateKey;
		private string infuraUrl;
        private static Web3 web3;
        private event EventHandler<LogEventArgs>? OnLog;
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

        public BlockchainService(IOptions<BlockchainSettings> blockchainSettings)
        {
			privateKey = blockchainSettings.Value.PrivateKey;
			contractAddress =blockchainSettings.Value.ContractAddress;
            infuraUrl = blockchainSettings.Value.InfuraUrl;
            OnLog += Logger.Instance.Log;
            var account = new Account(privateKey);
            web3 = new Web3(account, infuraUrl);
            OnLog?.Invoke(this, new LogEventArgs("Blockchain service started", LogLevel.Success));

        }

        public async Task RewardSensorAsync(string sensorWalletAddress, decimal tokenAmount)
		{
			try
			{
				OnLog?.Invoke(this, new LogEventArgs($"Sending {tokenAmount} tokens to {sensorWalletAddress}", LogLevel.Debug));

				// Convert tokenAmount to Wei
				var amountInWei = Web3.Convert.ToWei(tokenAmount, 18);

				// Get the contract instance
				var contract = web3.Eth.GetContract(contractABI, contractAddress);
                var transferFunction = contract.GetFunction("transfer");

				// Validate wallet address
				if (!Web3.IsChecksumAddress(sensorWalletAddress))
				{
					sensorWalletAddress = Web3.ToChecksumAddress(sensorWalletAddress);
				}
                var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();
                OnLog?.Invoke(this, new LogEventArgs($"Current gas price: {gasPrice}", LogLevel.Debug));

                var callInput = transferFunction.CreateCallInput(
				   from: web3.TransactionManager.Account.Address, // Sender's address
				   gas: null,                                    // Let it auto-estimate gas
				   value: null,                                  // No ETH value sent
				   functionInput: new object[] { sensorWalletAddress, amountInWei }
			   );

                // Estimate gas
                var gasEstimate = await web3.Eth.Transactions.EstimateGas.SendRequestAsync(callInput);

                OnLog?.Invoke(this, new LogEventArgs($"Gas estimate: {gasEstimate.Value}", LogLevel.Debug));
                // Send the transaction
                var transactionHash = await transferFunction.SendTransactionAsync(
					from: web3.TransactionManager.Account.Address,
					gas: gasEstimate,
					value: null,
					sensorWalletAddress,
					amountInWei
				);

				OnLog?.Invoke(this, new LogEventArgs($"Transaction successful: {transactionHash}", LogLevel.Success));
			}
            catch (Exception ex)
			{
				OnLog?.Invoke(this, new LogEventArgs($"Error while sending tokens: {ex.Message} | StackTrace: {ex.StackTrace}", LogLevel.Error));
			}
		}
	
		public async Task<decimal> GetBalanceAsync(string sensorWalletAddress)
		{
			try
			{
				OnLog?.Invoke(this, new LogEventArgs($"Getting balance for {sensorWalletAddress}", LogLevel.Debug));

				var contract = web3.Eth.GetContract(contractABI, contractAddress);
				var balanceFunction = contract.GetFunction("balanceOf");

				if (!Web3.IsChecksumAddress(sensorWalletAddress))
				{
					sensorWalletAddress = Web3.ToChecksumAddress(sensorWalletAddress);
				}

				var balance = await balanceFunction.CallAsync<BigInteger>(sensorWalletAddress);
				var balanceInEth = Web3.Convert.FromWei(balance, 18);

				OnLog?.Invoke(this, new LogEventArgs($"Balance: {balanceInEth} tokens", LogLevel.Debug));

				return balanceInEth;
			}
			catch (Exception ex)
			{
				OnLog?.Invoke(this, new LogEventArgs($"Error while getting balance: {ex.Message}", LogLevel.Error));
				return 0;
			}
		}
    }
}
