using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;



using MonoBrickFirmware;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Sensors;


using LegoRobot;



namespace RoboticaProject
{
    

    class MainClass
    {
        static void Main(string[] args)
        {
            // Moters voor het aandrijven van de robot
            Motor motorLinks = new Motor(MotorPort.OutA);
            Motor motorRechts = new Motor(MotorPort.OutD);
            //MotorPort motorLinks = new MotorPort.OutA;
            //MotorPort motorRechts = new MotorPort.OutD;

            // Moter voor de arm
            Motor motorArm = new Motor(MotorPort.OutB);

            EV3GyroSensor gyroSensor = new EV3GyroSensor(SensorPort.In1, GyroMode.Angle);


            // Maakt de robot aan
            Robot legoRobot = new LegoRobot.Robot(motorLinks, motorRechts, motorArm, gyroSensor);

            legoRobot.VoortBewegen(10000, 40, true);
            //legoRobot.DraaienHoeveelheidGraden(90);

            //legoRobot.VoortBewegen(3000, 40, true);
            //legoRobot.DraaienHoeveelheidGraden(90);

            //legoRobot.VoortBewegen(3000, 40, true);
            //legoRobot.DraaienHoeveelheidGraden(90);

            //legoRobot.VoortBewegen(3000, 40, true);
            //legoRobot.DraaienHoeveelheidGraden(90);

            //legoRobot.VoortBewegen(3000, 40, false);
            //legoRobot.DraaienHoeveelheidGraden(90);

            //legoRobot.DraaienNaar(300);
            //legoRobot.DraaienHoeveelheidGraden(20);

            Thread.Sleep(3000);
            //var gyro = new EV3GyroSensor(SensorPort.In1, GyroMode.Angle);

            //for (int i = 0; i<100;i++)
            //{
            //    LcdConsole.WriteLine($"Gyro sensor: {gyroSensor.Read()}");
            //    LcdConsole.WriteLine($"Gyro sensor: {gyroSensor.ReadAsString()}");
            //    Thread.Sleep(100);
            //}





        }




    }
}
