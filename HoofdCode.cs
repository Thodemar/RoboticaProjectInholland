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
        static void KillSwitch()
        {
            Environment.Exit(0);
        }

        static Robot Caliberen(Robot legoRobotCal)
        // Calibreert de robot voor de run
        {
            // Prompt om te vragen of door te gaan met caliberen
            void AskToKill()
            {
                LcdConsole.WriteLine("\n\nOmdoor te gaan klik rechter knop, om af te breken klik linker knop\n\n");

                while (legoRobotCal.GetStatusKnop(true) == false)
                {
                    if (legoRobotCal.GetStatusKnop(false))
                    {
                        KillSwitch();
                    }
                }
            }

            //// Test de motors
            //legoRobotCal.DraaienHoeveelheidGraden(-20);
            //legoRobotCal.DraaienHoeveelheidGraden(40);
            //legoRobotCal.Reizen(1000);
            //legoRobotCal.Reizen(1000, false);
            //legoRobotCal.Reizen(3);


            //LcdConsole.WriteLine("Bewegingsmotors getest");

            //AskToKill();

            //Thread.Sleep(2000);



            // calibratie gyro
            while (legoRobotCal.GetStatusKnop(true)==false)
            {
                if (legoRobotCal.GetStatusKnop(false))
                {
                    legoRobotCal.HerstartGyro();
                }
                LcdConsole.WriteLine(Convert.ToString(legoRobotCal.GetGyroWaarde()));
                Thread.Sleep(100);
            }
            //legoRobotCal.HerstartGyro();
            legoRobotCal.ResetHoektellerALLEENVOORCALIBRATIE();
            LcdConsole.WriteLine("Calibratie Gyro geslaagd");

            //Thread.Sleep(2000);

            //AskToKill();

            Thread.Sleep(2000);

            LcdConsole.WriteLine("Calibratie Arm");
            // brengt de arm naar benedene
            while (legoRobotCal.GetStatusKnop(true) == false)
            {
                if (legoRobotCal.GetStatusKnop(false))
                {
                    legoRobotCal.ArmBewegen(-10, 20, true);
                }

                Thread.Sleep(1000);
            }

            legoRobotCal.HerstelTachoTellerArm();
            LcdConsole.WriteLine("Calibratie Arm klaar");

            legoRobotCal.ArmOmhoog();

            return legoRobotCal;

        }



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





            int[] correctie = { 0, //Links
                0 }; // Rechts


            Robot legoRobot = new LegoRobot.Robot(motorLinks, motorRechts, motorArm, gyroSensor, ultraultrasonicSensor, knopLinks, knopRechts, correctie);

            bool killSwitchAan = false;

            void KillSwitchRun()
            {
                while (killSwitchAan)
                {
                    if (legoRobot.GetStatusKnop(false))
                    {
                        legoRobot.KILLMOTORS();
                        KillSwitch();
                    }
                    Thread.Sleep(100);
                }
            }

            Thread.Sleep(1000);
            legoRobot = Caliberen(legoRobot);
            Thread.Sleep(1000);

            void WhinyGyro()
            {
                int i = 400;

                while (true)
                {
                    Thread.Sleep(500);
                    LcdConsole.WriteLine($"{legoRobot.GetGyroWaarde()}");
                    i++;
                    LcdConsole.WriteLine($"{i}");
                }
            }


            void HaaiHalen()
            {
                legoRobot.DraaienNaar(85,20);
                Thread.Sleep(100);
                legoRobot.DraaienNaar(85,20);
                Thread.Sleep(90);

                //legoRobot.DraaienNaar(90);

                legoRobot.Reizen(9);

                legoRobot.DraaienNaar(-1,20);
                Thread.Sleep(100);
                legoRobot.DraaienNaar(-1,20);
                Thread.Sleep(100);




                legoRobot.Reizen(72);
                legoRobot.DraaienNaar(-52);
                legoRobot.Reizen(1);
                legoRobot.ArmOmlaag();
                Thread.Sleep(300);
                legoRobot.ArmOmhoog();
                legoRobot.Reizen(1, false);
                legoRobot.DraaienNaar(0);
                legoRobot.DraaienNaar(0);

                legoRobot.Reizen(39, false);
                legoRobot.DraaienNaar(85);
                legoRobot.DraaienNaar(85);

                //legoRobot.ArmOmlaag();

                //Tegen mast aan
                legoRobot.VoortBewegen(30, 30);

                Thread.Sleep(500);

                legoRobot.Reizen(34, false);

                legoRobot.DraaienNaar(10);

                legoRobot.Reizen(30, false);
                //legoRobot.ArmBewegen(60, 120);

            }


            void DuikbootTillenZijkant()
            {

                legoRobot.ArmBewegen(30, 50);
                legoRobot.Reizen(2);

                Thread.Sleep(2000);

                Thread Optillen = new Thread(() => legoRobot.ArmBewegen(80, 40));

                Optillen.Start();

                legoRobot.VoortBewegen(5, 50);

                Optillen.Join();
            }


            void OctopusRun()
            {
                //legoRobot.Reizen(1);
                legoRobot.DraaienNaar(-45);
                legoRobot.Reizen(40);

                // Octopoes opvangen
                legoRobot.VoortBewegen(4, 30);
                legoRobot.Reizen(3, false);
                legoRobot.DraaienHoeveelheidGraden(-44);
                legoRobot.Reizen(37);

                // draaien naar octo drop
                legoRobot.DraaienNaar(-1);
                legoRobot.Reizen(12);

                // Octo drop
                legoRobot.ArmBewegen(0, 30);
                Thread.Sleep(100);
                legoRobot.ArmOmhoog();
                legoRobot.Reizen(1);

                // draai naar vis
                legoRobot.DraaienNaar(-25);
                legoRobot.Reizen(1);
                legoRobot.DraaienNaar(-53);
                legoRobot.Reizen(30);

                //Draai naar duikboot
                legoRobot.DraaienNaar(40);
                legoRobot.ArmBewegen(20, 40);
                legoRobot.Reizen(30);

                DuikbootTillenZijkant();

                legoRobot.Reizen(10, false);
               
            }

            void HaaiWegBrengen()
            {
                legoRobot.DraaienNaar(60, 20);
                Thread.Sleep(100);
                legoRobot.DraaienNaar(60, 20);
                Thread.Sleep(90);

                //legoRobot.ArmOmlaag();

                legoRobot.Reizen(51);

                legoRobot.ArmOmlaag();

                Thread.Sleep(200);



                legoRobot.ArmOmhoog();







            }


            //legoRobot.Reizen(5);
            Thread killThread = new Thread(KillSwitchRun);

            killSwitchAan = true;

            killThread.Start();


            //HaaiHalen();

            HaaiWegBrengen();

            //OctopusRun();

            //DuikbootTillenZijkant();

            killSwitchAan = false;

            killThread.Join();


            //legoRobot.MOTORTEST(5000, 49, 50);


            Thread.Sleep(1000);


            //legoRobot.ArmBewegen(75, 50);
            //legoRobot.ArmBewegen(0,50);
            //LcdConsole.WriteLine(Convert.ToString(legoRobot.GetUltrasonicSensor()));

            legoRobot.ArmOmlaag();


            Thread.Sleep(3000);

        


        }
    }
}
