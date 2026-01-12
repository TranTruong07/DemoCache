using DemoForRedis;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DemoForRedis
{
    public class MongoRepository<TMongoModel> 
        where TMongoModel : WeatherForecast
    {
        protected readonly IMongoCollection<TMongoModel> _collection;
        protected readonly IMongoDatabase _database;
        public MongoRepository(IMongoDatabase database)
        {
            _database = database;
            _collection = _database.GetCollection<TMongoModel>(typeof(TMongoModel).Name);
        }

        /*
        Author  : TruongTN
        Created : 7/10/2025
        Updated : 22/10/2025
        Purpose : Thêm mới một bản ghi vào collection
        */
        public async Task AddAsync(TMongoModel model)
        {
            await _collection.InsertOneAsync(model);
        }

        /*
        Author  : TruongTN
        Created : 7/10/2025
        Updated : 22/10/2025
        Purpose : Thêm mới nhiều bản ghi vào collection
        */
        public async Task AddRangeAsync(List<TMongoModel> models)
        {
            await _collection.InsertManyAsync(models);
        }

        /*
        Author  : TruongTN
        Created : 7/10/2025
        Updated : 22/10/2025
        Purpose : Đếm số lượng bản ghi trong collection theo điều kiện đầu vào
        */
        public async Task<long> CountAsync(Expression<Func<TMongoModel, bool>> expression)
        {
            return await _collection.Find(expression).CountDocumentsAsync();
        }

        /*
        Author  : TruongTN
        Created : 7/10/2025
        Updated : 22/10/2025
        Purpose : Xóa một bản ghi trong collection theo ID
        */
        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(model => model.Id == id);
        }

        /*
        Author  : TruongTN
        Created : 7/10/2025
        Updated : 22/10/2025
        Purpose : Xóa nhiều bản ghi trong collection theo điều kiện đầu vào
        */
        public async Task DeleteManyAsync(FilterDefinition<TMongoModel> filter)
        {
            await _collection.DeleteManyAsync(filter);
        }
        /*
        Author  : TruongTN
        Created : 30/10/2025
        Updated : #
        Purpose : Xóa nhiều bản ghi trong collection theo 1 list model đầu vào
        */
        public async Task DeleteManyAsync(IEnumerable<TMongoModel> models)
        {
            var ids = models.Select(x => x.Id).ToList();
            var filter = Builders<TMongoModel>.Filter.In(x => x.Id, ids);
            await _collection.DeleteManyAsync(filter);
        }

        /*
        Author  : TruongTN
        Created : 7/10/2025
        Updated : 22/10/2025
        Purpose : Lấy một bản ghi trong collection theo điều kiện đầu vào
        */
        public async Task<TMongoModel> GetAsync(Expression<Func<TMongoModel, bool>> expression)
        {
            return await _collection.Find(expression).FirstOrDefaultAsync();
        }

        /*
        Author  : TruongTN
        Created : 7/10/2025
        Updated : 22/10/2025
        Purpose : Lấy một bản ghi trong collection theo ID
        */
        public async Task<TMongoModel> GetByIdAsync(string id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        /*
        Author  : TruongTN
        Created : 7/10/2025
        Updated : 22/10/2025
        Purpose : Lấy danh sách bản ghi trong collection theo điều kiện đầu vào
        */
        public async Task<List<TMongoModel>> GetListAsync(Expression<Func<TMongoModel, bool>> expression)
        {
            return await _collection.Find(expression).ToListAsync();
        }

        /*
        Author  : TruongTN
        Created : 7/10/2025
        Updated : 22/10/2025
        Purpose : Lấy danh sách tất cả bản ghi trong collection
        */
        public async Task<List<TMongoModel>> GetListAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }


        ///*
        //Author  : TruongTN
        //Created : 7/10/2025
        //Updated : 22/10/2025
        //Purpose : Update một bản ghi trong collection theo ID với model đầu vào
        //*/
        //public async Task UpdateAsync(TMongoModel model, string id)
        //{
        //    model.UpdatedAt = DateTime.UtcNow;
        //    var listUpdate = ParseObject.ToDictionary(model);

        //    // là helper của MongoDB tạo lệnh update
        //    var update = Builders<TMongoModel>.Update;
        //    var updateDefinition = new List<UpdateDefinition<TMongoModel>>();
        //    foreach (var item in listUpdate)
        //    {
        //        updateDefinition.Add(update.Set(item.Key, item.Value));
        //    }
        //    var filter = Builders<TMongoModel>.Filter.Eq(x => x.Id, id);
        //    await _collection.UpdateOneAsync(filter, update.Combine(updateDefinition));
        //}

    }
}
