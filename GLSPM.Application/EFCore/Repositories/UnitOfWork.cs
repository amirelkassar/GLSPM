using GLSPM.Domain.Entities;
using GLSPM.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Application.EFCore.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GLSPMDBContext _dbContext;
        private IRepository<Card, int> _cardsrepo;
        private IRepository<Password, int> _passwordsrepo;
        private IRepository<ApplicationUser, string> _appusersrepo;

        public UnitOfWork(GLSPMDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IRepository<Card, int> CardsRepository
        {
            get
            {
                _cardsrepo ??= new BaseRepository<Card, int>(_dbContext);
                return _cardsrepo;
            }
        }

        public IRepository<Password, int> PasswordsRepository
        {
            get
            {
                _passwordsrepo ??= new BaseRepository<Password, int>(_dbContext);
                return _passwordsrepo;
            }
        }

        public IRepository<ApplicationUser, string> UserssRepository
        {
            get
            {
                _appusersrepo ??= new BaseRepository<ApplicationUser, string>(_dbContext);
                return _appusersrepo;
            }
        }

        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
