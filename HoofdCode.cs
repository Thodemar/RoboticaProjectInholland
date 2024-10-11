using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;



using MonoBrickFirmware;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Sensors;
using MonoBrickFirmware.UserInput;



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

            // Duk knoppen
            EV3TouchSensor knopRechts = new EV3TouchSensor(SensorPort.In3);
            EV3TouchSensor knopLinks = new EV3TouchSensor(SensorPort.In2);


        //Knopjes
            ButtonEvents buts = new ButtonEvents();

            // Moter voor de arm
            Motor motorArm = new Motor(MotorPort.OutB);

            EV3GyroSensor gyroSensor = new EV3GyroSensor(SensorPort.In4, GyroMode.Angle);
            EV3UltrasonicSensor ultraultrasonicSensor = new EV3UltrasonicSensor(SensorPort.In1, UltraSonicMode.Centimeter);





            int[] correctie = { 2, //Links
                0 }; // Rechts


            Robot legoRobot = new LegoRobot.Robot(motorLinks, motorRechts, motorArm, gyroSensor, ultraultrasonicSensor,knopLinks,knopRechts, correctie);

            legoRobot.ArmBewegen(75,50);
            //LcdConsole.WriteLine(Convert.ToString(legoRobot.GetUltrasonicSensor()));
            Thread.Sleep(3000);









            //legoRobot.Reizen(500,true);


            //legoRobot.DraaienNaar(90);
            //legoRobot.DraaienHoeveelheidGraden(50);
            //Thread.Sleep(3000);
            //legoRobot.Reizen(3600);
            //legoRobot.DraaienNaar(0);
            //legoRobot.Reizen(1000);



            //legoRobot.VoortBewegen(2000, 60, false);

            //legoRobot.DraaienNaar(-100);
            //legoRobot.DraaienNaar(-200);
            //legoRobot.DraaienNaar(0);
            //legoRobot.DraaienNaar(-340);
            //legoRobot.DraaienHoeveelheidGraden(-40);
            //legoRobot.DraaienNaar(90);





            //bool Go= true;

            //buts.EscapePressed += () => { Go = false; };

            // Maakt de robot aan
            //Robot legoRobot = new LegoRobot.Robot(motorLinks, motorRechts, motorArm, gyroSensor, ultraultrasonicSensor, correctie);
            //while (Go)
            //{
            //    //legoRobot.Ultrasonic();

            //    //legoRobot.Armbewegen(20, 10, 10, 10);
            //    //legoRobot.Armbewegen(-20, 10, 10, 10);

            //LcdConsole.WriteLine($"Boe");





            //legoRobot.VoortBewegen(3000, 40, true);
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

            //Thread.Sleep(3000);
            //var gyro = new EV3GyroSensor(SensorPort.In1, GyroMode.Angle);

            //for (int i = 0; i<100;i++)
            //{
            //    LcdConsole.WriteLine($"Gyro sensor: {gyroSensor.Read()}");
            //    LcdConsole.WriteLine($"Gyro sensor: {gyroSensor.ReadAsString()}");
            //Thread.Sleep(100);
            //}

            //}




        }
    }
}
