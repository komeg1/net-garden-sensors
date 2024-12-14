from enum import Enum
#Mqtt broker credentials
MQTT_BROKER_ADDR = 'broker.hivemq.com'
MQTT_USER = 'test@test.com'
MQTT_PSWD = 'test'
MQTT_PORT = 1883
MQTT_TOPIC = "my/net/topic/835-435"
#Sensors config
class SensorType(Enum):
    TEMPERATURE = "Temperature"
    HUMIDITY = "Humidity"
    WIND = "Wind"
    SUN = "Sun"

UNITS = {
    SensorType.TEMPERATURE: "C",
    SensorType.HUMIDITY: "g/m3",
    SensorType.WIND: "km/h",
    SensorType.SUN: "W/m2"
}

SENSORS_CNT = 4

SENSOR_DATA_RANGE={
    SensorType.TEMPERATURE: (-10, 40),
    SensorType.HUMIDITY: (0, 30),
    SensorType.WIND: (0, 100),
    SensorType.SUN: (0, 1000)
}

LOCATIONS=["G1","G2","G3","G4"]