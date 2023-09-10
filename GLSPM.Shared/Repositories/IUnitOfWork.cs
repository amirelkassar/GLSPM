using GLSPM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GLSPM.Domain.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<Card,int> CardsRepository { get; }
        IRepository<Password, int> PasswordsRepository { get; }
        IRepository<ApplicationUser, string> UserssRepository { get; }
        Task<int> CommitAsync();
    }
}
