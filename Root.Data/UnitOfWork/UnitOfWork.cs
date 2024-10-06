using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Root.Data.GenericRepository;
using Root.Models.Utils;


namespace Root.Data.UnitOfWork
{
    public class UnitOfWork<TDbContext> : IDisposable where TDbContext : DbContext
    {
        #region Private member variables
        protected DbContext _context = null;
        private bool disposed = false;
        private Dictionary<Type, object> repositories;
        #endregion

        #region Implementing IDiosposable

        /// <summary>
        /// Protected Virtual Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Public Procedure Methods
        public GenericRepository<TDbContext, TDbEnity> Repository<TDbEnity>() where TDbEnity : class
        {
            try
            {
                if (repositories == null)
                {
                    repositories = new Dictionary<Type, object>();
                }

                var type = typeof(TDbEnity);

                if (!repositories.ContainsKey(type))
                {
                    var typeOfRepository = typeof(GenericRepository<,>);
                    //var repository = new GenericRepository<RemoteLog>(TDbContext);  //Activator.CreateInstance(typeOfRepository.MakeGenericType(typeof(T)), _context);
                    object repository = Activator.CreateInstance(typeOfRepository.MakeGenericType(typeof(TDbContext), typeof(TDbEnity)), _context);
                    repositories.Add(type, repository);
                }

                return (GenericRepository<TDbContext, TDbEnity>)repositories[type];
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }

    public class ReadUOM<TDbContext> : UnitOfWork<TDbContext>, IDisposable where TDbContext : DbContext
    {
        public ReadUOM(IConfiguration configuration)
        {
            DbContextOptionsBuilder<TDbContext> options = new DbContextOptionsBuilder<TDbContext>();
            string value = (string)typeof(ProgConstants).GetField(typeof(TDbContext).Name).GetValue(null);

            ConnHelper connHelper = new ConnHelper();
            string connectionString = connHelper.GetDBConn(configuration, value) + ";ApplicationIntent=ReadOnly";
            options.UseSqlServer(connectionString);
            _context = Activator.CreateInstance(typeof(TDbContext), options.Options) as TDbContext;
        }
    }

    public class WriteUOM<TDbContext> : UnitOfWork<TDbContext>, IDisposable where TDbContext : DbContext
    {
        public WriteUOM(IConfiguration configuration)//: base(configuration)
        {
            DbContextOptionsBuilder<TDbContext> options = new DbContextOptionsBuilder<TDbContext>();
            string value = (string)typeof(ProgConstants).GetField(typeof(TDbContext).Name).GetValue(null);

            ConnHelper connHelper = new ConnHelper();
            string connectionString = connHelper.GetDBConn(configuration, value);
            options.UseSqlServer(connectionString);
            _context = Activator.CreateInstance(typeof(TDbContext), options.Options) as TDbContext;
        }

        #region Public member methods...
        /// <summary>
        /// Save method.
        /// </summary>
        public async Task Save()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }

    internal sealed class ConnHelper
    {
        internal string GetDBConn(IConfiguration configuration, string value)
        {
            //string ConnStr = configuration.GetConnectionString(value);
            //string ConnKey = ConnStr.Substring(0, ProgConstants.ConnKeySize);
            //string ConnIV = ConnStr.Substring(ProgConstants.ConnKeySize, ProgConstants.ConnIVSize);
            //ConnStr = ConnStr.Substring(ProgConstants.ConnKeySize + ProgConstants.ConnIVSize, ConnStr.Length - ProgConstants.ConnKeySize - ProgConstants.ConnIVSize);
            //return TripleDES.Decrypt(ConnStr, ConnKey, ConnIV);
            //return "Data Source=WJLP-3429;Initial Catalog=Inventory;Integrated Security=True;Trust Server Certificate=True";
            return "Data Source=tcp:inventorymgmtdb.database.windows.net,1433;Initial Catalog=Inventory;User ID=inventorydb;Password=Admin@1234;TrustServerCertificate=true";
        }
    }
}
