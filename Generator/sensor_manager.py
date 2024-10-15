from const import * 
from sensor import Sensor
import random
import paho.mqtt.client as mqtt
import threading
import time

class SensorManager:
    def __init__(self, sensor_cnt: int):
        self.sensor_cnt = sensor_cnt
        self.sensors = self._init_sensors()
        self.client = mqtt.Client(client_id="test", userdata=None, protocol=mqtt.MQTTv5)
        self._configure_mqtt()
        self.threads = []  
        self.thread_running = True 


    def _init_sensors(self) -> list[Sensor]:
        sensors:list[Sensor] =[]
        id = 0
        for location in LOCATIONS:
            for type in SensorType:
                    sensors.append(Sensor(id,f"{type.value}{id}",type,location))
                    id+=1
        print("created:")
        for sensor in sensors:
            print(sensor)
        return sensors
    
    def _configure_mqtt(self) -> None:
         self.client.username_pw_set(MQTT_USER, MQTT_PSWD)
         self.client.connect(MQTT_BROKER_ADDR,MQTT_PORT)
         self.client.loop_start()

    def generate(self, sensorid) -> None:
        msg = self.client.publish(MQTT_TOPIC, payload=self.sensors[sensorid].generate(), qos=2)
        msg.wait_for_publish(1)

    def _sensor_thread(self, sensorid):
        while self.thread_running:
            self.generate(sensorid)
            delay = random.uniform(1, 100)  
            time.sleep(delay)  

    def start_sensor_threads(self):
        for i in range(len(self.sensors)):
            thread = threading.Thread(target=self._sensor_thread, args=(i,))
            thread.start()
            self.threads.append(thread)

    def stop_sensor_threads(self):
        self.thread_running = False
        for thread in self.threads:
            thread.join()  
        self.client.loop_stop()  
        self.client.disconnect() 
        print("All sensor threads stopped and MQTT client disconnected.")
         
         
    

    


        