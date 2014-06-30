var geocoder;
var map;
var marker;
var infowindow;

function initialize() {
    geocoder = new google.maps.Geocoder();

    var latlng = new google.maps.LatLng(55.7500, 37.6167); // just hardcoded location for situation when user turn off geolocation in his browser. Can be changed to any other location

    var mapOptions = {
        zoom: 8,
        center: latlng,
        disableDefaultUI: true,
        zoomControl: true,
        zoomControlOptions: {
            style: google.maps.ZoomControlStyle.LARGE
        }
    };

    map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
    marker = new google.maps.Marker({
        map: map
    });
    infowindow = new google.maps.InfoWindow();


    //If geolocation is turned off move map center to users location
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function(position) {
            latlng = new google.maps.LatLng(position.coords.latitude,
                position.coords.longitude);
            setMarkerWithInfoWindow(latlng, true);

        });
    } else {
        setMarkerWithInfoWindow(latlng, true);
    }

    
    //Add mouse click event
    google.maps.event.addListener(map, 'click', function (e) {
        setMarkerWithInfoWindow(e.latLng, false);
    });
}

function setMarkerWithInfoWindow(latlng, centerize) {
    infowindow.close();// we close info window because calculation data for new window can takes time, so it is better to hide it.
    marker.setPosition(latlng);
    storingCoordinates(latlng);
    showInfoWindow(latlng, infowindow);
    
    if (centerize) {
        map.setCenter(latlng);
    }
}

function storingCoordinates(latlng) {
    weatherAggregator.weatherPage.requestData.Location.Latitude = latlng.lat();
    weatherAggregator.weatherPage.requestData.Location.Longitude = latlng.lng();
}


function showInfoWindow(latlng, infowindow) {
    geocoder.geocode({ 'latLng': latlng }, function (results, status) {
        var message = "";
        if (status == google.maps.GeocoderStatus.OK) {
            if (results[0]) {
                message = results[0].formatted_address;
            }
            else { message = "Неизвестное место"; }
        } else { message = "Неизвестное место"; };
        infowindow.setPosition(latlng);
        infowindow.setContent(message);
        infowindow.open(map, marker);
    });
}

google.maps.event.addDomListener(window, 'load', initialize);