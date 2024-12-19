using Api.Entities.Config;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Numerics;

namespace Api
{
    public class BlockchainService : IBlockchainService
    {
		private readonly IWalletService _walletService;
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

        public BlockchainService(IOptions<BlockchainSettings> blockchainSettings, IWalletService walletService)
        {
			_walletService = walletService;
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

				var amountInWei = Web3.Convert.ToWei(tokenAmount, 18);
				var contract = web3.Eth.GetContract(contractABI, contractAddress);
                var transferFunction = contract.GetFunction("transfer");
                var gasEstimate = new HexBigInteger(60000);
				var transactionHash = await transferFunction.SendTransactionAsync(web3.TransactionManager.Account.Address, gasEstimate, null, null, sensorWalletAddress, amountInWei);
			}
            catch (Exception ex)
			{
				OnLog?.Invoke(this, new LogEventArgs($"Error while sending tokens: {ex.Message}", LogLevel.Error));
			}
		}
	
		public async Task<decimal> GetBalanceAsync(string sensorWalletAddress)
		{
            int retries = 5;
            int delayInMs = 250;
			for (int i = 0; i < retries; i++)
			{
				try
				{
					OnLog?.Invoke(this, new LogEventArgs($"Getting balance for {sensorWalletAddress}", LogLevel.Debug));

					var contract = web3.Eth.GetContract(contractABI, contractAddress);
					var balanceFunction = contract.GetFunction("balanceOf");
					var balance = await balanceFunction.CallAsync<BigInteger>(sensorWalletAddress);
					var balanceInEth = Web3.Convert.FromWei(balance, 18);
					OnLog?.Invoke(this, new LogEventArgs($"Balance: {balanceInEth} tokens", LogLevel.Debug));

					return balanceInEth;
				}
				catch (Exception ex)
				{
					OnLog?.Invoke(this, new LogEventArgs($"Error while getting balance: {ex.Message}. Retrying {{{i}/{retries}}}", LogLevel.Error));

					if (i == retries)
					{
                        OnLog?.Invoke(this, new LogEventArgs($"Error while getting balance: {ex.Message}. Max retry attempts reached.", LogLevel.Error));
                        return -1;
                    }

                    await Task.Delay(delayInMs);
				}
			}
            return -1;
        }

        public async Task<Dictionary<int, decimal>> GetBalanceAsync()
		{
			Dictionary<int,decimal> balances = new Dictionary<int, decimal>();
			for(int i=0;i<IWalletService.WALLETS_CNT;i++)
			{ 
				var address = _walletService.GetSensorWalletAddress(i);

                balances.Add(i, await GetBalanceAsync(address));
			}
			return balances;
		}
    }
}
