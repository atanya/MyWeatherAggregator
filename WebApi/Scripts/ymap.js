ymaps.ready(init);
defaultPlace = [55.76, 37.64]; // по умолчанию москва


function init() {
    var isSearch = false;
    var myPlacemark,
        myMap = new ymaps.Map('map-canvas', {
            center: defaultPlace,
            zoom: 9,
            controls: ['smallMapDefaultSet']
        });

    var searchControl = new ymaps.control.SearchControl({
        options: {
            kind: 'locality',
            //useMapBounds : true,
            noSelect: false,
            noPlacemark: true,
            noPopup: false,

        }
    });
    searchControl.events.add('resultshow', function (event) {
        isSearch = true;
    });

    searchControl.events.add('load', function (event) {
        // Проверяем, что это событие не "дозагрузки" результатов и
        // по запросу найден хотя бы один результат.
        if (!event.get('skip') && searchControl.getResultsCount()) {
            searchControl.showResult(0);
        }
    });

    myMap.controls.add(searchControl);

    // Слушаем клик на карте
    myMap.events.add('click', function(e) {
        var clickCoords = e.get('coords');
        setPlacemark(clickCoords, false);
    });

    myMap.events.add('boundschange', function(e) {
        if (isSearch) {
            var  newCenter = e.get('newCenter');
            setPlacemark(newCenter, false);
            isSearch = false;
        }
    });

    var storeButton = new ymaps.control.Button({
        data: {
            content: 'Запомнить',
            title: 'Запомнить место'
        },
        options: {
            selectOnClick: false,
            position : {
                left: 1,
                bottom: 1
            }

        }
    });

    storeButton.events.add('click', function() {
        var coordinates = myPlacemark.geometry.getCoordinates();
        setCookie("longitude", coordinates[0]);
        setCookie("latitude", coordinates[1]);
    });

    myMap.controls.add(storeButton);

    
    var cLng = getCookie("longitude");
    var cLtd = getCookie("latitude");
    if (cLng && cLtd) {
        setPlacemark([cLng, cLtd], true);
    } else {
        setPlaceMartByGeolocation();
    }

    function setPlaceMartByGeolocation() {
        ymaps.geolocation.get({
                provider: 'yandex',
                mapStateAutoApply: true
            })
            .then(function(result) {
                var position = result.geoObjects.position;
                setPlacemark(position, true);
            });

        ymaps.geolocation.get({
                provider: 'browser',
                mapStateAutoApply: true
            })
            .then(function(result) {
                var position = result.geoObjects.position;
                setPlacemark(position, true);
            });
    }

    function setPlacemark(placemarkCoords, centerize) {
        // Если метка уже создана – просто передвигаем ее
        if (myPlacemark) {
            myPlacemark.geometry.setCoordinates(placemarkCoords);
        }
        // Если нет – создаем.
        else {
            myPlacemark = createPlacemark(placemarkCoords);
            myMap.geoObjects.add(myPlacemark);

            // Слушаем событие окончания перетаскивания на метке.
            //myPlacemark.events.add('dragend', function() {
            //    getAddress(myPlacemark.geometry.getCoordinates());
            //});
        }

        storingCoordinates(placemarkCoords);

        getAddress(placemarkCoords);


        if (centerize) {
            myMap.setCenter(placemarkCoords);
        }
        myPlacemark.balloon.close();
        myPlacemark.balloon.open();


    }

    // Создание метки
    function createPlacemark(coords) {
        return new ymaps.Placemark(coords, {
            iconContent: 'поиск...',
            balloonContent: 'поиск...'
        }, {
            preset: 'islands#violetStretchyIcon',
            //draggable: true,
            balloonPanelMaxMapArea: 0
        });
    };

    // Определяем адрес по координатам (обратное геокодирование)
    function getAddress(addressCoords) {
        myPlacemark.properties.set('iconContent', 'поиск...');
        myPlacemark.properties.set('balloonContent', 'поиск...');
        ymaps.geocode(addressCoords).then(function (res) {
            var firstGeoObject = res.geoObjects.get(0);
            myPlacemark.properties.set({
                iconContent: firstGeoObject.properties.get('name'),
                balloonContent: firstGeoObject.properties.get('text')
            });

            storingAddressText(firstGeoObject);
        });
    };

    function storingCoordinates(storingCoords) {
        weatherAggregator.weatherPage.requestData.Location.Latitude = storingCoords[0];
        weatherAggregator.weatherPage.requestData.Location.Longitude = storingCoords[1];
    };

    function storingAddressText(firstGeoObject) {
        weatherAggregator.weatherPage.requestData.Location.AddressText =
            firstGeoObject.properties.get('text');
        weatherAggregator.weatherPage.requestData.Location.Country =
            firstGeoObject.properties.get('metaDataProperty.GeocoderMetaData.AddressDetails.Country.CountryName');
        weatherAggregator.weatherPage.requestData.Location.Region =
            firstGeoObject.properties.get('metaDataProperty.GeocoderMetaData.AddressDetails.Country.AdministrativeArea.AdministrativeAreaName');
        weatherAggregator.weatherPage.requestData.Location.City =
            firstGeoObject.properties.get('metaDataProperty.GeocoderMetaData.AddressDetails.Country.AdministrativeArea.SubAdministrativeArea.SubAdministrativeAreaName');
    }

    function setCookie(key, value) {
        var expires = new Date();
        expires.setTime(expires.getTime() + (1 * 24 * 60 * 60 * 1000));
        document.cookie = key + '=' + value + ';expires=' + expires.toUTCString();
    }

    function getCookie(key) {
        var keyValue = document.cookie.match('(^|;) ?' + key + '=([^;]*)(;|$)');
        return keyValue ? keyValue[2] : null;
    }

}