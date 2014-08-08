app.controller('editMapController', ['$scope', '$http', '$log', '$window', '$timeout', 'mapService', 'leafletEvents', 'leafletData', function ($scope, $http, $log, $window, $timeout, mapService, leafletEvents, leafletData) {

    $scope.shapeLayerGroup = new L.FeatureGroup();
    $scope.wktDictionary = {};

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
            $scope.wktDictionary[layer._leaflet_id] = wkt.write();
        });

        map.on('draw:deleted', function (e) {
            //Remove shape from dictionary
            angular.forEach(e.layers.getLayers(), function (value, key) {
                delete $scope.wktDictionary[value._leaflet_id];
            });
        });
    });




}]);