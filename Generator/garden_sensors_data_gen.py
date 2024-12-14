from const import MQTT_BROKER_ADDR,SENSORS_CNT
from sensor_manager import SensorManager
import threading
import keyboard

def main()->None:
    sensor_manager = SensorManager(SENSORS_CNT)
    sensor_manager.start_sensor_threads()

    input_thread = threading.Thread(target=listen_for_stop,args=(sensor_manager,))
    input_thread.start()
    
    input_thread.join()  

def listen_for_stop(sensor_manager:SensorManager):
    while True:
        if keyboard.is_pressed('q'):  
            print("Stopping sensor threads...")
            sensor_manager.stop_sensor_threads()
            break  

if __name__ == "__main__":
    main()