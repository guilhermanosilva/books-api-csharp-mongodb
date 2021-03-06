using System.Collections.Generic;
using BooksApiMongoDb.Data.Configuration;
using BooksApiMongoDb.Entities;
using MongoDB.Driver;

namespace BooksApiMongoDb.Repositories
{
    public class MongoRepository : IMongoRepository
    {
        private readonly IMongoCollection<Book> _books;

        public MongoRepository(IBooksDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _books = database.GetCollection<Book>(settings.BooksCollectionName);
        }


        public List<Book> Get()
        {
            return _books.Find(book => true).ToList();
        }

        public Book Get(string id)
        {
            return _books.Find<Book>(book => book.Id == id).FirstOrDefault();
        }
        public Book Create(Book book)
        {
            _books.InsertOne(book);
            return book;
        }

        public void Update(string id, Book book)
        {
            _books.ReplaceOne(book => book.Id == id, book);
            
        }

        public void UpdatePrice(string id, decimal price)
        {
            _books.FindOneAndUpdate(book => book.Id == id, 
                Builders<Book>.Update.Set(book => book.Price, price));
        }

        public void Remove(Book book)
        {
            _books.DeleteOne(b => b.Id == book.Id);
        }

        public void Remove(string id)
        {
            _books.DeleteOne(b => b.Id == id);
        }
    }
}