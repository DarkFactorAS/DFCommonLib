using System;
using System.Data;

namespace DFCommonLib.DataAccess
{
    public interface IDBPatcher
    {
        void Init();
        bool Patch(string system, int patchId, string sql);
        bool Successful();
    }
}