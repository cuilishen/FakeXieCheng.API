﻿using FakeXieCheng.API.Database;
using FakeXieCheng.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXieCheng.API.Services
{
    public class TouristRouteRepository : ITouristRouteRepository
    {
        readonly AppDbContext _context;
        public TouristRouteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TouristRoutePicture> GetPictureAsync(int pictureId)
        {
            return await _context.TouristRoutePictures.Where(item=>item.Id==pictureId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId)
        {
            return await (_context.TouristRoutePictures.Where(p => p.TouristRouteId == touristRouteId)).ToListAsync();
        }

        public async Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId)
        {
           return await _context.TouristRoutes.Include(t=>t.TouristRoutePictures).FirstOrDefaultAsync(n=>n.Id==touristRouteId);
        }

        public async Task<IEnumerable<TouristRoute>> GetTouristRoutesAsync(string keyword,string operatorType,int? rating)
        {
            IQueryable<TouristRoute> result = _context.TouristRoutes.Include(t => t.TouristRoutePictures);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                result = result.Where(t=>t.Title.Contains(keyword));
            }
            if (rating>0)
            {
                result = operatorType switch
                {
                    "largerThan"=>result.Where(t=>t.Rating>=rating),
                    "lessThan"=>result.Where(t=>t.Rating<=rating),
                    _=>result.Where(t=>t.Rating==rating),
                };
            }
            return await result.ToListAsync();
        }
        public async Task<IEnumerable<TouristRoute>> GetTouristRouteByIDListAsync(IEnumerable<Guid> ids)
        {
            return await _context.TouristRoutes.Where(t=>ids.Contains(t.Id)).ToListAsync();
        }
        public async Task<bool> TouristRouteExistsAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.AnyAsync(t=>t.Id==touristRouteId);
        }
        public void AddTouristRoute(TouristRoute touristRoute)
        {
            if (touristRoute==null)
            {
                throw new ArgumentNullException(nameof(touristRoute));
            }
            _context.TouristRoutes.Add(touristRoute);
            //_context.SaveChanges();
            //Save();
        }
        public void DeleteTouristRoute(TouristRoute touristRoute)
        {
            _context.TouristRoutes.Remove(touristRoute);
        }
        public void DeleteTouristRoutePicture(TouristRoutePicture picture)
        {
            _context.TouristRoutePictures.Remove(picture);
        }
        public async Task<bool> SaveAsync()
        {
            return (await _context.SaveChangesAsync()>=0 );
        }
        public void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes)
        {
            _context.TouristRoutes.RemoveRange(touristRoutes);
        }
        public void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture)
        {
            if (touristRouteId==Guid.Empty)
            {
                throw new ArgumentNullException( nameof(touristRouteId));

            }
            if (touristRoutePicture==null)
            {

                throw new ArgumentNullException(nameof(touristRoutePicture));
            }
            touristRoutePicture.TouristRouteId = touristRouteId;
            _context.TouristRoutePictures.Add(touristRoutePicture);
        }
    }
}
