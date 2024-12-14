# Garden sensors simulator system

## Description

## Installation

### Requirements
- Python 3.11 or higher
- .NET 7.0
- Docker
- MongoDB

### Installation

1. Clone the repository
2. Install the requirements

### Configuration

#### Database

1. Start the MongoDB server
2. Create a database for the sensor data
3. Create a collection for the sensor data

#### MQTT broker
Until Eclipse Mosquitto is not available, you can use HiveMQ as a broker. You can use the public broker at `broker.hivemq.com` on port `1883`.

You can use `https://www.hivemq.com/demos/websocket-client/` for testing. 
1. Click on the `Connect` button
2. All the settings can be left as default, you don't need to set `Username` and `Password`
3. In the `Publish` section, set the `Topic` to `my/net/topic/835-435`
4. Set `QoS` to 2

Example of a message:
```json
{
    "sensorId": "2",
    "value": 49.88,
    "location": "G1",
    "unit": "km/h",
    "timestamp": "2024-10-10T11:32:46.551051"
 }
```

#### .NET app settings

1. Open the `Api` directory
2. Open the `appsettings.json` (or `appsettings.Development.json` if not present) file

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "MqttSettings": {
    "Address": "broker.hivemq.com",
    "Port": 1883,
    "ClientId": "clientId-84kS7TIxcb",
    "Topic": "my/net/topic/835-435"
  },
  "Database": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "SensorsData",
    "SensorsCollectionName": "Sensors"
  }
}
```

1. `MqttSettings`:
    - `Address`: The address of the MQTT broker
    - `Port`: The port of the MQTT broker
    - `ClientId`: The client ID of the MQTT client (whatever value you want)
    - `Topic`: The topic to subscribe

2. `Database`:
    - `ConnectionString`: The connection string to the MongoDB database
    - `DatabaseName`: The name of the database
    - `SensorsCollectionName`: The name of the collection


#### Generator

1. Open the `Generator` directory
2. Create virtual environment
```powershell
pip -m venv venv
```
3. Activate the virtual environment
```powershell
.\venv\Scripts\Activate
```
4. Install the requirements
```powershell
pip install -r requirements.txt
```
5. To change broker settings, open `const.py` file and change the values of:
```python
MQTT_BROKER_ADDR = 'broker.hivemq.com'
MQTT_USER = 'test@test.com'
MQTT_PSWD = 'test'
MQTT_PORT = 1883
MQTT_TOPIC = "my/net/topic/835-435"
```

### Usage

If docker is not installed, you can run the API and the generator manually.

1. Ensure that the MongoDB server is running
2. Open the `Api` directory
3. Run the API, it may take while to start
```powershell
dotnet run
```
Example output when the API has started succesfully:
```powershell
[WARNING][12/14/2024 12:02:45 PM]       SensorDataService: connecting to db
[DEBUG][12/14/2024 12:02:45 PM]         SensorDataService: Collection: Sensors
[SUCCESS][12/14/2024 12:02:45 PM]       SensorDataService: connected to SensorsData db
[SUCCESS][12/14/2024 12:02:50 PM]       MqttService: connected
[SUCCESS][12/14/2024 12:02:50 PM]       MqttService: connected to broker.hivemq.com:1883, clientId: clientId-xxx, topic: my/net/topic/835-435
```
4. Open the `Generator` directory
5. Run the generator
```powershell
python garden_sensors_data_generator.py
```
6. The generator will start sending data to the specified MQTT broker

If everything is set up correctly, you should be able to see the data in logs of the API.
Example output:
```powershell
[DEBUG][12/14/2024 12:03:10 PM] MqttService: Received message: {
  "sensorId": "2",
  "value": 49.88,
  "location": "G1",
  "unit": "km/h",
  "timestamp": "2024-12-14T12:47:46.551051"
}
[DEBUG][12/14/2024 12:03:10 PM] SensorDataService: Successfully added to db
```


### SSE

To test the SSE endpoint, you can start live server in the `Api/Test` directory and open the `index.html` file (it's address has to be `http://127.0.0.1:5500`). You should be able to see the data being updated in real time.




