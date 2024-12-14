from const import UNITS,SensorType,SENSOR_DATA_RANGE
import random
import json
from datetime import datetime

class Sensor:
    def __init__(self, id: int, name: str, type: SensorType, location: str):
        self.id = id
        self.name = name
        self.type = type
        self.location = location
        self.unit = UNITS[type]
        self.range = SENSOR_DATA_RANGE[type]

    def generate(self) -> str:
        sensor_value = round(random.uniform(SENSOR_DATA_RANGE[self.type][0], SENSOR_DATA_RANGE[self.type][1]),2)
        sensor_data = {
            "sensorId": str(self.id),
            "value": sensor_value,
            "location": self.location,
            "unit": self.unit,
            "timestamp": datetime.now().isoformat()
        }
        print(json.dumps(sensor_data))
        return json.dumps(sensor_data)
    
    def __str__(self):
     return f"ID: {self.id}, name: {self.name}, location: {self.location}, unit: {self.unit}, range: {self.range}, type: {self.type}"