﻿using API.Contracts;
using API.Domain;
using API.Domain.Database;
using API.Domain.Database.Interfaces;
using API.Repositories;

namespace API.Services
{
    public class RateableService<T> : IRateableService<T> where T : class, IRateable
    {
        private readonly INonQueryRepository<T> _nonQueryRepository;

        public RateableService(INonQueryRepository<T> nonQueryRepository)
        {
            _nonQueryRepository = nonQueryRepository;
        }

        public async Task<Result<T>> UpdateRatingAsync(T item, User user)
        {
            if (item.UsersLiked.Contains(user))
            {
                item.UsersLiked.Remove(user);
            }
            else
            {
                item.UsersLiked.Add(user);
            }

            var updated = await _nonQueryRepository.UpdateAsync(item);
            return new Result<T>
            {
                Success = updated,
                Data = updated ? item : null,
                Errors = updated ? Array.Empty<string>() :
                    new string[] { StaticErrorMessages<T>.DeleteOperationFailed }
            };
        }
    }
}
