﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBSector.DataBase.Interfaces
{
    public interface ISendData
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string data);
    }
}
