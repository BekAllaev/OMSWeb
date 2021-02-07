using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSWeb.Data.Access.DAL
{
    public class EFUnitOfWork : IUnitOfWork
    {
        #region Declarations
        private DbContext _context;
        #endregion

        #region Constructor
        public EFUnitOfWork(DbContext dbContext)
        {
            _context = dbContext;
        }
        #endregion

        #region Properties
        public DbContext Context => _context;
        #endregion

        #region IUnitOfWork implementation
        public void Add<T>(T obj) where T : class
        {
            var set = _context.Set<T>();
            set.Add(obj);
        }

        public void Attach<T>(T obj) where T : class
        {
            var set = _context.Set<T>();
            set.Attach(obj);
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Snapshot)
        {
            return new DbTransaction(_context.Database.BeginTransaction(isolationLevel));
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Delete<T>(T obj) where T : class
        {
            var set = _context.Set<T>();
            set.Remove(obj);
        }

        public void Dispose()
        {
            _context = null;
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return _context.Set<T>();
        }

        public void Update<T>(T obj) where T : class
        {
            var set = _context.Set<T>();
            set.Update(obj);
        }
        #endregion
    }
}
