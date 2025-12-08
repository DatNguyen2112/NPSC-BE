using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace NSPC.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDatabaseFactory _databaseFactory;
        private DbContext _dataContext;
        private bool _disposed;
        public List<IRepositoryBase> ListRepository = new List<IRepositoryBase>();

        public UnitOfWork(IDatabaseFactory databaseFactory)
        {
            _databaseFactory = databaseFactory;
            _dataContext = _databaseFactory.GetDbContext();
        }

        public DbContext DataContext => _dataContext ?? (_dataContext = _databaseFactory.GetDbContext());

        public IRepository<T> GetRepository<T>() where T : class
        {
            var repository = new Repository<T>(_dataContext);
            ListRepository.Add(repository);
            return repository;
        }

        public int CountByCommand(string rawSQL, List<SqlParameter> listParameter)
        {
            int result = 0;
            using (var command = DataContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = rawSQL;
                command.Parameters.AddRange(listParameter.ToArray());
                DataContext.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }
            return result;
        }

        public List<dynamic> SelectByCommand(string rawSQL, List<SqlParameter> listParameter)
        {
            List<dynamic> items = new List<dynamic>();
            using (var command = DataContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = rawSQL;
                command.Parameters.AddRange(listParameter.ToArray());
                DataContext.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new ExpandoObject() as IDictionary<string, Object>; ;

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            item[reader.GetName(i)] = reader[i];
                        }
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public void Migrate()
        {
            DataContext.Database.Migrate();
        }

        public bool EnsureCreated()
        {
            return DataContext.Database.EnsureCreated();
        }

        public Task MigrateAsync()
        {
            return DataContext.Database.MigrateAsync();
        }

        public Task<bool> EnsureCreatedAsync()
        {
            return DataContext.Database.EnsureCreatedAsync();
        }

        public int Save()
        {
            var listTask = new List<Task>();
            foreach (var repository in ListRepository)
            {
                listTask.AddRange(repository.GetAllTask());
            }
            if (listTask.Count > 0)
            {
                Task.WaitAll(listTask.ToArray());
            }
            return DataContext.SaveChanges();
        }

        public bool CheckConnection()
        {
            return DataContext.Database.GetDbConnection().State == System.Data.ConnectionState.Connecting;
        }

        public async Task<int> SaveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var listTask = new List<Task>();
            foreach (var repository in ListRepository)
            {
                listTask.AddRange(repository.GetAllTask());
            }
            if (listTask.Count > 0)
            {
                await Task.WhenAll(listTask);
            }
            return await DataContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                {
                    _dataContext.Dispose();
                    _disposed = true;
                }

            _disposed = false;
        }

        public DbContext GetDbContext()
        {
            return _dataContext;
        }
    }
}