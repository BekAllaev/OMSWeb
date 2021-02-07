using System;

namespace OMSWeb.Data.Access.DAL
{
    /// <summary>
    /// Transaction unit of work 
    /// </summary>
    public interface ITransaction : IDisposable
    {
        void Commit();

        void RollBack();    
    }
}