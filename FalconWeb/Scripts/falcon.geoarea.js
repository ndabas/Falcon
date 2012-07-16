$(document).ready(function () {
    var mapOptions = {
        center: new google.maps.LatLng(28.633462, 77.219853),
        zoom: 11,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
    var markers = new Array();
    var isClosed = false;
    var poly = new google.maps.Polyline({
        map: map,
        path: [],
        strokeColor: "#FF0000",
        strokeOpacity: 1.0,
        strokeWeight: 2
    });
    var closePoly = function () {
        if (isClosed)
            return;
        var path = poly.getPath();
        poly.setMap(null);
        poly = new google.maps.Polygon({
            map: map,
            path: path,
            strokeColor: "#FF0000",
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: "#FF0000",
            fillOpacity: 0.35
        });
        isClosed = true;
    };

    var savePoints = function () {
        var path = poly.getPath().getArray();
        var points = new Array();
        for (var p in path) {
            points.push(path[p].lat() + "," + path[p].lng());
        }
        $("#LatLongs").val(points.join("|"));
    };

    var loadPoints = function () {
        var val = $("#LatLongs").val();
        if (val.length > 0) {
            var points = val.split("|");
            var bounds = new google.maps.LatLngBounds();
            for (var p in points) {
                (function () {
                    var ll = points[p].split(",");
                    var point = new google.maps.LatLng(parseFloat(ll[0]), parseFloat(ll[1]));
                    var marker = new google.maps.Marker({
                        map: map,
                        position: point,
                        draggable: true,
                        title: poly.getPath().getLength().toString()
                    });
                    markers.push(marker);
                    google.maps.event.addListener(marker, 'drag', function (dragEvent) {
                        poly.getPath().setAt(parseInt(marker.getTitle()), dragEvent.latLng);
                        savePoints();
                    });
                    poly.getPath().push(point);
                    bounds.extend(point);
                })();
            }
            closePoly();
            map.setCenter(bounds.getCenter());
        }
    };
    loadPoints();

    $("#resetButton").click(function () {
        isClosed = false;
        poly.setMap(null);
        poly = new google.maps.Polyline({
            map: map,
            path: [],
            strokeColor: "#FF0000",
            strokeOpacity: 1.0,
            strokeWeight: 2
        });
        for (var m in markers) {
            markers[m].setMap(null);
        }
        markers = new Array();
        return false;
    });

    $("#doneButton").click(function () {
        closePoly();
        return false;
    });

    google.maps.event.addListener(map, 'click', function (clickEvent) {
        if (isClosed)
            return;
        var isFirstMarker = poly.getPath().length === 0;
        var marker = new google.maps.Marker({
            map: map,
            position: clickEvent.latLng,
            draggable: true,
            title: poly.getPath().getLength().toString()
        });
        markers.push(marker);
        if (isFirstMarker) {
            google.maps.event.addListener(marker, 'click', closePoly);
        }
        google.maps.event.addListener(marker, 'drag', function (dragEvent) {
            poly.getPath().setAt(parseInt(marker.getTitle()), dragEvent.latLng);
        });
        poly.getPath().push(clickEvent.latLng);

        savePoints();
    });
});
