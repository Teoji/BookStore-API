﻿using BookStore_API.Contracts;
using BookStore_API.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore_API.Services
{
    public class AuthorRespository : IAuthorRepository
    {
        private  ApplicationDbContext _db  { get; set; }

        public AuthorRespository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(Author entity)
        {
            await _db.Authors.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(Author entity)
        {
            _db.Authors.Remove(entity);
            return await Save();
        }

        public async Task<IList<Author>> FindAll()
        {
            var authors = await _db.Authors.ToListAsync();
            return authors;
        }

        public async Task<Author> FindById(int id)
        {
            var author = await _db.Authors.FindAsync(id);
            return author;
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(Author entity)
        {
             _db.Authors.Update(entity);
            return await Save();
        }

        public async Task<bool> isExists(int Id)
        {
            return await _db.Authors.AnyAsync(a =>a.Id == Id);
        }
    }
}
