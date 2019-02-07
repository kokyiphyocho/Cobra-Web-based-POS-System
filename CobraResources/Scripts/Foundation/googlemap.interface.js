function Initialize_GoogleMap() {
    $(document).ready(function () {
        $('[sa-elementtype=googlemap]').each(function () {
            if ($(this).attr('gmap-instantload') == 'true') {                
                $(this).CreateMapInstance();
            }
        });
    });
}

$.fn.CreateMapInstance = function () {
    var lcLatitude = parseFloat($(this).attr('gmap-latitude'));
    var lcLongitude = parseFloat($(this).attr('gmap-longitude'));
    var lcMapTypeID = parseMapTypeID($(this).attr('gmap-maptype'));
    var lcZoom = parseInt($(this).attr('gmap-zoom'));
    var lcShowMarker = $(this).attr('gmap-showmarker') == 'true';
    
    var lcMapOptions = {
        zoom: 16,
        disableDefaultUI: true,
        zoomControl: true,
        draggable: true,
        fullscreenControl: true,
        center: new google.maps.LatLng(lcLatitude, lcLongitude),
        mapTypeId: google.maps.MapTypeId.ROADMAP
    }


    $(this).attr('id', Math.random());
    var lcMap = new google.maps.Map($(this)[0], lcMapOptions);

    if (lcShowMarker) {
        lcMarker = new google.maps.Marker({ map: lcMap, position: new google.maps.LatLng(lcLatitude, lcLongitude) });
    }

    setTimeout(function () {
        google.maps.event.trigger(lcMap, 'resize');
        lcMap.setCenter(lcMarker.getPosition());
    }, 200);
}

function parseMapTypeID(paMapTypeStr) {
    switch (paMapTypeStr) {
        case 'terrain': return (google.maps.MapTypeId.TERRAIN);
        case 'roadmap': return (google.maps.MapTypeId.ROADMAP);
        case 'satellite': return (google.maps.MapTypeId.SATELLITE);
        case 'hybrid': return (google.maps.MapTypeId.HYBRID);
        default: return (google.maps.MapTypeId.ROADMAP);
    }
}


