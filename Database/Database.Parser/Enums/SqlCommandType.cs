using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Parser.Enums
{
    public enum SqlCommandType
    {
        CreateTable,
        DropTable,
        Insert,
        Select,
        Delete,
        CreateIndex,
        DropIndex
    }
}
