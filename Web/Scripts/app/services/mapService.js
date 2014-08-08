app.service('mapService', ['$http', '$log', '$q', function ($http, $log, $q) {

    this.GetCenter = function () {
        return { lat: 41.881858, lng: -87.632403, zoom: 16, minZoom: 13, maxZoom: 18 };
    };

    this.GetDefaults = function () {

        //TODO: Get an application key at mapbox.com
        var mapboxApplicationKey = '';
        var theTileLayer = 'http://a.tiles.mapbox.com/v3/' + mapboxApplicationKey + '/{z}/{x}/{y}.png';

        return {
            minZoom: 13,
            maxZoom: 18,
            tileLayer: theTileLayer,
            tileLayerOptions: { attribution: '<a href="https://www.mapbox.com/about/maps/" target="_blank">Maps &copy; Mapbox &copy; OpenStreetMap</a>' },
        };

    };
}]);