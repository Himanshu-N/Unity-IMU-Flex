#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"

// Define flex sensor pins
const int flexPinB = 36;
const int flexPinR = 39;
const int flexPinY = 34;

// Flex sensor ranges for mapping
const int minSensorValueB = 1800;
const int maxSensorValueB = 2800;
const int minSensorValueR = 1400;
const int maxSensorValueR = 2800;
const int minSensorValueY = 2200;
const int maxSensorValueY = 3400;

MPU6050 mpu;

#define INTERRUPT_PIN 2
bool dmpReady = false; // Will be true if MPU is successfully initialized
uint8_t mpuIntStatus;
uint8_t devStatus;
uint16_t packetSize;
uint16_t fifoCount;
uint8_t fifoBuffer[64];
Quaternion q;

// Default quaternion values when MPU is not connected
float defaultQuat[4] = {1.0, 0.0, 0.0, 0.0};

volatile bool mpuInterrupt = false;
void dmpDataReady() {
    mpuInterrupt = true;
}

void setup() {
    // Initialize flex sensor pins
    pinMode(flexPinB, INPUT);
    pinMode(flexPinR, INPUT);
    pinMode(flexPinY, INPUT);

    // Initialize I2C
    Wire.begin();
    Wire.setClock(400000);

    // Initialize serial communication
    Serial.begin(9600);
    while (!Serial);

    // Initialize MPU6050
    Serial.println("Initializing I2C devices...");
    mpu.initialize();
    pinMode(INTERRUPT_PIN, INPUT);

    Serial.println("Testing device connections...");
    if (mpu.testConnection()) {
        Serial.println("MPU6050 connection successful");

        Serial.println("Initializing DMP...");
        devStatus = mpu.dmpInitialize();
        mpu.setXGyroOffset(220);
        mpu.setYGyroOffset(76);
        mpu.setZGyroOffset(-85);
        mpu.setZAccelOffset(1788);

        if (devStatus == 0) {
            mpu.CalibrateAccel(6);
            mpu.CalibrateGyro(6);
            mpu.PrintActiveOffsets();
            Serial.println("Enabling DMP...");
            mpu.setDMPEnabled(true);

            attachInterrupt(digitalPinToInterrupt(INTERRUPT_PIN), dmpDataReady, RISING);
            mpuIntStatus = mpu.getIntStatus();
            Serial.println("DMP ready! Waiting for first interrupt...");
            dmpReady = true;

            packetSize = mpu.dmpGetFIFOPacketSize();
        } else {
            Serial.print("DMP Initialization failed (code ");
            Serial.print(devStatus);
            Serial.println(")");
            dmpReady = false; // Indicate failure
        }
    } else {
        Serial.println("MPU6050 connection failed");
        dmpReady = false; // MPU not connected
    }
}

void loop() {
    // Read flex sensor values
    int sensorValueB = analogRead(flexPinB);
    int sensorValueR = analogRead(flexPinR);
    int sensorValueY = analogRead(flexPinY);

    // Map the flex sensor values to 0.0 - 1.0 range
    float mappedValueB = (float)(sensorValueB - minSensorValueB) / (maxSensorValueB - minSensorValueB);
    float mappedValueR = (float)(sensorValueR - minSensorValueR) / (maxSensorValueR - minSensorValueR);
    float mappedValueY = (float)(sensorValueY - minSensorValueY) / (maxSensorValueY - minSensorValueY);

    // Constrain the values between 0.0 and 1.0
    mappedValueB = constrain(mappedValueB, 0.00, 1.00);
    mappedValueR = constrain(mappedValueR, 0.00, 1.00);
    mappedValueY = constrain(mappedValueY, 0.00, 1.00);

    // If MPU is ready and we have data, use real quaternion values
    if (dmpReady && mpu.dmpGetCurrentFIFOPacket(fifoBuffer)) {
        mpu.dmpGetQuaternion(&q, fifoBuffer);

        // Print flex sensor values and real quaternion values
        Serial.print(mappedValueB);
        Serial.print(",");
        Serial.print(mappedValueR);
        Serial.print(",");
        Serial.print(mappedValueY);
        Serial.print(",");
        Serial.print(q.w);
        Serial.print(",");
        Serial.print(q.x);
        Serial.print(",");
        Serial.print(q.y);
        Serial.print(",");
        Serial.println(q.z);

    } else {
        // MPU not connected or DMP failed, print default quaternion values with flex sensor values
        Serial.print(mappedValueB);
        Serial.print(",");
        Serial.print(mappedValueR);
        Serial.print(",");
        Serial.print(mappedValueY);
        Serial.print(",");
        Serial.print(defaultQuat[0]);
        Serial.print(",");
        Serial.print(defaultQuat[1]);
        Serial.print(",");
        Serial.print(defaultQuat[2]);
        Serial.print(",");
        Serial.println(defaultQuat[3]);
    }

    delay(100);  // small delay for stability
}
