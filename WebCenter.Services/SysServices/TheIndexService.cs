using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using WebCenter.IServices;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace WebCenter.Services.SysServices
{
    public class TheIndexService : ITheIndexService
    {
        private readonly StandardAnalyzer _analyzer;

        private readonly DirectoryInfo _path; //索引库存放在这个文件夹里 
        private readonly Version _version;

        public TheIndexService()
        {
            _version = Version.LUCENE_30;
            _analyzer = new StandardAnalyzer(_version);
            _path =
                new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["pathIndex"]);
        }

        public void CreateIndex(Guid id, string table, string content)
        {
            //Directory表示索引文件保存的地方，是抽象类，两个子类FSDirectory表示文件中，RAMDirectory 表示存储在内存中  
            var directory = FSDirectory.Open(_path);

            //第三个参数为是否创建索引文件夹,Bool Create,如果为True，则新创建的索引会覆盖掉原来的索引文件，反之，则不必创建,更新即可。  
            using (
                var write = new IndexWriter(directory, _analyzer, !IndexReader.IndexExists(directory),
                                            IndexWriter.MaxFieldLength.UNLIMITED))
            {
                write.DeleteDocuments(new Term("id", id.ToString()));

                var document = new Document();
                document.Add(new Field("id", id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                document.Add(new Field("table", table, Field.Store.YES, Field.Index.ANALYZED));
                document.Add(new Field("content", content, Field.Store.YES, Field.Index.ANALYZED,
                                       Field.TermVector.WITH_POSITIONS_OFFSETS));
                write.AddDocument(document);
               
            }
        }

        public void RemoveIndex(Guid id)
        {
            var directory = FSDirectory.Open(_path);

            using (
                var write = new IndexWriter(directory, _analyzer, !IndexReader.IndexExists(directory),
                                            IndexWriter.MaxFieldLength.UNLIMITED))
            {
                write.DeleteDocuments(new Term("id", id.ToString()));
            }
        }

        public void OptimizeIndex()
        {
            var directory = FSDirectory.Open(_path);

            using (var write = new IndexWriter(directory, _analyzer, !IndexReader.IndexExists(directory), IndexWriter.MaxFieldLength.UNLIMITED))
            {
                write.Optimize();
            }
        }

        public IEnumerable<Guid> Search(string table, string searchQuery)
        {
            if (searchQuery.StartsWith("*"))
                searchQuery = searchQuery.Remove(0, 1);

            if (searchQuery.StartsWith("?"))
                searchQuery = searchQuery.Remove(0, 1);

            const int hitsLimit = 1000;

            //索引库存放在这个文件夹里 

            //Directory表示索引文件保存的地方，是抽象类，两个子类FSDirectory表示文件中，RAMDirectory 表示存储在内存中  
            var directory = FSDirectory.Open(_path);

            using (var searcher = new IndexSearcher(directory)) // Build IndexSearch
            {
                //定义多条件搜索分析器
                var bquery = new BooleanQuery();




                var querys = new[] { table, searchQuery }; // 把搜索条件放到数组里

                var fields = new[] { "table", "content" }; //相对应的索引

                var flags = new[] { Occur.MUST, Occur.MUST };

                //多条件搜索拆分器
                var query = MultiFieldQueryParser.Parse(_version, querys, fields, flags,
                                                          _analyzer);
                bquery.Add(query, Occur.MUST);

                var hits = searcher.Search(bquery, hitsLimit).ScoreDocs;
                var results = _mapLuceneToDataList(hits, searcher);

                return results;
            }
        }

        private static IEnumerable<Guid> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits, Searchable searcher)
        {
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
        }

        private static Guid _mapLuceneDocumentToData(Document doc)
        {
            return Guid.Parse(doc.Get("id"));
        }
    }
}