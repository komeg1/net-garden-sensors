pragma solidity ^0.8.0;

interface ERC20Interface {
	function totalSupply() external view returns (uint);
	function balanceOf(address tokenOwner) external view returns (uint balance);
	function allowance(address tokenOwner, address spender) external view returns (uint remaining);
	function transfer(address to, uint tokens) external returns (bool success);
	function approve(address spender, uint tokens) external returns (bool success);
	function transferFrom(address from, address to, uint tokens) external returns (bool success);

	event Transfer(address indexed from, address indexed to, uint tokens);
	event Approval(address indexed tokenOwner, address indexed spender, uint tokens);
}

contract SensorRewardToken is ERC20Interface {
	string public symbol;
	string public  name;
	uint8 public decimals;
	uint public _totalSupply;

	mapping(address => uint) balances;
	mapping(address => mapping(address => uint)) allowed;

	constructor() {
		symbol = "GSRT";
		name = "Garden Sensor Reward Token";
		decimals = 18;
		_totalSupply = 1_000_001_000_000_000_000_000_000_000;
		balances[msg.sender] = _totalSupply;
		emit Transfer(address(0), msg.sender, _totalSupply);
	}

	function totalSupply() public view override returns (uint) {
		return _totalSupply;
	}

	function balanceOf(address tokenOwner) public view override returns (uint balance) {
		return balances[tokenOwner];
	}

	function transfer(address to, uint tokens) public override returns (bool success) {
		balances[msg.sender] -= tokens;
		balances[to] += tokens;
		emit Transfer(msg.sender, to, tokens);
		return true;
	}

	function approve(address spender, uint tokens) public override returns (bool success) {
		allowed[msg.sender][spender] = tokens;
		emit Approval(msg.sender, spender, tokens);
		return true;
	}

	function transferFrom(address from, address to, uint tokens) public override returns (bool success) {
		balances[from] -= tokens;
		allowed[from][msg.sender] -= tokens;
		balances[to] += tokens;
		emit Transfer(from, to, tokens);
		return true;
	}

	function allowance(address tokenOwner, address spender) public view override returns (uint remaining) {
		return allowed[tokenOwner][spender];
	}

}