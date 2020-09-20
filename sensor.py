import sys
import subprocess
import time
from datetime import datetime
import serial
from prometheus_client import CollectorRegistry, Gauge, write_to_textfile

########################################
# Prepare prometheus_client
########################################

lib = '/var/lib/prometheus/node-exporter'

registry = CollectorRegistry()

temperature = Gauge('sensor_omron_temperature', 'Temperature', registry=registry)
humidity = Gauge('sensor_omron_humidity', 'Humidity', registry=registry)
light = Gauge('sensor_omron_light', 'Ambient light', registry=registry)
barometric = Gauge('sensor_omron_barometric', 'Barometric pressure', registry=registry)
noise = Gauge('sensor_omron_noise', 'Sound noise', registry=registry)

discomfort = Gauge('sensor_omron_discomfort', 'Discomfort index', registry=registry)
heat = Gauge('sensor_omron_heat', 'Heat stroke', registry=registry)

etvoc = Gauge('sensor_omron_etvoc', 'eTVOC', registry=registry)
eco2 = Gauge('sensor_omron_eco2', 'eCO2', registry=registry)

seismic = Gauge('sensor_omron_seismic', 'Seismic intensity', registry=registry)
vibration = Gauge('sensor_omron_vibration', 'Vibration information', registry=registry)
si = Gauge('sensor_omron_si', 'SI value', registry=registry)
pga = Gauge('sensor_omron_pga', 'PGA', registry=registry)


########################################
# Sensor
########################################

def calc_crc(buf, length):
    crc = 0xFFFF
    for i in range(length):
        crc = crc ^ buf[i]
        for i in range(8):
            carrayFlag = crc & 1
            crc = crc >> 1
            if (carrayFlag == 1):
                crc = crc ^ 0xA001
    crcH = crc >> 8
    crcL = crc & 0x00FF
    return (bytearray([crcL, crcH]))

def write_data(data):
    temperature.set(int(hex(data[9]) + '{:02x}'.format(data[8], 'x'), 16) / 100)
    humidity.set(int(hex(data[11]) + '{:02x}'.format(data[10], 'x'), 16) / 100)
    light.set(int(hex(data[13]) + '{:02x}'.format(data[12], 'x'), 16))
    barometric.set(int(hex(data[17]) + '{:02x}'.format(data[16], 'x') + '{:02x}'.format(data[15], 'x') + '{:02x}'.format(data[14], 'x'), 16) / 1000)
    noise.set(int(hex(data[19]) + '{:02x}'.format(data[18], 'x'), 16) / 100)

    discomfort.set(int(hex(data[25]) + '{:02x}'.format(data[24], 'x'), 16) / 100)
    heat.set(int(hex(data[27]) + '{:02x}'.format(data[26], 'x'), 16) / 100)

    etvoc.set(int(hex(data[21]) + '{:02x}'.format(data[20], 'x'), 16))
    eco2.set(int(hex(data[23]) + '{:02x}'.format(data[22], 'x'), 16))

    seismic.set(int(hex(data[34]) + '{:02x}'.format(data[33], 'x'), 16) / 1000)
    vibration.set(int(hex(data[28]), 16))
    si.set(int(hex(data[30]) + '{:02x}'.format(data[29], 'x'), 16) / 10)
    pga.set(int(hex(data[32]) + '{:02x}'.format(data[31], 'x'), 16) / 10)

    write_to_textfile(lib + '/sensor_omron.prom', registry)


########################################
# Main
########################################

if __name__ == '__main__':

    while True:
        try:
            ser = serial.Serial('/dev/ttyUSB0', 115200, serial.EIGHTBITS, serial.PARITY_NONE)

            try:
                while True:
                    command = bytearray([0x52, 0x42, 0x05, 0x00, 0x01, 0x21, 0x50])
                    command = command + calc_crc(command, len(command))
                    tmp = ser.write(command)
                    time.sleep(0.1)
                    data = ser.read(ser.inWaiting())

                    if len(data) > 55:
                        write_data(data)

                    time.sleep(5)

            except serial.serialutil.SerialException as e:
                print(e)
                ser.close()

        except (FileNotFoundError, serial.serialutil.SerialException) as e:
            print(e)

        time.sleep(5)
