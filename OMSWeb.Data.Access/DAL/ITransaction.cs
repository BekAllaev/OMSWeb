using System;

namespace OMSWeb.Data.Access.DAL
{
    public interface ITransaction : IDisposable
    {
        void Commit();

        void RollBack();    
    }
}