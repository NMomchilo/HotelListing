using HotelListing.Data;
using HotelListing.IReposiroty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Reposiroty
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext context;
        private IGenericRepository<Country> countries;
        private IGenericRepository<Hotel> hotels;

        public UnitOfWork(DatabaseContext context)
        {
            this.context = context;
        }

        public IGenericRepository<Country> Countries => this.countries ??= new GenericRepository<Country>(context);

        public IGenericRepository<Hotel> Hotels => this.hotels ??= new GenericRepository<Hotel>(context);

        public void Dispose()
        {
            this.context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Save()
        {
            await this.context.SaveChangesAsync();
        }
    }
}
