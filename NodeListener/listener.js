var net = require('net');
var http = require('http');

var server = net.createServer(function(socket) {
    console.log('Connected');
    socket.on('data', function(data) {
        console.log("" + data);
        http.get("http://track.gpsconvergence.com/TrackingData/Report2?data=" + encodeURIComponent("" + data), function(response) {
            response.on('data', function(chunk) {
            	console.log("-> [" + response.statusCode + "] " + chunk);
            });
        });
    });
});

server.listen(8191);
