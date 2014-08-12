app.controller('editMapController', ['$scope', '$http', '$log', '$window', '$timeout', 'mapService', 'leafletEvents', 'leafletData', function ($scope, $http, $log, $window, $timeout, mapService, leafletEvents, leafletData) {

    var url = '/api/MapItem';

    //Initialize map
    $scope.leafletCenter = mapService.GetCenter();
    $scope.leafletDefaults = mapService.GetDefaults();

    //This is currently the only way to get access to the drawnItems feature group which
    //is required to implement the edit functionality
    $scope.drawnItems = new L.FeatureGroup();
    var options = {
        edit: {
            featureGroup: $scope.drawnItems
        },
        draw: {
            circle: false
        }
    };
    var drawControl = new L.Control.Draw(options);

    $scope.leafletDraw = {
        custom: [drawControl]
    };

    //Function to retrieve a raw reference to the leaflet map.
    leafletData.getMap('leafletMap').then(function (map) {

        map.addLayer($scope.drawnItems);

        map.on('draw:created', function (e) {
            var layer = e.layer;
            //Add shape to ModalMap
            $scope.drawnItems.addLayer(layer);

            //Save WKT to the dictionary
            var wkt = new Wkt.Wkt();
            wkt.fromJson(layer.toGeoJSON());

            data = {
                "EntityType": e.layerType,
                "Wkt" : wkt.write()
            }

            $http.post(url, data).then(function (result) {
                e.layer.dbId = result.data.Id;
            });;
        });

        map.on('draw:edited', function (e) {
            var layers = e.layers;
            layers.eachLayer(function (layer) {
                var idToEdit = layer.dbId;

                //Get WKT
                var wkt = new Wkt.Wkt();
                wkt.fromJson(layer.toGeoJSON());

                data = {
                    "Id": idToEdit,
                    "Wkt": wkt.write()
                }

                $http.put(url + "/" + idToEdit, data);
            });
        });

        map.on('draw:deleted', function (e) {
            var layers = e.layers;
            layers.eachLayer(function (layer) {
                var idToEdit = layer.dbId;
                var idToDelete = layer.dbId;

                $http.delete(url + "/" + idToDelete);
            });
        });
    });

    //Get all current objects
    $http.get(url).then(function (result) {
        angular.forEach(result.data, function (value, key) {
            var wkt = new Wkt.Wkt();
            wkt.read(value.Wkt);
            var theLayer = wkt.toObject();
            theLayer.dbId = value.Id;
            $scope.drawnItems.addLayer(theLayer);
        });
    });

}]);