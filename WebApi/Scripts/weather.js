weatherAggregator.weatherPage = {
    requestData: {Location: {} },
    initWeatherApp: function() {

        var weatherApp = angular.module('weatherApp', []).config(function ($sceProvider) {
            $sceProvider.enabled(false); //Yes, I've disabled it. And I'm happy with it.
        });

        weatherApp.controller('WeatherController', function($scope, $timeout) {
            weatherAggregator.proxy.getSources(function(response) {
                $scope.$apply(function() {
                    $scope.sources = response;
                    $timeout(weatherAggregator.weatherPage.restoreSourceChecks);
                });
            }, weatherAggregator.weatherPage.onGetWeatherComplete);

            $scope.weatherModels = [{ index: 0 }, { index: 1}, { index: 2 }, { index: 3 }];

            $scope.getWeather = weatherAggregator.weatherPage.getWeather;
        });
        
        $(window).on('beforeunload', function () {
            weatherAggregator.weatherPage.saveSourceChecks();
        });
    },

    restoreSourceChecks: function () {
        var storedSources = localStorage.getItem('Sources');
        if (storedSources != null) {
            storedSources = JSON.parse(storedSources);
            $.each(storedSources, function (j, s) {
                $('#sourcesDiv').find('input[data-sourceid=' + s.Id + ']').prop('checked', s.IsChecked);
            });
        } else {
            $('#sourcesDiv').find('input[type=checkbox]').prop('checked', true);
        }
    },

    saveSourceChecks: function () {
        var sources = [];
        $('#sourcesDiv').find('input[type=checkbox]').each(function (i, e) {
            sources.push({
                Id: $(e).data('sourceid'),
                IsChecked: $(e).is(':checked')
            });
        });
        localStorage.setItem('Sources', JSON.stringify(sources));
    },

    init: function () {
        weatherAggregator.utils.showWaiter();
        $("#tabsDiv").on('click', "a[id^='tabDay']", function () {
            $("[id^='contenttabDay']").removeClass('active');
            var location = "[id$='content" + $(this).attr('id') + "']";
            $(location).addClass('active');
            $(".tab-title").removeClass('active');
            $(this).parent().addClass('active');
        });
    },

    getWeather: function () {
        var selectedSources = $("#sourcesDiv input:checked").map(function () {
            return $(this).data("sourceid");
        }).get();

        if (!selectedSources.length) {
            alert("Пожалуйста, выберите хотя бы 1 сайт");
        } else if (weatherAggregator.weatherPage.requestData.Location.Latitude == null || weatherAggregator.weatherPage.requestData.Location.Longitude == null) {
            alert("Пожалуйста, выберите место на карте");
        } else {
            weatherAggregator.utils.showWaiter();
            var data = {
                Sources: selectedSources,
                Location: {
                    Latitude: weatherAggregator.weatherPage.requestData.Location.Latitude,
                    Longitude: weatherAggregator.weatherPage.requestData.Location.Longitude
                },
                DateRange: {
                    From: new Date(),
                    To: new Date()
                }
            };

            data.DateRange.To.setDate(data.DateRange.To.getDate() + 2);
            weatherAggregator.proxy.getWeather(data, weatherAggregator.weatherPage.bindWeather, weatherAggregator.weatherPage.onGetWeatherComplete);
        }
    },

    bindWeather: function (data) {
        if (data && data.length) {
            $(data).each(function(index, element) {
                element.index = index;
                element.active = index == 0 ? "active" : "";
                element.Precipitation = weatherAggregator.utils.getPrecipitation(element.Precipitation);
                element.cloudInfo = weatherAggregator.utils.getCloudInfo(element.Cloudness);
                element.day = weatherAggregator.utils.getDay(index);
            });
            var $scope = angular.element($("body")).scope();
            $scope.$apply(function () { $scope.weatherModels = data; });
            $("#tabsDiv").removeClass('hidden');
        }
    },

    onGetWeatherComplete: function() {
        weatherAggregator.utils.hideWaiter();
    }
};

weatherAggregator.weatherPage.initWeatherApp();
$(weatherAggregator.weatherPage.init);