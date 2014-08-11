using MapApp.DAL;
using MapApp.Web.Models;
using Microsoft.SqlServer.Types;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace MapApp.Web.Controllers
{
    public class MapItemController : ApiController
    {
        private MapAppContext db = new MapAppContext(); 

        // GET api/<controller>/5
        [ResponseType(typeof(MapItemDTO))]
        public async Task<IHttpActionResult> Get(int id)
        {
            var mItem = await db.MapItems.Select(mi =>
                new MapItemDTO()
                {
                    Id = mi.Id,
                    EntityType = mi.EntityType,
                    Wkt = mi.Geolocation.WellKnownValue.WellKnownText
                }).SingleOrDefaultAsync(b => b.Id == id);
            if (mItem == null)
            {
                return NotFound();
            }

            return Ok(mItem);
        }

        public IQueryable<MapItemDTO> GetMapItems()
        {
            var query = db.MapItems.Select(mi =>
                        new MapItemDTO()
                        {
                            Id = mi.Id,
                            EntityType = mi.EntityType,
                            Wkt = mi.Geolocation.WellKnownValue.WellKnownText
                        });

            return query;
        }

        // POST api/<controller>
        [ResponseType(typeof(MapItem))]
        public async Task<IHttpActionResult> Post(MapItemDTO dto)
        {
            MapItem mItem = new MapItem()
            {
                EntityType = dto.EntityType,
                Geolocation = MakeValidGeographyFromText(dto.Wkt)
            };

            db.MapItems.Add(mItem);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = mItem.Id }, dto);
        }

        // PUT api/<controller>/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(int id, MapItemDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            MapItem mItem = new MapItem() {
                Id = dto.Id,
                EntityType = dto.EntityType,
                Geolocation = MakeValidGeographyFromText(dto.Wkt)
            };
            db.Entry(mItem).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MapItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE api/<controller>/5
        [ResponseType(typeof(MapItem))]
        public async Task<IHttpActionResult> Delete(int id)
        {
            MapItem mItem = await db.MapItems.FindAsync(id);
            if (mItem == null)
            {
                return NotFound();
            }

            db.MapItems.Remove(mItem);
            await db.SaveChangesAsync();

            return Ok(mItem);
        }

        private DbGeography MakeValidGeographyFromText(string inputWkt)
        {
            SqlGeography sqlPolygon = SQLSpatialTools.Functions.MakeValidGeographyFromText(inputWkt, 4326);
            return DbGeography.FromBinary(sqlPolygon.STAsBinary().Value);
        }

        private bool MapItemExists(int id)
        {
            return db.MapItems.Count(e => e.Id == id) > 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}