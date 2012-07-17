function sendCommand(imei, command) {
        $("#commandStatus").text("Sending command...");
        $.ajax({
            url: getCommandUrl(),
            type: 'POST',
            data: {IMEI: imei, cmd: command},
            success: function(result) {
                $("#commandStatus").text("Command sent.");
            }
        });
    }

    $(document).ready(function () {

        var trackData = new Object();
        var trackList = new Array();
        var updateList = function () {
            trackList = new Array();
            $("#deviceList input:checked").each(function () {
                trackList.push($(this).val());
            });
        };

        var map = null;
        var overlays = new Array();

        var greenCar = new google.maps.MarkerImage(
            getContentUrl() + 'icons/car_green_25px.png',
            new google.maps.Size(25, 25),
            new google.maps.Point(0, 0));
        var redCar = new google.maps.MarkerImage(
            getContentUrl() + 'icons/car_red_25px.png',
            new google.maps.Size(25, 25),
            new google.maps.Point(0, 0));

        var updateInfo = function (data) {
            var timestamp = new Date(parseInt(data.SatelliteTimeStamp.substr(6)) - (new Date()).getTimezoneOffset() * 60000);
            var info = 'Last update: ' + timestamp.format("mmm d, yyyy h:MM:ss TT") + '<br>' +
                (data.SOSTimeStamp == null ? "" : "<strong>SOS button pressed at " + (new Date(parseInt(data.SOSTimeStamp.substr(6)) - (new Date()).getTimezoneOffset() * 60000)).format("mmm d, yyyy h:MM:ss TT") + "</strong><br>") +
                'Speed: ' + (Math.round(data.Speed * 100) / 100) + ' km/h<br>' +
                'Mileage: ' + (Math.round(data.Mileage * 100) / 100) + ' km<br>' +
                'ADC: ' + data.ADC0 + ', Input1: ' + data.IOStatus.charAt(4) + ', Input2: ' + data.IOStatus.charAt(5) +
                ', OutputA: ' + data.IOStatus.charAt(8) + ', OutputB: ' + data.IOStatus.charAt(9) + '<br>' +
                'Engine ' + (data.IOStatus.charAt(9) == '1' ? 'disabled' : 'enabled') + '<br>' +
                '<a onclick="sendCommand(\'' + data.IMEI + '\', \'disable\');return false;" href="#">Disable Engine</a> | <a onclick="sendCommand(\'' + data.IMEI + '\', \'enable\');return false;" href="#">Enable Engine</a>';
            return info;
        };

        var updateMap = function () {

            for (var i in overlays) {
                overlays[i].setMap(null);
            }
            overlays = new Array();

            for (var d in trackData) {
                var list = trackData[d];
                var current = new google.maps.LatLng(list[0].Latitude, list[0].Longitude);

                if (map == null) {
                    var mapOptions = {
                        center: current,
                        zoom: 14,
                        mapTypeId: google.maps.MapTypeId.ROADMAP
                    };
                    map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
                }

                var icon = (list[0].IOStatus && list[0].IOStatus.charAt(5) == '1') ? greenCar : redCar;
                var marker = new google.maps.Marker({
                    position: current,
                    map: map,
                    icon: icon
                });
                map.panTo(marker.getPosition());
                overlays.push(marker);

                trackData[d][0].SOSTimeStamp = null;
                var coords = new Array();
                for (var r in list) {
                    coords.push(new google.maps.LatLng(list[r].Latitude, list[r].Longitude));

                    if (list[r].EventType == 0x01 && trackData[d][0].SOSTimeStamp == null) {
                        trackData[d][0].SOSTimeStamp = list[r].SatelliteTimeStamp;
                    }
                }

                var path = new google.maps.Polyline({
                    path: coords,
                    strokeColor: "#0000FF",
                    strokeOpacity: 0.5,
                    strokeWeight: 2
                });
                path.setMap(map);
                overlays.push(path);

                $("#info_" + trackData[d][0].IMEI).html(updateInfo(trackData[d][0]));
            }
        };

        $("#deviceList input").change(updateList);

        var getDTPicker = function (name) {
            return $(name).data("tDateTimePicker");
        };

        var toLocalDate = function (d) {
            return new Date(d.valueOf() - (new Date()).getTimezoneOffset() * 60000);
        };

        var toUtcDate = function (d) {
            return new Date(d.valueOf() + (new Date()).getTimezoneOffset() * 60000);
        };

        setTimeout(function () {
            getDTPicker("#timeRangeFrom").value(toLocalDate(getDTPicker("#timeRangeFrom").value()));
            getDTPicker("#timeRangeTo").value(toLocalDate(getDTPicker("#timeRangeTo").value()));
        }, 400);

        setInterval(function () {
            var postData;
            if ($("#dateControls input[name=timeRangeType]:checked").val() == "relative") {
                postData = {
                    IMEIs: trackList,
                    minutes: $("#timeRangeValue").val()
                };
            }
            else {
                postData = {
                    IMEIs: trackList,
                    minutes: null,
                    from: getDTPicker("#timeRangeFrom").value().format("isoUtcDateTime"),
                    to: getDTPicker("#timeRangeTo").value().format("isoUtcDateTime")
                };
            }
            $.ajax({
                url: getMultiTrackUrl(),
                type: "POST",
                data: postData,
                dataType: "json",
                traditional: true,
                success: function (result) {
                    trackData = new Object();
                    for (var t in result) {
                        if (typeof trackData[result[t].IMEI] == "undefined")
                            trackData[result[t].IMEI] = new Array();
                        trackData[result[t].IMEI].push(result[t]);
                    }
                    updateMap();
                },
                error: function (request, status, error) {
                    // alert(error);
                }
            });
        }, 10000);
    });
