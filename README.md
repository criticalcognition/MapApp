# Map App
A sample application using SQL Server 2008 Spatial Extensions, Entity Framework v6, ASP.NET Web API v2, AngularJS, and Leaflet to demonstrate how to draw shapes on a map and round trip them to SQL Server. 

## Setup
Run the script DBSetup.sql in the db directory to create the database, user, and table used in the sample.

This sample uses OpenStreetMaps and MapBox for tiling.  You will need to go to MapBox.com to acquire a developer key.  The key can then needs to be inserted into the `Scripts/app/services/mapService.js` file:

```JavaScript
var mapboxApplicationKey = '';
var theTileLayer = 'http://a.tiles.mapbox.com/v3/' + mapboxApplicationKey + '/{z}/{x}/{y}.png';
```

## The Challenge
Display custom drawn shapes on a map. Store the data in a backend that can then be searched by bounds.

### Technologies Used
[Leaflet][1]
[Wicket][2]
[AngularJS][3]
[ASP.NET Web API v2][4]
[Entity Framework v6][5]
[SQL Server 2008 Spatial Extensions][6]
[SQL Server Spatial Tools][7]

### Front End
Using the open source project Leaflet was an easy choice.  It provides a great abstraction to the mapping tile provider of your choice. Getting access to the data that defines the shape was the job of the Wicket component.  The `wkt.write()` method was all that was needed to get the Well-known text of the shape.

### API and Back End
ASP.NET Web API v2 provides an out of the box solution for creating a RESTful service. Inside the methods I used EF6 to communicate with the back end data store. Starting with v2008, SQL Server supports spatial types of data. Each shape is stored in a field of type `geolocation`. In EF, the corresponding class is called `DBGeography`. Now we just need to translate the WKT into the format that SQL Server understands. This is the job of a the SQL Server Spatial Tools component that takes WKT as input and outputs a `DBGeography` type.


```csharp
private DbGeography MakeValidGeographyFromText(string inputWkt)
{
    SqlGeography sqlPolygon = SQLSpatialTools.Functions.MakeValidGeographyFromText(inputWkt, 4326);
    return DbGeography.FromBinary(sqlPolygon.STAsBinary().Value);
}
```
When we want to retrieve data from the data, converting it to WKT is trivial.
```csharp
Wkt = Geolocation.WellKnownValue.WellKnownText
```

The sample can be extended for retrieving all GEO items that are located within specific boundaries.

```csharp
DbGeography polygon = MakeValidGeographyFromText(wkt);
var mapItemList = db.MapItems.Where(mi => mi.Geolocation.Intersects(polygon)).ToList();
```




  [1]: https://github.com/Leaflet/Leaflet
  [2]: https://github.com/arthur-e/Wicket
  [3]: https://angularjs.org/
  [4]: http://www.asp.net/web-api
  [5]: https://entityframework.codeplex.com/
  [6]: http://technet.microsoft.com/en-us/library/bb933876%28v=sql.105%29.aspx
  [7]: http://sqlspatialtools.codeplex.com/