using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Timers;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using GoogleSheet;
using S7.Net;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PLC_Reader_Console
{
    //сслыка на GoogleSheet - https://docs.google.com/spreadsheets/d/1PWxv4H1p-z-LR21uULmE0SJ6EG-WLFmJteXieF6drtg/edit#gid=0
    class Program
    {

        static void Main(string[] args)
        {



            //Подключение к БД
            ConnectDB connect = new("SRV-XAMP\\SQLEXPRESS", "PLC_data", "pcl_user", "pcl_user");

            connect.openConnection();
            string queryString = System.String.Format($"INSERT INTO DataTable (Id, Name) VALUES ( 1,Jonv)");
            SqlCommand command = new SqlCommand(queryString, connect.getConnection());
            command.ExecuteReader();
            connect.closeConnection();


            Console.ReadKey();


            //********************



            System.Timers.Timer timer = new System.Timers.Timer(60000);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            ConsoleKeyInfo response = new ConsoleKeyInfo();

            Console.WriteLine("Приложение запущено");
            Console.WriteLine("Для выхода нажмите Q");
            Console.WriteLine($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");
            do
            {
                response = Console.ReadKey();
                if (response.Key != ConsoleKey.A) ReadWrite();


            } while (response.Key != ConsoleKey.Q);


            Console.WriteLine("Приложение остановлено, нажмите любую кнопку");
            timer.Stop();

            Console.ReadKey();

        }

        private static void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            ReadWrite();

            Console.WriteLine($"{e.SignalTime.Date.ToShortDateString()} {e.SignalTime.Hour}:{e.SignalTime.Minute}:{e.SignalTime.Second}  - Данные записаны");
        }

        public static void ReadWrite()
        {
            var plcReader_20 = new PLC_Reader(CpuType.S7300, "192.168.3.20");

            var plcReader_112 = new PLC_Reader(CpuType.S71200, "192.168.3.112");

            var plcReader_115 = new PLC_Reader(CpuType.S71200, "192.168.3.115");

            List<object> plcData = new List<object>();

            double tempData = 0;
            double comp1Current = (plcReader_20.ReadValueToPLC(new PLC_CellData(DataType.DataBlock, 2, 970, VarType.Real)) * 0.86 * 1.73 * 400) / 1000,
                comp2Current = (plcReader_20.ReadValueToPLC(new PLC_CellData(DataType.DataBlock, 2, 990, VarType.Real)) * 0.86 * 1.73 * 400) / 1000,
                comp3Current = (plcReader_20.ReadValueToPLC(new PLC_CellData(DataType.DataBlock, 2, 1010, VarType.Real)) * 0.86 * 1.73 * 400) / 1000,
                comp4Current = (plcReader_20.ReadValueToPLC(new PLC_CellData(DataType.DataBlock, 2, 1030, VarType.Real)) * 0.86 * 1.73 * 400) / 1000,
                comp5Current = (plcReader_20.ReadValueToPLC(new PLC_CellData(DataType.DataBlock, 22, 190, VarType.Real)) * 0.86 * 1.73 * 400) / 1000;



            //
            //1 столбец - время
            //plcData.Add($"{e.SignalTime.Hour}:{e.SignalTime.Minute}:{e.SignalTime.Second}");
            plcData.Add($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");
            //2 столбец - Обратка ледводы 1            
            plcData.Add(plcReader_112.ReadValueToPLC(new PLC_CellData(DataType.DataBlock, 1, 0, VarType.Real)));
            //3 столбец - Обратка ледводы 2 
            plcData.Add(plcReader_112.ReadValueToPLC(new PLC_CellData(DataType.DataBlock, 1, 4, VarType.Real)));
            //4 столбец - Подача ледводы 
            plcData.Add(plcReader_112.ReadValueToPLC(new PLC_CellData(DataType.DataBlock, 1, 16, VarType.Real)));
            //5 столбец - Давление Ледводы
            plcData.Add(plcReader_112.ReadValueToPLC(new PLC_CellData(DataType.DataBlock, 1, 20, VarType.Real)));
            //6 столбец - Суммарная мощность компрессоров ВСЕХ
            plcData.Add(comp1Current + comp2Current + comp3Current + comp4Current + comp5Current);
            //7 столбец - Суммарная мощность компрессоров 1-4
            plcData.Add(comp1Current + comp2Current + comp3Current + comp4Current);
            //8 столбец - Мощность компрессора 5
            plcData.Add(comp5Current);
            //9 столбец - Давление нагнетания 1
            plcData.Add(plcReader_20.ReadValueToPLC(new PLC_CellData(DataType.DataBlock, 2, 870, VarType.Real)));
            //10 столбец - Давление нагнетания 2 
            plcData.Add(plcReader_20.ReadValueToPLC(new PLC_CellData(DataType.DataBlock, 2, 1110, VarType.Real)));
            //11 столбец - Давление всасывания PV
            plcData.Add(plcReader_20.ReadValueToPLC(new PLC_CellData(DataType.DataBlock, 2, 810, VarType.Real)));
            //12 столбец - Давление всасывания SP
            plcData.Add(plcReader_20.ReadValueToPLC(new PLC_CellData(DataType.DataBlock, 3, 64, VarType.Real)));
            //13 столбец -Температура наружнего воздуха
            plcData.Add(plcReader_115.ReadValueToPLC(new PLC_CellData(DataType.DataBlock, 19, 718, VarType.Real)));


            /*
            for (int i = 0; i < plcData.Count; i++)
            {
                Console.WriteLine(plcData[i]);
            }*/





            //////////////////////////
            var shSender = new GoogleDataSender("1PWxv4H1p-z-LR21uULmE0SJ6EG-WLFmJteXieF6drtg", DateTime.Now.Date.ToShortDateString().ToString());

            shSender.CreateEntry(plcData);
        }




    }
}
