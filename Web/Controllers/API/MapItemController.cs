using MapApp.DAL;
using MapApp.Web.Models;
using Microsoft.SqlServer.Types;
using System.Collections.Generic;
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

        // GET api/MapItem/5
        [ResponseType(typeof(MapItemDTO))]
        public async Task<IHttpActionResult> GetMapItem(int id)
        {
            var mItem = await db.MapItems.SingleOrDefaultAsync(b => b.Id == id);
 
            if (mItem == null)
            {
                return NotFound();
            }

            MapItemDTO dto = new MapItemDTO()
                            {
                                Id = mItem.Id,
                                EntityType = mItem.EntityType,
                                Wkt = mItem.Geolocation.WellKnownValue.WellKnownText
                            };

            return Ok(dto);
        }

        // GET: api/MapItem
        public List<MapItemDTO> GetMapItems()
        {
            //Need to materialize the items ToList first otherwise we would get this exception:
            //"The specified type member 'WellKnownValue' is not supported in LINQ to Entities. Only initializers, entity members, 
            //and entity navigation properties are supported."

            var dtoList = db.MapItems.ToList().Select(mi =>
                            new MapItemDTO()
                            {
                                Id = mi.Id,
                                EntityType = mi.EntityType,
                                Wkt = mi.Geolocation.WellKnownValue.WellKnownText
                            }).ToList();

            return dtoList;
        }

        // POST api/MapItem
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
            dto.Id = mItem.Id;
            return CreatedAtRoute("DefaultApi", new { id = mItem.Id }, dto);
        }

        // PUT api/MapItem/5
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

            return StatusCode(HttpStatusCode.OK);
        }

        // DELETE api/MapItem/5
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

            return StatusCode(HttpStatusCode.OK);
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