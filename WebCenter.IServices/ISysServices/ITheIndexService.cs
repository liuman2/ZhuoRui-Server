using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCenter.IServices
{
    public interface ITheIndexService
    {
        void CreateIndex(Guid id, string table, string content);    //创建索引
        void RemoveIndex(Guid id);     //移除索引
        void OptimizeIndex();   //优化索引
        IEnumerable<Guid> Search(string table, string searchQuery); //搜索
    }
}
