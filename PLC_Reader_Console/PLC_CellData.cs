using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleSheet
{
    internal class PLC_CellData
    {
        public DataType DataType { get; set; }
        public int DB_Namber { get; set; }
        public int DB_Address { get; set; }
        public VarType VarType { get; set; }
        public PLC_CellData(DataType dataType, int DB_Namber, int DB_Address, VarType varType)
        {
            this.DataType = dataType;
            this.DB_Namber = DB_Namber;
            this.DB_Address = DB_Address;
            this.VarType = varType;

        }
    }
}