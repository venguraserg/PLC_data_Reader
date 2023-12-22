using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S7.Net;

namespace GoogleSheet
{
    internal class PLC_Reader
    {
        private readonly Plc plc;
        private List<PLC_CellData> dataCells;

        public PLC_Reader(CpuType cpuType, string ipAdress)
        {
            this.plc = new Plc(cpuType, ipAdress, 0, 0);
        }

        public double ReadValueToPLC(PLC_CellData plc_CellData)
        {

            using (var plc_new = this.plc)
            {

                plc.Open();

                var variable = (float)plc.Read(plc_CellData.DataType, plc_CellData.DB_Namber, plc_CellData.DB_Address, plc_CellData.VarType, 1);



                plc.Close();

                return (double)variable;

            }




        }
        public double ReadValueToPLC_new()
        {

            using (var plc_new = this.plc)
            {

                plc.Open();

                var variable = plc.Read("DB1.DBD0");



                plc.Close();

                return (double)variable;

            }




        }
    }
}