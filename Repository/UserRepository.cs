using HotMusic.Contract;
using HotMusic.DataModel;

namespace HotMusic.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MusicDbContext _dbContext;
        public UserRepository(MusicDbContext context)
        {
            _dbContext = context;
        }

        public void Add(Users user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var delObj = _dbContext.Users.Find(id);
            if (delObj != null)
            {
                _dbContext.Users.Remove(delObj);
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<Users> GetAll(string keyword = "")
        {
            var listData = _dbContext.Users.ToList();
            return listData;
        }

        public Users GetById(int id)
        {
            Users user = _dbContext.Users.FirstOrDefault(u => u.UserId == id);
            return user;
        }

        public Users GetByName(string name)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.UserName.Contains(name) || u.FullName.Contains(name));
            return user;
        }

        public void Update(Users user)
        {
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
        }
    }
}
