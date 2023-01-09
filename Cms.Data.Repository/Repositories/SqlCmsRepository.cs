using Cms.Data.Repository.Models;

namespace Cms.Data.Repository.Repositories{
    public class SqlCmsRepository{//:ICmsRepository{ //burası commentlendi. cunku async eklenince hata veriyor.
        public SqlCmsRepository(){

        }
        public IEnumerable<Course> GetAllCourses(){
            return null;
        }
}
}